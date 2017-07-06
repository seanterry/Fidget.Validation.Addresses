using Fidget.Validation.Addresses.Service.Metadata;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Service.Decorators
{
    /// <summary>
    /// Decorator that caches service client responses.
    /// </summary>

    class CachingDecorator : IServiceClient
    {
        /// <summary>
        /// Service client decorated by the current instance.
        /// </summary>
        
        readonly IServiceClient Client;

        /// <summary>
        /// Cache storage.
        /// </summary>
        
        readonly IMemoryCache Cache;

        /// <summary>
        /// Constructs a decorator that caches service client responses.
        /// </summary>
        /// <param name="client">Service client to be decorated.</param>
        /// <param name="cache">Cache storage.</param>
        
        public CachingDecorator( IServiceClient client, IMemoryCache cache )
        {
            Client = client ?? throw new ArgumentNullException( nameof(client) );
            Cache = cache ?? throw new ArgumentNullException( nameof(cache) );
        }

        /// <summary>
        /// Returns a cached query response for the specified record if one exists, otherwise querying the remote service.
        /// </summary>
        /// <typeparam name="T">Type of the metadata response.</typeparam>
        /// <param name="id">Data record to return.</param>
        
        public async Task<T> Query<T>( string id ) where T : ICommonMetadata
        {
            if ( id == null ) throw new ArgumentNullException( nameof( id ) );

            var key = $"{id}/{typeof(T).FullName}";

            Lazy<Task<T>> factory( ICacheEntry entry )
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours( 12 );
                return new Lazy<Task<T>>( () => Client.Query<T>( id ), LazyThreadSafetyMode.PublicationOnly );
            }

            return await Cache
                .GetOrCreate( key, factory )
                .Value;
        }
    }
}