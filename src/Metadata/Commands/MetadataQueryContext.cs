using Fidget.Commander;
using Fidget.Validation.Addresses.Client;
using System;

namespace Fidget.Validation.Addresses.Metadata.Commands
{
    /// <summary>
    /// Common metadata query dependencies.
    /// </summary>

    class MetadataQueryContext : IMetadataQueryContext
    {
        /// <summary>
        /// Gets the remote address service client.
        /// </summary>
        
        public IServiceClient Client { get; }

        /// <summary>
        /// Gets the key/identifier builder.
        /// </summary>
        
        public IKeyBuilder Builder { get; }

        /// <summary>
        /// Gets the command dispatcher.
        /// </summary>
        
        public ICommandDispatcher Dispatcher { get; }

        /// <summary>
        /// Constructs a context for common metadata query dependencies. 
        /// </summary>
        /// <param name="client">Remote address service client.</param>
        /// <param name="builder">Key/identifier builder.</param>
        /// <param name="dispatcher">Command dispatcher.</param>
        
        public MetadataQueryContext( IServiceClient client, IKeyBuilder builder, ICommandDispatcher dispatcher )
        {
            Client = client ?? throw new ArgumentNullException( nameof(client) );
            Builder = builder ?? throw new ArgumentNullException( nameof(builder) );
            Dispatcher = dispatcher ?? throw new ArgumentNullException( nameof(dispatcher) );
        }
    }
}
