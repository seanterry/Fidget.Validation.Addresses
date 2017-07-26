using Fidget.Validation.Addresses.Adapters;
using Fidget.Validation.Addresses.Metadata;
using System;

namespace Fidget.Validation.Addresses
{
    /// <summary>
    /// Extension methods related to adapters.
    /// </summary>

    public static class AdapterExtensions
    {
        /// <summary>
        /// Returns the global metadata entry.
        /// </summary>
        /// <param name="adapter">Global service adapter.</param>
        
        public static GlobalMetadata Query( this IGlobalAdapter adapter )
        {
            if ( adapter == null ) throw new ArgumentNullException( nameof( adapter ) );

            return adapter.QueryAsync().Result;
        }

        /// <summary>
        /// Returns the default country metadata.
        /// </summary>
        /// <param name="adapter">Country service adapter.</param>
        
        public static CountryMetadata QueryDefault( this ICountryAdapter adapter )
        {
            if ( adapter == null ) throw new ArgumentNullException( nameof( adapter ) );

            return adapter.QueryDefaultAsync().Result;
        }

        /// <summary>
        /// Returns the metadata for the specified country.
        /// </summary>
        /// <param name="adapter">Country service adapter.</param>
        /// <param name="country">Key of the country.</param>
        /// <param name="language">Language of the metadata.</param>
        
        public static CountryMetadata Query( this ICountryAdapter adapter, string country, string language = null )
        {
            if ( adapter == null ) throw new ArgumentNullException( nameof( adapter ) );

            return adapter.QueryAsync( country, language ).Result;
        }

        /// <summary>
        /// Returns the metadata for the specified province.
        /// </summary>
        /// <param name="adapter">Province service adapter.</param>
        /// <param name="country">Key of the country.</param>
        /// <param name="province">Key or name of the province.</param>
        /// <param name="language">Language of the metadata.</param>
        
        public static ProvinceMetadata Query( this IProvinceAdapter adapter, string country, string province, string language = null )
        {
            if ( adapter == null ) throw new ArgumentNullException( nameof( adapter ) );

            return adapter.QueryAsync( country, province, language ).Result;
        }
    }
}