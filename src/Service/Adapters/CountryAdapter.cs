using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Service.Adapters
{
    /// <summary>
    /// Adapter for interacting with country metadata.
    /// </summary>
    
    class CountryAdapter : ICountryAdapter
    {
        /// <summary>
        /// Service client that backs the current instance.
        /// </summary>
        
        IServiceClient Client;

        /// <summary>
        /// Global metadata adapter.
        /// </summary>
        
        readonly IGlobalAdapter Global;

        /// <summary>
        /// Constructs an adapter for interacting with country metadata.
        /// </summary>
        /// <param name="client">Service client that will back the instance.</param>
        /// <param name="global">Global metadata adapter.</param>
        
        public CountryAdapter( IServiceClient client, IGlobalAdapter global )
        {
            Client = client ?? throw new ArgumentNullException( nameof(client) );
            Global = global ?? throw new ArgumentNullException( nameof(global) );
        }

        /// <summary>
        /// Returns the identifier for the record.
        /// </summary>
        /// <param name="parent">Parent region key.</param>
        /// <param name="key">Key of the region to identify.</param>
        /// <param name="language">Language for the metadata.</param>
        
        protected string BuildIdentifier( string parent, string key, string language )
        {
            // remove language component of parent if present
            parent = parent.Contains( "--" )
                ? parent.Remove( parent.IndexOf( "--" ) )
                : parent;

            return $"{parent}/{key}{(language != null ? $"--{language}" : string.Empty)}";
        }

        /// <summary>
        /// Queries the service for the specified country.
        /// </summary>
        /// <param name="country">
        /// Value to search for a country key.
        /// This can be a case-insensitive value that matches a country key.
        /// This can also be the value "ZZ" for the default country metadata.
        /// </param>
        /// <returns>The specified country, if found; otherwise null.</returns>

        public async Task<ICountryMetadata> Query( string country, string language )
        {
            var defaultMeta = await Client.Query<CountryMetadata>( "data/ZZ" );

            // handle requests for the default metadata
            if ( country == "ZZ" ) return defaultMeta;

            // short-circuit if the key isn't found in the global data
            var globalMeta = await Global.Query();
            if ( !Global.TryGetCountryKey( globalMeta, country, out string key ) ) return null;
        
            var id = BuildIdentifier( globalMeta.Id, key, language );
            var countryMeta = await Client.Query<CountryMetadata>( id );
            
            // coalesce default values
            if ( countryMeta != null && defaultMeta != null )
            {
                countryMeta.Format = countryMeta.Format ?? defaultMeta.Format;
                countryMeta.Required = countryMeta.Required ?? defaultMeta.Required;
                countryMeta.Uppercase = countryMeta.Uppercase ?? defaultMeta.Uppercase;
                countryMeta.StateType = countryMeta.StateType ?? defaultMeta.StateType;
                countryMeta.LocalityType = countryMeta.LocalityType ?? defaultMeta.LocalityType;
                countryMeta.SublocalityType = countryMeta.SublocalityType ?? defaultMeta.SublocalityType;
                countryMeta.PostalCodeType = countryMeta.PostalCodeType ?? defaultMeta.PostalCodeType;
            }

            return countryMeta;
        }

        /// <summary>
        /// Returns whether the given region contains the specified child region.
        /// </summary>
        /// <param name="parent">Region whose children to search.</param>
        /// <param name="child">Key or name of the child region (case insinsitive).</param>
        /// <param name="key">Key value of the child region, if found.</param>
        
        protected bool TryGetChildKey( IHierarchicalMetadata parent, string child, out string key )
        {
            var keys = parent?.ChildRegionKeys;

            int? getKeyIndex( params IEnumerable<string>[] collections )
            {
                foreach ( var collection in collections )
                {
                    if ( collection.IndexOf( child, out int index, StringComparer.OrdinalIgnoreCase ) )
                    {
                        return index;
                    }
                }

                return null;
            }

            var keyIndex = getKeyIndex( parent?.ChildRegionKeys, parent?.ChildRegionNames, parent?.ChildRegionLatinNames );

            if ( keys != null && keyIndex.HasValue )
            {
                key = keys.ElementAtOrDefault( keyIndex.Value );
                return true;
            }

            else
            {
                key = null;
                return false;
            }
        }

        /// <summary>
        /// Returns whether the given province value can be located in the country metadata.
        /// </summary>
        /// <param name="country">Country metadata (always returns false if not provided).</param>
        /// <param name="province">Key or name of the province (case insinsitive).</param>
        /// <param name="key">Key of the province, if found.</param>

        public bool TryGetProvinceKey( ICountryMetadata country, string province, out string key ) =>
            TryGetChildKey( country, province, out key );
    }
}