using Fidget.Validation.Addresses.Metadata;
using Fidget.Validation.Addresses.Validation;
using System;
using System.Collections.Generic;

namespace Fidget.Validation.Addresses
{
    /// <summary>
    /// Extension methods related to the address service.
    /// </summary>

    public static class AddressServiceExtensions
    {
        /// <summary>
        /// Returns whether the given value is in the collection and outputs its index.
        /// </summary>
        /// <typeparam name="T">Type of the items in the collection.</typeparam>
        /// <param name="source">Source collection to search.</param>
        /// <param name="value">Value to locate in the collection.</param>
        /// <param name="index">Index of the located item.</param>
        /// <param name="comparer">Equality comparer.</param>
        
        internal static bool IndexOf<T>( this IEnumerable<T> source, T value, out int index, IEqualityComparer<T> comparer = null )
        {
            comparer = comparer ?? EqualityComparer<T>.Default;
            index = 0;

            if ( source != null )
            {
                foreach ( var item in source )
                {
                    if ( comparer.Equals( item, value ) ) return true;
                    index++;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Returns gobal metadata information.
        /// </summary>
        /// <param name="service">Address service.</param>
        
        public static GlobalMetadata GetGlobal( this IAddressService service )
        {
            if ( service == null ) throw new ArgumentNullException( nameof( service ) );

            return service
                .GetGlobalAsync()
                .Result;
        }

        /// <summary>
        /// Returns metadata for the specified country.
        /// </summary>
        /// <param name="service">Address service.</param>
        /// <param name="countryKey">Key of the country to return. Use "ZZ" for the default country metadata.</param>
        /// <param name="language">
        /// (Optional) Language code for the metadata to return.
        /// If metadata is not available for the language, no result will be returned.
        /// </param>

        public static CountryMetadata GetCountry( this IAddressService service, string countryKey, string language = null )
        {
            if ( service == null ) throw new ArgumentNullException( nameof( service ) );
            
            return service
                .GetCountryAsync( countryKey, language )
                .Result;
        }

        /// <summary>
        /// Returns metadata for the specified province if it is available.
        /// </summary>
        /// <param name="service">Address service.</param>
        /// <param name="countryKey">Key of the parent country.</param>
        /// <param name="province">Key or name of the province to return.</param>
        /// <param name="language">
        /// (Optional) Language code for the metadata to return.
        /// If metadata is not available for the language, no result will be returned.
        /// </param>

        public static ProvinceMetadata GetProvince( this IAddressService service, string countryKey, string province, string language = null )
        {
            if ( service == null ) throw new ArgumentNullException( nameof( service ) );
            
            return service
                .GetProvinceAsync( countryKey, province, language )
                .Result;
        }

        /// <summary>
        /// Returns metadata for the specified locality if it is available.
        /// </summary>
        /// <param name="service">Address service.</param>
        /// <param name="countryKey">Key of the parent country.</param>
        /// <param name="provinceKey">Key of the parent province.</param>
        /// <param name="localityKey">Key of the locality to return.</param>
        /// <param name="language">
        /// (Optional) Language code for the metadata to return.
        /// If metadata is not available for the language, no result will be returned.
        /// </param>

        public static LocalityMetadata GetLocality( this IAddressService service, string countryKey, string provinceKey, string localityKey, string language = null )
        {
            if ( service == null ) throw new ArgumentNullException( nameof( service ) );
            if ( countryKey == null ) throw new ArgumentNullException( nameof( countryKey ) );
            if ( provinceKey == null ) throw new ArgumentNullException( nameof( provinceKey ) );
            if ( localityKey == null ) throw new ArgumentNullException( nameof( localityKey ) );

            return service
                .GetLocalityAsync( countryKey, provinceKey, localityKey, language )
                .Result;
        }

        /// <summary>
        /// Returns metadata for the specified sublocality if it is available.
        /// </summary>
        /// <param name="service">Address service.</param>
        /// <param name="countryKey">Key of the parent country.</param>
        /// <param name="provinceKey">Key of the parent province.</param>
        /// <param name="localityKey">Key of the parent locality.</param>
        /// <param name="sublocalityKey">Key of the sublocality to return.</param>
        /// <param name="language">
        /// (Optional) Language code for the metadata to return.
        /// If metadata is not available for the language, no result will be returned.
        /// </param>

        public static SublocalityMetadata GetSublocality( this IAddressService service, string countryKey, string provinceKey, string localityKey, string sublocalityKey, string language = null )
        {
            if ( service == null ) throw new ArgumentNullException( nameof( service ) );
            if ( countryKey == null ) throw new ArgumentNullException( nameof( countryKey ) );
            if ( provinceKey == null ) throw new ArgumentNullException( nameof( provinceKey ) );
            if ( localityKey == null ) throw new ArgumentNullException( nameof( localityKey ) );
            if ( sublocalityKey == null ) throw new ArgumentNullException( nameof( sublocalityKey ) );

            return service
                .GetSublocalityAsync( countryKey, provinceKey, localityKey, sublocalityKey, language )
                .Result;
        }

        /// <summary>
        /// Validates the given address.
        /// </summary>
        /// <param name="service">Address service.</param>
        /// <param name="address">Address to validate.</param>
        /// <returns>The collection of validation errors, if any.</returns>
        
        public static IEnumerable<ValidationFailure> Validate( this IAddressService service, AddressData address )
        {
            if ( service == null ) throw new ArgumentNullException( nameof( service ) );
            if ( address == null ) throw new ArgumentNullException( nameof( address ) );

            return service
                .ValidateAsync( address )
                .Result;
        }
    }
}