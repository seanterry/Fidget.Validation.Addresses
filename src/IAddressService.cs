using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Validation;
using System.Collections.Generic;
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

        Task<IGlobalMetadata> GetGlobalAsync();

        /// <summary>
        /// Returns metadata for the specified country if it is available.
        /// </summary>
        /// <param name="countryKey">Key of the country to return. Use "ZZ" for the default country metadata.</param>
        /// <param name="language">
        /// (Optional) Language code for the metadata to return.
        /// If metadata is not available for the language, no result will be returned.
        /// </param>

        Task<ICountryMetadata> GetCountryAsync( string countryKey, string language = null );

        /// <summary>
        /// Returns metadata for the specified province if it is available.
        /// </summary>
        /// <param name="countryKey">Key of the parent country.</param>
        /// <param name="provinceKey">Key of the province to return.</param>
        /// <param name="language">
        /// (Optional) Language code for the metadata to return.
        /// If metadata is not available for the language, no result will be returned.
        /// </param>

        Task<IProvinceMetadata> GetProvinceAsync( string countryKey, string provinceKey, string language = null );

        /// <summary>
        /// Returns metadata for the specified locality if it is available.
        /// </summary>
        /// <param name="countryKey">Key of the parent country.</param>
        /// <param name="provinceKey">Key of the parent province.</param>
        /// <param name="localityKey">Key of the locality to return.</param>
        /// <param name="language">
        /// (Optional) Language code for the metadata to return.
        /// If metadata is not available for the language, no result will be returned.
        /// </param>

        Task<ILocalityMetadata> GetLocalityAsync( string countryKey, string provinceKey, string localityKey, string language = null );

        /// <summary>
        /// Returns metadata for the specified sublocality if it is available.
        /// </summary>
        /// <param name="countryKey">Key of the parent country.</param>
        /// <param name="provinceKey">Key of the parent province.</param>
        /// <param name="localityKey">Key of the parent locality.</param>
        /// <param name="sublocalityKey">Key of the sublocality to return.</param>
        /// <param name="language">
        /// (Optional) Language code for the metadata to return.
        /// If metadata is not available for the language, no result will be returned.
        /// </param>

        Task<ISublocalityMetadata> GetSublocalityAsync( string countryKey, string provinceKey, string localityKey, string sublocalityKey, string language = null );

        /// <summary>
        /// Validates the given address.
        /// </summary>
        /// <param name="address">Address to validate.</param>
        /// <returns>The collection of validation errors, if any.</returns>

        Task<IEnumerable<ValidationFailure>> ValidateAsync( AddressData address );
    }
}