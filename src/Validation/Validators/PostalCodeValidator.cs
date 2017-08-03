using Fidget.Validation.Addresses.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Fidget.Validation.Addresses.Validation.Validators
{
    /// <summary>
    /// Validates the postal code field in an address.
    /// </summary>
    
    public class PostalCodeValidator : IAddressValidator
    {
        /// <summary>
        /// Error representing an invlid format.
        /// </summary>
        
        static readonly AddressValidationFailure InvalidFormat = new AddressValidationFailure { Field = AddressField.PostalCode, Error = AddressFieldError.InvalidFormat };

        /// <summary>
        /// Error representing a mismatched value.
        /// </summary>
        
        static readonly AddressValidationFailure MismatchingValue = new AddressValidationFailure { Field = AddressField.PostalCode, Error = AddressFieldError.MismatchingValue };
        
        /// <summary>
        /// Validates the address.
        /// </summary>
        /// <param name="context">Address validation context.</param>
        /// <returns>The collection of any validation errors.</returns>
        
        public IEnumerable<AddressValidationFailure> Validate( ValidationContext context )
        {
            var value = context.Address?.PostalCode;
            var pattern = 
                context.SublocalityMetadata?.PostalCodePatternOverride ??
                context.LocalityMetadata?.PostalCodePatternOverride ??
                context.ProvinceMetadata?.PostalCodePatternOverride ??
                context.CountryMetadata?.PostalCodePattern;

            var prefixes = new string[]
            {
                context.SublocalityMetadata?.PostalCodePattern,
                context.LocalityMetadata?.PostalCodePattern,
                context.ProvinceMetadata.PostalCodePattern,
            };

            if ( !string.IsNullOrWhiteSpace( value ) )
            {
                if ( pattern is string && !Regex.IsMatch( value, $"^{pattern}$" ) )
                {
                    return new AddressValidationFailure[] { InvalidFormat };
                }

                foreach ( var prefix in prefixes )
                {
                    if ( prefix is string && !Regex.IsMatch( value, $"^({prefix})" ) )
                    {
                        return new AddressValidationFailure[] { MismatchingValue };
                    }
                }
            }

            return new AddressValidationFailure[0];
        }
    }
}