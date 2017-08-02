using System.Collections.Generic;
using System.Linq;

namespace Fidget.Validation.Addresses.Validation.Validators
{
    /// <summary>
    /// Validates the province value in an address.
    /// </summary>

    public class ProvinceValidator : RegionValidator, IAddressValidator
    {
        /// <summary>
        /// Failure for the validator.
        /// </summary>

        static readonly AddressValidationFailure Failure = new AddressValidationFailure
        {
            Field = AddressField.Province,
            Error = AddressFieldError.UnkownValue,
        };

        /// <summary>
        /// Validates the address.
        /// </summary>
        /// <param name="context">Address validation context.</param>
        /// <returns>The collection of any validation errors.</returns>

        public IEnumerable<AddressValidationFailure> Validate( ValidationContext context ) =>
            IsValid( context.Address?.Province, context.CountryMetadata, context.ProvinceMetadata )
                ? new AddressValidationFailure[0]
                : new AddressValidationFailure[] { Failure };
    }
}
