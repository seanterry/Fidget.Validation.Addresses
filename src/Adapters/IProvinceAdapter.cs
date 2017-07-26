using Fidget.Validation.Addresses.Metadata;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Adapters
{
    /// <summary>
    /// Defines an adapter for querying province metadata.
    /// </summary>

    public interface IProvinceAdapter
    {
        /// <summary>
        /// Returns the metadata for the specified province if found, otherwise null.
        /// </summary>
        /// <param name="country">Key of the country containing the province.</param>
        /// <param name="province">Key or name of the province.</param>
        /// <param name="language">Metadata language (optional).</param>

        Task<ProvinceMetadata> QueryAsync( string country, string province, string language = null );
    }
}