using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;
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
        /// Returns global-level address metadata.
        /// </summary>
        
        public async Task<IGlobalMetadata> GetGlobal() => await Client.Query<GlobalMetadata>( "data" );
    }
}