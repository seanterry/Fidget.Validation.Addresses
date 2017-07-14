﻿using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Defines a means of validating an address against regional metadata.
    /// </summary>

    public interface IAddressValidator
    {
        /// <summary>
        /// Validates the given address.
        /// </summary>
        /// <param name="address">Address to validate.</param>
        /// <param name="context">Validation context.</param>
        /// <param name="next">The next validator in the sequence.</param>
        /// <returns>The collection of validation errors, if any.</returns>
        
        IEnumerable<ValidationFailure> Validate( AddressData address, IValidationContext context );
    }
}