using Fidget.Validation.Addresses.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Fidget.Validation.Addresses.Validation.Validators
{
    public class PostalCodeValidatorTests
    {
        IAddressValidator instance = new PostalCodeValidator();

        ValidationContext context = new ValidationContext();
        IEnumerable<AddressValidationFailure> invoke() => instance.Validate( context );

        [Fact]
        public void IsRegistered()
        {
            var actual = DependencyInjection.Container.GetAllInstances<IAddressValidator>();
            Assert.Contains( actual, _ => _ is PostalCodeValidator );
        }

        /// <summary>
        /// Cases where the pattern is valid.
        /// </summary>

        public static IEnumerable<object[]> ValidPatternCases()
        {
            var control = Convert.ToBase64String( Guid.NewGuid().ToByteArray() );
            var pattern = @"(\d{5})(?:[ \-](\d{4}))?";
            var values = new string[]
            {
                // undefined values
                null,
                string.Empty,
                "\t",

                // values that match the patterns
                "72000",
                "72000-0000",
            };

            var noPattern =
                from value in values
                select new object[] { value, null, null, null, null };

            var validInCountry =
                from value in values
                select new object[] { value, pattern, null, null, null };

            var validInProvince =
                from value in values
                select new object[] { value, control, pattern, null, null };

            var validInLocality =
                from value in values
                select new object[] { value, control, control, pattern, null };

            var validInSubLocality =
                from value in values
                select new object[] { value, control, control, control, pattern };

            return noPattern
                .Union( validInCountry )
                .Union( validInProvince )
                .Union( validInLocality )
                .Union( validInSubLocality );
        }

        [Theory]
        [MemberData( nameof( ValidPatternCases ) )]
        public void Validate_whenPatternValidInOverride_returns_success( string value, string country, string province, string locality, string sublocality )
        {
            context.Address = new AddressData { PostalCode = value };
            context.CountryMetadata = country != null ? new CountryMetadata { PostalCodePattern = country } : null;
            context.ProvinceMetadata = province != null ? new ProvinceMetadata { PostalCodePatternOverride = province } : null;
            context.LocalityMetadata = locality != null ? new LocalityMetadata { PostalCodePatternOverride = locality } : null;
            context.SublocalityMetadata = sublocality != null ? new SublocalityMetadata { PostalCodePatternOverride = sublocality } : null;

            var actual = invoke();
            Assert.Empty( actual );
        }

        /// <summary>
        /// Cases where the pattern is not valid.
        /// </summary>

        public static IEnumerable<object[]> InvalidPatternCases()
        {
            var control = Convert.ToBase64String( Guid.NewGuid().ToByteArray() );
            var pattern = @"(\d{5})(?:[ \-](\d{4}))?";
            var values = new string[]
            {
                // values that match the pattern
                "72000",
                "72000-0000",
            };

            // test cases override the valid pattern with an invalid one
            var invalidInCountry =
                from value in values
                select new object[] { value, control, null, null, null };

            var invalidInProvince =
                from value in values
                select new object[] { value, pattern, control, null, null };

            var invalidInLocality =
                from value in values
                select new object[] { value, pattern, pattern, control, null };

            var invalidInSubLocality =
                from value in values
                select new object[] { value, pattern, pattern, pattern, control };

            return invalidInCountry
                .Union( invalidInProvince )
                .Union( invalidInLocality )
                .Union( invalidInSubLocality );
        }

        [Theory]
        [MemberData( nameof( InvalidPatternCases ) )]
        public void Validate_whenPatternInvalidInOverride_returns_failure( string value, string country, string province, string locality, string sublocality )
        {
            context.Address = new AddressData { PostalCode = value };
            context.CountryMetadata = country != null ? new CountryMetadata { PostalCodePattern = country } : null;
            context.ProvinceMetadata = province != null ? new ProvinceMetadata { PostalCodePatternOverride = province } : null;
            context.LocalityMetadata = locality != null ? new LocalityMetadata { PostalCodePatternOverride = locality } : null;
            context.SublocalityMetadata = sublocality != null ? new SublocalityMetadata { PostalCodePatternOverride = sublocality } : null;

            var expected = new AddressValidationFailure { Field = AddressField.PostalCode, Error = AddressFieldError.InvalidFormat };
            var actual = invoke();
            Assert.Equal( expected, actual.Single() );
        }

        /// <summary>
        /// In cases where both the pattern and the prefix are not valid, we should only return the error for the pattern.
        /// </summary>
        
        [Theory]
        [MemberData( nameof(InvalidPatternCases))]
        public void Validate_whenPatternAndPrefixInvalid_returns_patternFailure( string value, string country, string province, string locality, string sublocality )
        {
            var prefix = Convert.ToBase64String( Guid.NewGuid().ToByteArray() );
            context.Address = new AddressData { PostalCode = value };
            context.CountryMetadata = new CountryMetadata { PostalCodePattern = country };
            context.ProvinceMetadata = new ProvinceMetadata { PostalCodePatternOverride = province, PostalCodePattern = prefix };
            context.LocalityMetadata = new LocalityMetadata { PostalCodePatternOverride = locality, PostalCodePattern = prefix };
            context.SublocalityMetadata = new SublocalityMetadata { PostalCodePatternOverride = sublocality, PostalCodePattern = prefix };

            var expected = new AddressValidationFailure { Field = AddressField.PostalCode, Error = AddressFieldError.InvalidFormat };
            var actual = invoke();
            Assert.Equal( expected, actual.Single() );
        }

        public static IEnumerable<object[]> ValidPrefixCases()
        {
            var pattern = @"(\d{5})(?:[ \-](\d{4}))?";
            var prefix = @"71[6-9]|72";
            var values = new string[]
            {
                // undefined values
                null,
                string.Empty,
                "\t",

                // values that match the prefix
                "72000",
                "71800-0000",
            };

            // use the pattern in the country
            // note: the all-null case is covered by the pattern cases

            var validInProvince =
                from value in values
                select new object[] { value, pattern, prefix, null, null };

            var validInLocality =
                from value in values
                select new object[] { value, pattern, null, prefix, null };

            var validInSublocality =
                from value in values
                select new object[] { value, pattern, null, null, prefix };

            var validInAll =
                from value in values
                select new object[] { value, pattern, prefix, prefix, prefix };

            return validInAll
                .Union( validInProvince )
                .Union( validInLocality )
                .Union( validInSublocality );
        }

        [Theory]
        [MemberData( nameof( ValidPrefixCases ) )]
        public void Validate_whenPrefixValid_returns_success( string value, string country, string province, string locality, string sublocality )
        {
            context.Address = new AddressData { PostalCode = value };
            context.CountryMetadata = country != null ? new CountryMetadata { PostalCodePattern = country } : null;
            context.ProvinceMetadata = province != null ? new ProvinceMetadata { PostalCodePattern = province } : null;
            context.LocalityMetadata = locality != null ? new LocalityMetadata { PostalCodePattern = locality } : null;
            context.SublocalityMetadata = sublocality != null ? new SublocalityMetadata { PostalCodePattern = sublocality } : null;

            var actual = invoke();
            Assert.Empty( actual );
        }

        public static IEnumerable<object[]> InvalidPrefixCases()
        {
            var pattern = @"(\d{5})(?:[ \-](\d{4}))?";
            var prefix = @"71[6-9]|72";
            var values = new string[]
            {
                // values that do not match the prefix
                "82000",
                "71500-0000",
            };

            // use the pattern in the country
            // note: the all-null case is covered by the pattern cases

            var invalidInProvince =
                from value in values
                select new object[] { value, pattern, prefix, null, null };

            var invalidInLocality =
                from value in values
                select new object[] { value, pattern, null, prefix, null };

            var invalidInSublocality =
                from value in values
                select new object[] { value, pattern, null, null, prefix };

            var invalidInAll =
                from value in values
                select new object[] { value, pattern, prefix, prefix, prefix };

            return invalidInAll
                .Union( invalidInProvince )
                .Union( invalidInLocality )
                .Union( invalidInSublocality );
        }

        [Theory]
        [MemberData( nameof( InvalidPrefixCases ) )]
        public void Validate_whenPrefixInvalid_returns_failure( string value, string country, string province, string locality, string sublocality )
        {
            context.Address = new AddressData { PostalCode = value };
            context.CountryMetadata = country != null ? new CountryMetadata { PostalCodePattern = country } : null;
            context.ProvinceMetadata = province != null ? new ProvinceMetadata { PostalCodePattern = province } : null;
            context.LocalityMetadata = locality != null ? new LocalityMetadata { PostalCodePattern = locality } : null;
            context.SublocalityMetadata = sublocality != null ? new SublocalityMetadata { PostalCodePattern = sublocality } : null;

            var expected = new AddressValidationFailure { Field = AddressField.PostalCode, Error = AddressFieldError.MismatchingValue };
            var actual = invoke();
            Assert.Equal( expected, actual.Single() );
        }
    }
}