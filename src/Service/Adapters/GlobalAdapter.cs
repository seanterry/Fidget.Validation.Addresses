using Fidget.Validation.Addresses.Metadata;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Service.Adapters
{
    /// <summary>
    /// Adapter for interacting with global metadata.
    /// </summary>

    class GlobalAdapter : IGlobalAdapter
    {
        /// <summary>
        /// Client that backs the current instance.
        /// </summary>
        
        readonly IServiceClient Client;

        /// <summary>
        /// Constructs an adapter for interacting with global metadata.
        /// </summary>
        /// <param name="client">Service client that will back the instance.</param>
        
        public GlobalAdapter( IServiceClient client )
        {
            Client = client ?? throw new ArgumentNullException( nameof(client) );
        }

        /// <summary>
        /// Returns global metadata.
        /// </summary>
        
        public async Task<GlobalMetadata> Query() => await Client.Query<GlobalMetadata>( "data" );

        /// <summary>
        /// Returns whether the given country value can be located in the global metadata.
        /// </summary>
        /// <param name="global">Global metadata (always returns false if not provided).</param>
        /// <param name="country">
        /// Value to search for a country key.
        /// This can be a case-insensitive value that matches a country key.
        /// </param>
        /// <param name="key">Key of the country, if found.</param>
        
        public bool TryGetCountryKey( GlobalMetadata global, string country, out string key )
        {
            var countries = global?.Countries;

            if ( countries != null && countries.IndexOf( country, out int index, StringComparer.OrdinalIgnoreCase ) )
            {
                key = global.Countries.ElementAt( index );
                return true;
            }

            else 
            {
                key = null;
                return false;
            }
        }
    }
}