using Fidget.Validation.Addresses.Service.Metadata;
using System;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Service.Decorators
{
    /// <summary>
    /// Decorator that converts responses to default values of the type when the response is not a valid entry.
    /// </summary>

    class NullifyingDecorator : IServiceClient
    {
        /// <summary>
        /// Service client decorated by the current instance.
        /// </summary>

        readonly IServiceClient Client;

        /// <summary>
        /// Constructs a decorator that converts responses to default values of the type when the response is not a valid entry.
        /// </summary>
        /// <param name="client">Service client decorated by the current instance.</param>
        
        public NullifyingDecorator( IServiceClient client )
        {
            Client = client ?? throw new ArgumentNullException( nameof(client) );
        }

        /// <summary>
        /// Returns a query response for the specified record if it has an identifier, otherwise null.
        /// </summary>
        /// <typeparam name="T">Type of the metadata response.</typeparam>
        /// <param name="id">Data record to return.</param>

        public async Task<T> Query<T>( string id ) where T : ICommonMetadata
        {
            if ( id == null ) throw new ArgumentNullException( nameof( id ) );

            var result = await Client.Query<T>( id );

            return result.Id != null
                ? result
                : default(T);
        }
    }
}