using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Defines a delegate that represents a continuation of validation execution.
    /// </summary>
    /// <param name="address">Address to validate.</param>
    /// <param name="context">Validation context.</param>
    /// <returns>The collection of validation errors, if any.</returns>

    public delegate IEnumerable<ValidationFailure> ValidationDelegate( AddressData address, IValidationContext context );
}