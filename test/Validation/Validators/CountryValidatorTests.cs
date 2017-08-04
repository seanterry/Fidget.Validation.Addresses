using Fidget.Validation.Addresses.Metadata;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fidget.Validation.Addresses.Validation.Validators
{
    public class CountryValidatorTests
    {
        IAddressValidator instance => new CountryValidator();

        ValidationContext context = new ValidationContext();
        IEnumerable<AddressValidationFailure> invoke() => instance.Validate( context );

        [Fact]
        public void IsRegistered()
        {
            var actual = DependencyInjection.Container.GetAllInstances<IAddressValidator>();
            Assert.Contains( actual, _ => _ is CountryValidator );
        }

        /// <summary>
        /// Cases where the country should be considered valid.
        /// </summary>
        
        public static IEnumerable<object[]> ValidCases()
        {
            // cases where the country is not specified in the address
            // the country being unspecified is a different validator
            var unspecified =
                from value in new string[] { null, string.Empty, "\t" }
                from country in new CountryMetadata[] { null, new CountryMetadata() }
                select new object[] { value, country };

            // cases where the country is specified and its metadata exists
            var valid = new object[][]
            {
                new object[] { "ABC", new CountryMetadata() },
            };
                
            return valid.Union( unspecified );
        }

        [Theory]
        [MemberData(nameof(ValidCases))]
        public void Validate_whenValid_returnsSuccess( string country, CountryMetadata meta )
        {
            context.Address = new AddressData { Country = country };
            context.CountryMetadata = meta;

            var actual = invoke();
            Assert.Empty( actual );
        }

        [Fact]
        public void Validate_whenNotValid_returnsFailure()
        {
            // when the country is specified but has no metadata, a failure should be reported
            context.Address = new AddressData { Country = "XW" };
            context.CountryMetadata = null;

            var expected = new AddressValidationFailure { Field = AddressField.Country, Error = AddressFieldError.UnkownValue };
            var actual = invoke();

            Assert.Equal( expected, actual.Single() );
        }
    }
}