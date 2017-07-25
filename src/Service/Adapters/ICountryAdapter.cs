using Fidget.Validation.Addresses.Metadata;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Service.Adapters
{
    /// <summary>
    /// Defines an adapter for interacting with country metadata.
    /// </summary>

    public interface ICountryAdapter
    {
        /// <summary>
        /// Queries the service for the specified country.
        /// </summary>
        /// <param name="country">
        /// Value to search for a country key.
        /// This can be a case-insensitive value that matches a country key.
        /// This can also be the value "ZZ" for the default country metadata.
        /// </param>
        /// <returns>The specified country, if found; otherwise null.</returns>

        Task<CountryMetadata> Query( string country, string language );

        /// <summary>
        /// Returns whether the given province value can be located in the country metadata.
        /// </summary>
        /// <param name="country">Country metadata (always returns false if not provided).</param>
        /// <param name="province">Key or name of the province (case insinsitive).</param>
        /// <param name="key">Key of the province, if found.</param>

        bool TryGetProvinceKey( CountryMetadata country, string province, out string key );
    }
}