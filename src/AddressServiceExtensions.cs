using Fidget.Validation.Addresses.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Fidget.Validation.Addresses
{
    /// <summary>
    /// Extension methods related to the address service.
    /// </summary>
    
    public static class AddressServiceExtensions
    {
        /// <summary>
        /// Returns global metadata information.
        /// </summary>
        /// <param name="service">Remote address service.</param>
        
        public static GlobalMetadata GetGlobal( this IAddressService service )
        {
            if ( service == null ) throw new ArgumentNullException( nameof( service ) );

            return service.GetGlobalAsync( CancellationToken.None ).Result;
        }

        /// <summary>
        /// Returns metadata for the specified country.
        /// </summary>
        /// <param name="service">Remote address service.</param>
        /// <param name="country">Key of the country.</param>
        /// <param name="language">Requested language of the metadata.</param>
        /// <returns>Metadata for the specified country if found, otherwise null.</returns>
        
        public static CountryMetadata GetCountry( this IAddressService service, string country, string language = null )
        {
            if ( service == null ) throw new ArgumentNullException( nameof( service ) );

            return service.GetCountryAsync( country, language, CancellationToken.None ).Result;
        }

        /// <summary>
        /// Returns metadata for the specified province.
        /// </summary>
        /// <param name="service">Remote address service.</param>
        /// <param name="country">Key of the containing country.</param>
        /// <param name="province">Key or name of the province.</param>
        /// <param name="language">Requested language of the metadata.</param>
        /// <returns>Metadata for the specified province if found, otherwise null.</returns>
        
        public static ProvinceMetadata GetProvince( this IAddressService service, string country, string province, string language = null )
        {
            if ( service == null ) throw new ArgumentNullException( nameof( service ) );

            return service.GetProvinceAsync( country, province, language, CancellationToken.None ).Result;
        }

        /// <summary>
        /// Returns metadata for the specified locality.
        /// </summary>
        /// <param name="service">Remote address service.</param>
        /// <param name="country">Key of the containing country.</param>
        /// <param name="province">Key or name of the containing province.</param>
        /// <param name="locality">Key or name of the locality.</param>
        /// <param name="language">Requested language of the metadata.</param>
        /// <returns>Metadata for the specified locality if found, otherwise null.</returns>

        public static LocalityMetadata GetLocality( this IAddressService service, string country, string province, string locality, string language = null )
        {
            if ( service == null ) throw new ArgumentNullException( nameof( service ) );

            return service.GetLocalityAsync( country, province, locality, language, CancellationToken.None ).Result;
        }

        /// <summary>
        /// Returns metadata for the specified sublocality.
        /// </summary>
        /// <param name="service">Remote address service.</param>
        /// <param name="country">Key of the containing country.</param>
        /// <param name="province">Key or name of the containing province.</param>
        /// <param name="locality">Key or name of the containing locality.</param>
        /// <param name="sublocality">Key or name of the sublocality.</param>
        /// <param name="language">Requested language of the metadata.</param>
        /// <returns>Metadata for the specified sublocality if found, otherwise null.</returns>
        
        public static SublocalityMetadata GetSublocality( this IAddressService service, string country, string province, string locality, string sublocality, string language = null )
        {
            if ( service == null ) throw new ArgumentNullException( nameof( service ) );

            return service.GetSublocalityAsync( country, province, locality, sublocality, language, CancellationToken.None ).Result;
        }
    }
}