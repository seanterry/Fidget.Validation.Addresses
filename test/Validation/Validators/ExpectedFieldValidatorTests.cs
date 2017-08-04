using Fidget.Validation.Addresses.Metadata;
using System.Collections.Generic;
using Xunit;

namespace Fidget.Validation.Addresses.Validation.Validators
{
    public class ExpectedFieldValidatorTests
    {
        IAddressValidator instance => new ExpectedFieldValidator();

        ValidationContext context = new ValidationContext();
        IEnumerable<AddressValidationFailure> invoke() => instance.Validate( context );

        [Fact]
        public void IsRegistered()
        {
            var actual = DependencyInjection.Container.GetAllInstances<IAddressValidator>();
            Assert.Contains( actual, _=> _ is ExpectedFieldValidator );
        }

        public static IEnumerable<object[]> ValidationCases = new object[][]
        {
            new object[] { AddressField.Locality, new AddressData { Locality = "ABC" } },
            new object[] { AddressField.Name, new AddressData { Name = "ABC" } },
            new object[] { AddressField.Organization, new AddressData { Organization = "ABC" } },
            new object[] { AddressField.PostalCode, new AddressData { PostalCode = "ABC" } },
            new object[] { AddressField.Province, new AddressData { Province = "ABC" } },
            new object[] { AddressField.SortingCode, new AddressData { SortingCode = "ABC" } },
            new object[] { AddressField.StreetAddress, new AddressData { StreetAddress = "ABC" } },
            new object[] { AddressField.Sublocality, new AddressData { Sublocality = "ABC" } },
        };

        [Theory]
        [MemberData(nameof(ValidationCases))]
        public void Validate_whenNotAllowed_returnsFailure( AddressField field, AddressData address )
        {
            context.Address = address;
            context.CountryMetadata = new CountryMetadata { Format = $"NOADCSZX%nnoadcszx" };

            var expected = new AddressValidationFailure { Field = field, Error = AddressFieldError.UnexpectedField };
            var actual = invoke();
            Assert.Contains( expected, actual );
        }

        [Theory]
        [MemberData( nameof( ValidationCases ) )]
        public void Validate_whenAllowed_returnsSuccess( AddressField field, AddressData address )
        {
            context.Address = address;
            context.CountryMetadata = new CountryMetadata { Format = $"NOADCSZX%n%{(char)field}noadcszx" };

            var expected = new AddressValidationFailure { Field = field, Error = AddressFieldError.UnexpectedField };
            var actual = invoke();
            Assert.DoesNotContain( expected, actual );
        }
    }
}