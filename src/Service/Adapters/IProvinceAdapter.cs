using Fidget.Validation.Addresses.Service.Metadata;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Service.Adapters
{
    /// <summary>
    /// Defines an adapter for interacting with province metadata.
    /// </summary>

    public interface IProvinceAdapter
    {
        /// <summary>
        /// Queries the service for the specified province.
        /// </summary>
        /// <param name="country"> Key of the country.</param>
        /// <param name="province">Key or name of the province.</param>
        /// <param name="language">Language for the metadata.</param>
        /// <returns>The specified province, if found; otherwise null.</returns>

        Task<IProvinceMetadata> Query( string country, string province, string language );

        /// <summary>
        /// Returns whether the given locality value can be located in the province metadata.
        /// </summary>
        /// <param name="province">Province metadata (always returns false if not provided).</param>
        /// <param name="locality">Key or name of the locality (case insinsitive).</param>
        /// <param name="key">Key of the locality, if found.</param>

        bool TryGetLocalityKey( IProvinceMetadata province, string locality, out string key );
    }
}