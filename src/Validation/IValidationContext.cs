using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Defines a context containing validation metadata.
    /// </summary>

    public interface IValidationContext
    {
        /// <summary>
        /// Returns the elements that are required to be valid.
        /// </summary>

        IEnumerable<AddressField> GetRequiredFields();

        /// <summary>
        /// Returns the elements that are allowed in a valid address for the country.
        /// </summary>

        IEnumerable<AddressField> GetAllowedFields();
    }
}