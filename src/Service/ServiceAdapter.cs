using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Service
{
    /// <summary>
    /// Adapter for interacting with the remote address service.
    /// </summary>

    class ServiceAdapter : IServiceAdapter
    {
        /// <summary>
        /// Client that backs the current instance.
        /// </summary>
        
        readonly IServiceClient Client;

        /// <summary>
        /// Constructs an adapter for interacting with the remote address service.
        /// </summary>
        /// <param name="client">Service client that will back the adapter.</param>
        
        public ServiceAdapter( IServiceClient client )
        {
            Client = client ?? throw new ArgumentNullException( nameof(client) );
        }

        /// <summary>
        /// Builds and returns the data service identifier based on the given keys and language.
        /// </summary>
        /// <param name="language">Language to encode into the identifier.</param>
        /// <param name="keys">Keys for the identifier.</param>

        [Obsolete]
        string BuildIdentifier( string language, ICommonMetadata parent, string key ) => $"{parent.Id}/{key}{(language != null ? $"--{language}" : string.Empty)}";

        /// <summary>
        /// Builds and returns the data service identifier based on the given keys and language.
        /// </summary>
        /// <param name="language">Language to encode into the identifier.</param>
        /// <param name="keys">Keys for the identifier.</param>

        string BuildIdentifier( string language, params string[] keys ) => $"data/{string.Join( "/", keys )}{(language != null ? $"--{language}" : string.Empty)}";
        
        /// <summary>
        /// Returns global-level address metadata.
        /// </summary>

        public async Task<IGlobalMetadata> GetGlobal() => await Client.Query<GlobalMetadata>( "data" );


        /// <summary>
        /// Returns metadata for the specified country if it is available.
        /// </summary>
        /// <param name="global">Global metadata.</param>
        /// <param name="country">Identifier of the country to return.</param>
        /// <param name="language">Language code for the metadata to return.</param>

        public async Task<ICountryMetadata> GetCountry( string country, string language )
        {
            var global = await GetGlobal();
            var defaults = await Client.Query<CountryMetadata>( "data/ZZ" );
            
            // handle request for default country
            if ( country == "ZZ" ) return defaults;

            // handle non-existing country
            if ( global == null ) return null;
            if ( !global.Countries.IndexOf( country, out int index, StringComparer.OrdinalIgnoreCase ) ) return null;
            
            country = global.Countries.ElementAt( index );
            var id = BuildIdentifier( language, country );
            var result = await Client.Query<CountryMetadata>( id );

            if ( result != null && defaults != null )
            {
                result.Format = result.Format ?? defaults.Format;
                result.Required = result.Required ?? defaults.Required;
                result.Uppercase = result.Uppercase ?? defaults.Uppercase;
                result.StateType = result.StateType ?? defaults.StateType;
                result.LocalityType = result.LocalityType ?? defaults.LocalityType;
                result.SublocalityType = result.SublocalityType ?? defaults.SublocalityType;
                result.PostalCodeType = result.PostalCodeType ?? defaults.PostalCodeType;
            }

            return result;
        }

        /// <summary>
        /// Returns whether the specified regional identifier is a child of the given parent region.
        /// </summary>
        /// <param name="parent">Parent region.</param>
        /// <param name="value">Key, name, or latin name of the child region.</param>
        /// <param name="key">Key of the child region, if found.</param>

        bool TryGetChildKey( IHierarchicalMetadata parent, string value, out string key )
        {
            int? getKeyIndex( params IEnumerable<string>[] collections )
            {
                foreach ( var collection in collections )
                {
                    var candidate = collection != null
                        ? Array.FindIndex( collection as string[] ?? collection?.ToArray(), _ => _.Equals( value, StringComparison.OrdinalIgnoreCase ) )
                        : (int?)null;

                    if ( candidate >= 0 ) return candidate;
                }

                return null;
            }

            var index = getKeyIndex( parent?.ChildRegionKeys, parent?.ChildRegionNames, parent?.ChildRegionLatinNames );

            key = index.HasValue
                ? parent.ChildRegionKeys.ElementAtOrDefault( index.Value )
                : null;

            return key != null;
        }

        /// <summary>
        /// Returns metadata for the specified province if it is available.
        /// </summary>
        /// <param name="country">Parent country of the province.</param>
        /// <param name="province">Province key or name.</param>
        /// <param name="language">Language code for the metadata to return.</param>

        public async Task<IProvinceMetadata> GetProvince( ICountryMetadata country, string province, string language ) =>
            TryGetChildKey( country, province, out string provinceKey )
                ? await Client.Query<ProvinceMetadata>( BuildIdentifier( language, country, provinceKey ) )
                : null;

        //public async Task<ILocalityMetadata> GetLocality( IProvinceMetadata province, string locality, string language ) =>
    }
}