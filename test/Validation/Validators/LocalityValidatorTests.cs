using Fidget.Validation.Addresses.Metadata;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fidget.Validation.Addresses.Validation.Validators
{
    public class LocalityValidatorTests
    {
        IAddressValidator instance => new LocalityValidator();

        ValidationContext context = new ValidationContext();
        IEnumerable<AddressValidationFailure> invoke() => instance.Validate( context );

        [Fact]
        public void IsRegistered()
        {
            var actual = DependencyInjection.Container.GetAllInstances<IAddressValidator>();
            Assert.Contains( actual, _ => _ is LocalityValidator );
        }

        /// <summary>
        /// Cases where the province should be considered valid.
        /// </summary>

        public static IEnumerable<object[]> ValidCases()
        {
            // cases where the value is not specified in the address
            var unspecified =
                from value in new string[] { null, string.Empty, "\t" }
                from parent in new ProvinceMetadata[] { null, new ProvinceMetadata(), new ProvinceMetadata { ChildRegionKeys = new string[] { "XX" } } }
                from child in new LocalityMetadata[] { null, new LocalityMetadata { Key = "XX" } }
                select new object[] { value, parent, child };

            // cases where the parent does not specify any children
            var freeForm =
                from parent in new ProvinceMetadata[] { null, new ProvinceMetadata(), new ProvinceMetadata { ChildRegionKeys = new string[0] } }
                from child in new LocalityMetadata[] { null, new LocalityMetadata { Key = "XX" } }
                select new object[] { "XX", parent, child };

            // cases where the child is specified and the metadata key is in the parent
            var valid = new object[][]
            {
                new object[] { "XXChild", new ProvinceMetadata { ChildRegionKeys = new string[] { "XX" } }, new LocalityMetadata { Key = "XX" } },
            };

            return valid.Union( unspecified ).Union( freeForm );
        }

        [Theory]
        [MemberData( nameof( ValidCases ) )]
        public void Validate_whenValid_returnsSuccess( string value, ProvinceMetadata parent, LocalityMetadata child )
        {
            context.Address = new AddressData { Locality = value };
            context.ProvinceMetadata = parent;
            context.LocalityMetadata = child;

            var actual = invoke();
            Assert.Empty( actual );
        }

        public static IEnumerable<object[]> InvalidCases()
        {
            var keys = new string[] { "XA", "XX", "XB" };

            return
                from child in new LocalityMetadata[] { null, new LocalityMetadata { Key = "XXChild" } }
                select new object[] { "XXChild", new ProvinceMetadata { ChildRegionKeys = keys }, child };
        }

        [Theory]
        [MemberData( nameof( InvalidCases ) )]
        public void Validate_whenNotValid_returnsFailure( string value, ProvinceMetadata parent, LocalityMetadata child )
        {
            // when the child region is specified but its metadata does not match a key in the parent
            context.Address = new AddressData { Locality = value };
            context.ProvinceMetadata = parent;
            context.LocalityMetadata = child;

            var expected = new AddressValidationFailure { Field = AddressField.Locality, Error = AddressFieldError.UnkownValue };
            var actual = invoke();

            Assert.Equal( expected, actual.Single() );
        }
    }
}