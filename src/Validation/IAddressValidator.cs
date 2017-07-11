using Fidget.Validation.Addresses.Service.Metadata;
using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Defines a means of validating an address against regional metadata.
    /// </summary>

    public interface IAddressValidator
    {
        /// <summary>
        /// Validates the given address against regional metadata.
        /// </summary>
        /// <param name="address">Address to validate.</param>
        /// <param name="global">Global metadata.</param>
        /// <param name="country">Country metadata.</param>
        /// <param name="province">Province metadata.</param>
        /// <param name="locality">Locality metadata.</param>
        /// <param name="sublocality">Sublocality metadata.</param>
        /// <returns>The collection of validation errors, if any.</returns>

        IEnumerable<ValidationFailure> Validate( AddressData address, IGlobalMetadata global, ICountryMetadata country, IProvinceMetadata province, ILocalityMetadata locality, ISublocalityMetadata sublocality );
    }
}