using Fidget.Validation.Addresses.Metadata;
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

        /// <summary>
        /// Returns metadata for the specified province.
        /// </summary>
        /// <param name="country">Key of the containing country.</param>
        /// <param name="province">Key or name of the province.</param>
        /// <param name="language">Requested language of the metadata.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Metadata for the specified province if found, otherwise null.</returns>

        Task<ProvinceMetadata> GetProvinceAsync( string country, string province, string language = null, CancellationToken cancellationToken = default(CancellationToken) );

        /// <summary>
        /// Returns metadata for the specified locality.
        /// </summary>
        /// <param name="country">Key of the containing country.</param>
        /// <param name="province">Key or name of the containing province.</param>
        /// <param name="locality">Key or name of the locality.</param>
        /// <param name="language">Requested language of the metadata.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Metadata for the specified locality if found, otherwise null.</returns>

        Task<LocalityMetadata> GetLocalityAsync( string country, string province, string locality, string language = null, CancellationToken cancellationToken = default(CancellationToken) );

        /// <summary>
        /// Returns metadata for the specified sublocality.
        /// </summary>
        /// <param name="country">Key of the containing country.</param>
        /// <param name="province">Key or name of the containing province.</param>
        /// <param name="locality">Key or name of the containing locality.</param>
        /// <param name="sublocality">Key or name of the sublocality.</param>
        /// <param name="language">Requested language of the metadata.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Metadata for the specified sublocality if found, otherwise null.</returns>

        Task<SublocalityMetadata> GetSublocalityAsync( string country, string province, string locality, string sublocality, string language = null, CancellationToken cancellationToken = default(CancellationToken) );
    }
}