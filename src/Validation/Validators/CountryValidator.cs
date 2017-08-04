using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Validation.Validators
{
    /// <summary>
    /// Validates the country value in an address.
    /// </summary>

    public class CountryValidator : IAddressValidator
    {
        /// <summary>
        /// Failure for the validator.
        /// </summary>
        
        static readonly AddressValidationFailure Failure = new AddressValidationFailure 
        { 
            Field = AddressField.Country,
            Error = AddressFieldError.UnkownValue,
        };

        /// <summary>
        /// Validates the address.
        /// </summary>
        /// <param name="context">Address validation context.</param>
        /// <returns>The collection of any validation errors.</returns>
        
        public IEnumerable<AddressValidationFailure> Validate( ValidationContext context )
        {
            // fail when the country is specified but the metadata is not found
            if ( !string.IsNullOrWhiteSpace( context.Address?.Country ) )
            if ( context.CountryMetadata == null )
            {
                return new AddressValidationFailure[] { Failure };
            }

            return new AddressValidationFailure[0];
        }
    }
}