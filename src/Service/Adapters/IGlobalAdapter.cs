using Fidget.Validation.Addresses.Service.Metadata;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Service.Adapters
{
    /// <summary>
    /// Defines an adapter for interacting with global metadata.
    /// </summary>

    public interface IGlobalAdapter
    {
        /// <summary>
        /// Returns global metadata.
        /// </summary>

        Task<IGlobalMetadata> Query();

        /// <summary>
        /// Returns whether the given country value can be located in the global metadata.
        /// </summary>
        /// <param name="global">Global metadata (always returns false if not provided).</param>
        /// <param name="country">
        /// Value to search for a country key.
        /// This can be a case-insensitive value that matches a country key.
        /// </param>
        /// <param name="key">Key of the country, if found.</param>

        bool TryGetCountryKey( IGlobalMetadata global, string country, out string key );
    }
}