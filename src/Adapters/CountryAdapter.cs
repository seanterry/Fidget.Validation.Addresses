using Fidget.Validation.Addresses.Client;
using Fidget.Validation.Addresses.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Adapters
{
    /// <summary>
    /// Adapter for querying country metadata.
    /// </summary>
    
    class CountryAdapter : ICountryAdapter
    {
        /// <summary>
        /// Service client that backs the current instance.
        /// </summary>

        readonly IServiceClient Client;

        /// <summary>
        /// Global metadata adapter.
        /// </summary>

        readonly IGlobalAdapter Global;

        /// <summary>
        /// Key location service.
        /// </summary>
        
        readonly IKeyService KeyService;

        /// <summary>
        /// Constructs an adapter for querying country metadata.
        /// </summary>
        /// <param name="client">Service client that will back the instance.</param>
        /// <param name="global">Global metadata adapter.</param>
        /// <param name="keyService">Key location service.</param>
        
        public CountryAdapter( IServiceClient client, IGlobalAdapter global, IKeyService keyService )
        {
            Client = client ?? throw new ArgumentNullException( nameof(client) );
            Global = global ?? throw new ArgumentNullException( nameof(global) );
            KeyService = keyService ?? throw new ArgumentNullException( nameof(keyService) );
        }

        /// <summary>
        /// Returns the default country metadata values.
        /// </summary>
        
        public async Task<CountryMetadata> QueryDefaultAsync() => await Client.Query<CountryMetadata>( "data/ZZ" );
        
        /// <summary>
        /// Queries the service for the specified country.
        /// </summary>
        /// <param name="country">Key of the country to return (case insensitive).</param>
        /// <param name="language">Language of the metadata to return.</param>
        /// <returns>The specified country if it exists, otherwise null.</returns>
        
        public async Task<CountryMetadata> QueryAsync( string country, string language )
        {
            var globalMeta = await Global.QueryAsync();
            
            // short-circuit if the key is not found
            if ( !KeyService.TryGetCountryKey( globalMeta, country, out string key ) ) return null;

            var id = KeyService.BuildIdentifier( globalMeta, key, language );
            var result = await Client.Query<CountryMetadata>( id );
            var defaults = await QueryDefaultAsync();

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
    }
}