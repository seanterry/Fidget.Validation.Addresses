﻿using Fidget.Validation.Addresses.Service.Metadata;
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
    }
}