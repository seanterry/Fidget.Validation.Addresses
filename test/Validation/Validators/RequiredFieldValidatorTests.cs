using Fidget.Validation.Addresses.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fidget.Validation.Addresses.Validation.Validators
{
    public class RequiredFieldValidatorTests
    {
        IAddressValidator instance => new RequiredFieldValidator();

        ValidationContext context = new ValidationContext();
        IEnumerable<AddressValidationFailure> invoke() => instance.Validate( context );

        [Fact]
        public void IsRegistered()
        {
            var actual = DependencyInjection.Container.GetAllInstances<IAddressValidator>();
            Assert.Contains( actual, _=> _ is RequiredFieldValidator );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        [InlineData( "\t" )]
        public void Validate_Country_alwaysRequired( string country )
        {
            context = new ValidationContext { Address = new AddressData { Country = country } };
            var actual = invoke();
            var expected = new AddressValidationFailure { Field = AddressField.Country, Error = AddressFieldError.MissingRequiredField };
            Assert.Equal( expected, actual.Single() );
        }

        /// <summary>
        /// Cases where required fields should be additive among regions.
        /// </summary>
        
        public static IEnumerable<object[]> AdditiveCases()
        {
            var fields = Enum.GetValues( typeof(AddressField) )
                .OfType<AddressField>()
                .Where( _=> _ != AddressField.Country )
                .ToArray();

            for ( var i = 0; i < fields.Length; i++ )
            {
                var values = fields.Skip( i ).Take( 4 ).ToArray();

                if ( values.Length == 4 )
                {
                    yield return new object[] { values[0], values[1], values[2], values[3] };
                }
            }
        }
        
        [Theory]
        [MemberData(nameof(AdditiveCases))]
        public void Validate_Required_isAdditive( AddressField field1, AddressField field2, AddressField field3, AddressField field4 )
        {
            context.Address = new AddressData {};
            context.CountryMetadata = new CountryMetadata { Required = new AddressField[] { field1 } };
            context.ProvinceMetadata = new ProvinceMetadata { Required = new AddressField[] { field2 } };
            context.LocalityMetadata = new LocalityMetadata { Required = new AddressField[] { field3 } };
            context.SublocalityMetadata = new SublocalityMetadata { Required = new AddressField[] { field4 } };

            var expected = new AddressValidationFailure[]
            {
                new AddressValidationFailure { Field = AddressField.Country, Error = AddressFieldError.MissingRequiredField },
                new AddressValidationFailure { Field = field1, Error = AddressFieldError.MissingRequiredField },
                new AddressValidationFailure { Field = field2, Error = AddressFieldError.MissingRequiredField },
                new AddressValidationFailure { Field = field3, Error = AddressFieldError.MissingRequiredField },
                new AddressValidationFailure { Field = field4, Error = AddressFieldError.MissingRequiredField },
            }
                .OrderBy( _=> _.Field )
                .ThenBy( _=> _.Error )
                .ToArray();

            var actual = invoke();
            Assert.Equal( expected, actual );
        }

        public static IEnumerable<object[]> FailureCases()
        {
            var regions = new RegionalMetadata[]
            {
                new CountryMetadata(),
                new ProvinceMetadata(),
                new LocalityMetadata(),
                new SublocalityMetadata(),
            };

            var values = new string[] { null, string.Empty, "\t" };
            var fields = Enum.GetValues( typeof( AddressField ) )
                .OfType<AddressField>()
                .ToArray();

            return
                from value in values
                from field in fields
                from region in regions
                select new object[] { value, field, region };
        }

        [Theory]
        [MemberData(nameof(FailureCases))]
        public void Validate_whenRequiredFieldNotProvided_returnsFailure( string value, AddressField field, RegionalMetadata region )
        {
            context.Address = new AddressData
            {
                Country = value,
                Language = value,
                Locality = value,
                Name = value,
                Organization = value,
                PostalCode = value,
                Province = value,
                SortingCode = value,
                StreetAddress = value,
                Sublocality = value,
            };

            region.Required = new AddressField[] { field };
            context.CountryMetadata = region as CountryMetadata;
            context.ProvinceMetadata = region as ProvinceMetadata;
            context.LocalityMetadata = region as LocalityMetadata;
            context.SublocalityMetadata = region as SublocalityMetadata;

            var expected = new AddressValidationFailure { Field = field, Error = AddressFieldError.MissingRequiredField };
            var actual = invoke();
            Assert.Contains( expected, actual );
        }

        /// <summary>
        /// Cases where the field should not report a failure.
        /// </summary>
        
        public static IEnumerable<object[]> SuccessCases()
        {
            yield return new object[] { AddressField.Locality, new AddressData { Locality = "abc" } };
            yield return new object[] { AddressField.Name, new AddressData { Name = "abc" } };
            yield return new object[] { AddressField.Organization, new AddressData { Organization = "abc" } };
            yield return new object[] { AddressField.PostalCode, new AddressData { PostalCode = "abc" } };
            yield return new object[] { AddressField.Province, new AddressData { Province = "abc" } };
            yield return new object[] { AddressField.SortingCode, new AddressData { SortingCode = "abc" } };
            yield return new object[] { AddressField.StreetAddress, new AddressData { StreetAddress = "abc" } };
            yield return new object[] { AddressField.Sublocality, new AddressData { Sublocality = "abc" } };
        }

        [Theory]
        [MemberData(nameof(SuccessCases))]
        public void Validate_whenFieldProvided_returns_success( AddressField field, AddressData address )
        {
            var required = new AddressField[] { field };
            
            context.Address = address;
            context.CountryMetadata = new CountryMetadata { Required = required };
            context.ProvinceMetadata = new ProvinceMetadata { Required = required };
            context.LocalityMetadata = new LocalityMetadata { Required = required };
            context.SublocalityMetadata = new SublocalityMetadata { Required = required };

            var expected = new AddressValidationFailure { Field = field, Error = AddressFieldError.MissingRequiredField };
            var actual = invoke();

            Assert.DoesNotContain( expected, actual );
        }
    }
}