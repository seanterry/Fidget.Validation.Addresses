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
        /// Cases where both pattern and prefix match.
        /// </summary>
        
        public static IEnumerable<object[]> ValidCases()
        {
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

            return
                from value in values
                select new object[] { value, @"(\d{5})(?:[ \-](\d{4}))?", @"71[6-9]|72" };
        }

        /// <summary>
        /// Cases where neither pattern nor prefix match.
        /// </summary>

        public static IEnumerable<object[]> InvalidCases()
        {
            var values = new string[]
            {
                "6200",
                "42000-000",
            };

            return
                from value in values
                select new object[] { value, @"(\d{5})(?:[ \-](\d{4}))?", @"71[6-9]|72" };
        }

        /// <summary>
        /// Cases where the pattern does not match, but the prefix does.
        /// </summary>

        public static IEnumerable<object[]> InvalidPatternCases()
        {
            var values = new string[]
            {
                "7200",
                "7200-0000",
                "ABCDE",
                "ABCDE-FGHI",
            };

            return
                from value in values
                select new object[] { value, @"(\d{5})(?:[ \-](\d{4}))?", @"[7A]" };
        }

        /// <summary>
        /// Cases where the pattern matches, but the prefix does not.
        /// </summary>
        
        public static IEnumerable<object[]> InvalidPrefixCases()
        {
            var values = new string[]
            {
                "72000",
                "72000-0001",
            };

            return
                from value in values
                select new object[] { value, @"(\d{5})(?:[ \-](\d{4}))?", @"[6B]" };
        }

        /// <summary>
        /// Simplest validation at country/province level.
        /// </summary>
        
        [Theory]
        [MemberData(nameof(ValidCases))]
        public void Validate_whenValid_returns_success( string value, string pattern, string prefix )
        {
            context.Address = new AddressData { PostalCode = value };
            context.CountryMetadata = new CountryMetadata { PostalCodePattern = pattern };
            context.ProvinceMetadata = new ProvinceMetadata { PostalCodePattern = prefix };

            var actual = invoke();
            Assert.Empty( actual );
        }

        /// <summary>
        /// In cases where neither the pattern nor prefix match, the pattern error should be returned.
        /// </summary>
        
        [Theory]
        [MemberData(nameof(InvalidCases))]
        public void Validate_whenBothInvalid_returnsPatternInvalid( string value, string pattern, string prefix )
        {
            context.Address = new AddressData { PostalCode = value };
            context.CountryMetadata = new CountryMetadata { PostalCodePattern = pattern };
            context.ProvinceMetadata = new ProvinceMetadata { PostalCodePattern = prefix };

            var expected = new AddressValidationFailure { Field = AddressField.PostalCode, Error = AddressFieldError.InvalidFormat };
            var actual = invoke();

            Assert.Equal( expected, actual.Single() );
        }

        /// <summary>
        /// Sublocality pattern overrides all higher regions.
        /// </summary>
        
        [Theory]
        [MemberData(nameof(ValidCases))]
        public void Validate_whenValidAtSublocality_returns_success( string value, string pattern, string prefix )
        {
            var random = Convert.ToBase64String( Guid.NewGuid().ToByteArray() );
            context.Address = new AddressData { PostalCode = value };
            context.CountryMetadata = new CountryMetadata { PostalCodePattern = random };
            context.ProvinceMetadata = new ProvinceMetadata { PostalCodePatternOverride = random };
            context.LocalityMetadata = new LocalityMetadata { PostalCodePatternOverride = random };
            context.SublocalityMetadata = new SublocalityMetadata { PostalCodePatternOverride = pattern };

            var actual = invoke();
            Assert.Empty( actual );
        }

        /// <summary>
        /// Locality pattern overrides all higher regions.
        /// </summary>

        [Theory]
        [MemberData( nameof( ValidCases ) )]
        public void Validate_whenValidAtLocality_returns_success( string value, string pattern, string prefix )
        {
            var random = Convert.ToBase64String( Guid.NewGuid().ToByteArray() );
            context.Address = new AddressData { PostalCode = value };
            context.CountryMetadata = new CountryMetadata { PostalCodePattern = random };
            context.ProvinceMetadata = new ProvinceMetadata { PostalCodePatternOverride = random };
            context.LocalityMetadata = new LocalityMetadata { PostalCodePatternOverride = pattern };
            
            var actual = invoke();
            Assert.Empty( actual );
        }
    }
} 