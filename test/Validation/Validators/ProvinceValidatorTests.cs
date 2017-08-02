using Fidget.Validation.Addresses.Metadata;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fidget.Validation.Addresses.Validation.Validators
{
    public class ProvinceValidatorTests
    {
        IAddressValidator instance => new ProvinceValidator();

        ValidationContext context = new ValidationContext();
        IEnumerable<AddressValidationFailure> invoke() => instance.Validate( context );

        [Fact]
        public void IsRegistered()
        {
            var actual = DependencyInjection.Container.GetAllInstances<IAddressValidator>();
            Assert.Contains( actual, _ => _ is ProvinceValidator );
        }

        /// <summary>
        /// Cases where the province should be considered valid.
        /// </summary>
        
        public static IEnumerable<object[]> ValidCases()
        {
            // cases where the province is not specified in the address
            var unspecified =
                from value in new string[] { null, string.Empty, "\t" }
                from country in new CountryMetadata[] { null, new CountryMetadata(), new CountryMetadata { ChildRegionKeys = new string[] { "XX" } } }
                from province in new ProvinceMetadata[] { null, new ProvinceMetadata { Key = "XX" } }
                select new object[] { value, country, province };

            // cases where the country does not specify any children
            var freeForm = 
                from country in new CountryMetadata[] { null, new CountryMetadata(), new CountryMetadata { ChildRegionKeys = new string[0] } }
                from province in new ProvinceMetadata[] { null, new ProvinceMetadata { Key = "XX" } }
                select new object[] { "XX", country, province };
                
            // cases where the province is specified and the metadata key is in the parent children
            var valid = new object[][]
            {
                new object[] { "XXProvince", new CountryMetadata { ChildRegionKeys = new string[] { "XX" } }, new ProvinceMetadata { Key = "XX" } },
            };
                
            return valid.Union( unspecified ).Union( freeForm );
        }

        [Theory]
        [MemberData(nameof(ValidCases))]
        public void Validate_whenValid_returnsSuccess( string value, CountryMetadata country, ProvinceMetadata province )
        {
            context.Address = new AddressData { Province = value };
            context.CountryMetadata = country;
            context.ProvinceMetadata = province;

            var actual = invoke();
            Assert.Empty( actual );
        }

        public static IEnumerable<object[]> InvalidCases()
        {
            var keys = new string[] { "XA", "XX", "XB" };

            return
                from province in new ProvinceMetadata[] { null, new ProvinceMetadata { Key = "XXProvince" } }
                select new object[] { "XXProvince", new CountryMetadata { ChildRegionKeys = keys }, province };
        }

        [Theory]
        [MemberData(nameof(InvalidCases))]
        public void Validate_whenNotValid_returnsFailure( string value, CountryMetadata country, ProvinceMetadata province )
        {
            // when the child region is specified but its metadata does not match a key in the parent
            context.Address = new AddressData { Province = value };
            context.CountryMetadata = country;
            context.ProvinceMetadata = province;

            var expected = new AddressValidationFailure { Field = AddressField.Province, Error = AddressFieldError.UnkownValue };
            var actual = invoke();

            Assert.Equal( expected, actual.Single() );
        }
    }
}