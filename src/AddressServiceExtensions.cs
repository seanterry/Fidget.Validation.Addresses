using Fidget.Validation.Addresses.Service.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fidget.Validation.Addresses
{
    /// <summary>
    /// Extension methods related to the address service.
    /// </summary>
    
    public static class AddressServiceExtensions
    {
        /// <summary>
        /// Returns gobal metadata information.
        /// </summary>
        /// <param name="service">Address service.</param>
        
        public static IGlobalMetadata GetGlobal( this IAddressService service )
        {
            if ( service == null ) throw new ArgumentNullException( nameof( service ) );

            return service
                .GetGlobalAsync()
                .Result;
        }

        /// <summary>
        /// Returns metadata for the specified country.
        /// </summary>
        /// <param name="countryKey">Key of the country to return. Use "ZZ" for the default country metadata.</param>
        /// <param name="language">
        /// (Optional) Language code for the metadata to return.
        /// If metadata is not available for the language, no result will be returned.
        /// </param>
        
        public static ICountryMetadata GetCountry( this IAddressService service, string countryKey, string language = null )
        {
            if ( service == null ) throw new ArgumentNullException( nameof( service ) );
            if ( countryKey == null ) throw new ArgumentNullException( nameof( countryKey ) );

            return service
                .GetCountryAsync( countryKey, language )
                .Result;
        }

        /// <summary>
        /// Returns metadata for the specified province if it is available.
        /// </summary>
        /// <param name="countryKey">Key of the parent country.</param>
        /// <param name="provinceKey">Key of the province to return.</param>
        /// <param name="language">
        /// (Optional) Language code for the metadata to return.
        /// If metadata is not available for the language, no result will be returned.
        /// </param>
        
        public static IProvinceMetadata GetProvince( this IAddressService service, string countryKey, string provinceKey, string language = null )
        {
            if ( service == null ) throw new ArgumentNullException( nameof( service ) );
            if ( countryKey == null ) throw new ArgumentNullException( nameof( countryKey ) );
            if ( provinceKey == null ) throw new ArgumentNullException( nameof( provinceKey ) );

            return service
                .GetProvinceAsync( countryKey, provinceKey, language )
                .Result;
        }
    }
}