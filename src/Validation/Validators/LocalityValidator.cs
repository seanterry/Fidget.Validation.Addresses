using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Validation.Validators
{
    /// <summary>
    /// Validates the locality value in an address.
    /// </summary>

    public class LocalityValidator : RegionValidator, IAddressValidator
    {
        /// <summary>
        /// Failure for the validator.
        /// </summary>

        static readonly AddressValidationFailure Failure = new AddressValidationFailure
        {
            Field = AddressField.Locality,
            Error = AddressFieldError.UnkownValue,
        };

        /// <summary>
        /// Validates the address.
        /// </summary>
        /// <param name="context">Address validation context.</param>
        /// <returns>The collection of any validation errors.</returns>

        public IEnumerable<AddressValidationFailure> Validate( ValidationContext context ) =>
            IsValid( context.Address?.Locality, context.ProvinceMetadata, context.LocalityMetadata )
                ? new AddressValidationFailure[0]
                : new AddressValidationFailure[] { Failure };
    }
}