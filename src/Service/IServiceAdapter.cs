using Fidget.Validation.Addresses.Metadata;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Service
{
    /// <summary>
    /// Defines an adapter for interacting with the remote address service.
    /// </summary>

    public interface IServiceAdapter
    {
        /// <summary>
        /// Returns global-level address metadata.
        /// </summary>

        Task<GlobalMetadata> GetGlobal();

        /// <summary>
        /// Returns metadata for the specified country if it is available.
        /// </summary>
        /// <param name="global">Global metadata.</param>
        /// <param name="country">Identifier of the country to return.</param>
        /// <param name="language">Language code for the metadata to return.</param>

        Task<CountryMetadata> GetCountry( string country, string language );

        /// <summary>
        /// Returns metadata for the specified province if it is available.
        /// </summary>
        /// <param name="country">Parent country of the province.</param>
        /// <param name="province">Province key or name.</param>
        /// <param name="language">Language code for the metadata to return.</param>

        Task<ProvinceMetadata> GetProvince( CountryMetadata country, string province, string language );
    }
}