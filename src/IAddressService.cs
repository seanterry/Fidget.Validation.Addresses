using Fidget.Validation.Addresses.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses
{
    /// <summary>
    /// Defines a service for address validation and metadata exploration.
    /// </summary>
    
    public interface IAddressService
    {
        /// <summary>
        /// Returns gobal metadata information.
        /// </summary>
        /// <param name="cancellationToken">Not used.</param>

        Task<GlobalMetadata> GetGlobalAsync( CancellationToken cancellationToken = default(CancellationToken) );

        /// <summary>
        /// Returns metadata for the specified country.
        /// </summary>
        /// <param name="country">Key of the country.</param>
        /// <param name="language">Requested language of the metadata.</param>
        /// <param name="cancellationToken">Not used.</param>
        /// <returns>Metadata for the specified country if found, otherwise null.</returns>

        Task<CountryMetadata> GetCountryAsync( string country, string language = null, CancellationToken cancellationToken = default(CancellationToken) );
    }
}