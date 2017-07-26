using Fidget.Validation.Addresses.Metadata;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Adapters
{
    /// <summary>
    /// Defines an adatper for querying country metadata.
    /// </summary>

    public interface ICountryAdapter
    {
        /// <summary>
        /// Returns the default country metadata values.
        /// </summary>

        Task<CountryMetadata> QueryDefaultAsync();

        /// <summary>
        /// Queries the service for the specified country.
        /// </summary>
        /// <param name="country">Key of the country to return (case insensitive).</param>
        /// <param name="language">Language of the metadata to return (optional).</param>
        /// <returns>The specified country if it exists, otherwise null.</returns>

        Task<CountryMetadata> QueryAsync( string country, string language = null );
    }
}