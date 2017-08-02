using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Defines a validator for an address.
    /// </summary>

    public interface IAddressValidator
    {
        /// <summary>
        /// Validates the address.
        /// </summary>
        /// <param name="context">Address validation context.</param>
        /// <returns>The collection of any validation errors.</returns>

        IEnumerable<AddressValidationFailure> Validate( ValidationContext context );
    }
}