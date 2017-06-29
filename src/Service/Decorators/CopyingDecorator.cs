using Fidget.Extensions.Reflection;
using Fidget.Validation.Addresses.Service.Metadata;
using System;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Service
{
    /// <summary>
    /// Decorator for providing copies of metadata.
    /// </summary>

    class CopyingDecorator : IServiceClient
    {
        /// <summary>
        /// Service client decorated by the current instance.
        /// </summary>

        readonly IServiceClient Client;

        /// <summary>
        /// Constructs a decorator that creates copies of the address metadata.
        /// </summary>
        /// <param name="client">Service client decorated by the current instance.</param>

        public CopyingDecorator( IServiceClient client )
        {
            Client = client ?? throw new ArgumentNullException( nameof( client ) );
        }

        /// <summary>
        /// Returns a cached query response for the specified record if one exists, otherwise querying the remote service.
        /// </summary>
        /// <typeparam name="T">Type of the metadata response.</typeparam>
        /// <param name="id">Data record to return.</param>

        public async Task<T> Query<T>( string id ) where T : ICommonMetadata
        {
            if ( id == null ) throw new ArgumentNullException( nameof( id ) );
            
            var result = await Client.Query<T>( id );
            var reflector = result.Reflect();
            
            return result != null ? reflector.Clone( result ) : result;
        }
    }
}