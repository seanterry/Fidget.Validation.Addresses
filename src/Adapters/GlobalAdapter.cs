using Fidget.Validation.Addresses.Client;
using Fidget.Validation.Addresses.Metadata;
using System;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Adapters
{
    /// <summary>
    /// Adapter for querying global metadata.
    /// </summary>

    class GlobalAdapter : IGlobalAdapter
    {
        /// <summary>
        /// Service client that backs the current instance.
        /// </summary>
        
        readonly IServiceClient Client;

        /// <summary>
        /// Constructs an adapter for querying global metadata.
        /// </summary>
        /// <param name="client">Service client that will back the instance.</param>
        
        public GlobalAdapter( IServiceClient client )
        {
            Client = client ?? throw new ArgumentNullException( nameof(client) );
        }

        /// <summary>
        /// Returns the global metadata entry.
        /// </summary>
        
        public async Task<GlobalMetadata> QueryAsync() => await Client.Query<GlobalMetadata>( "data" );
    }
}