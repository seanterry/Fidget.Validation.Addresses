using Fidget.Commander;
using Fidget.Validation.Addresses.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Metadata.Commands
{
    /// <summary>
    /// Command for querying global metadata.
    /// </summary>

    public struct GlobalMetadataQuery : ICommand<GlobalMetadata>
    {
        /// <summary>
        /// Gets the default instance of the command.
        /// </summary>
        
        internal static GlobalMetadataQuery Default { get; } = new GlobalMetadataQuery();

        /// <summary>
        /// Handler for the command.
        /// </summary>
        
        public class Handler : ICommandHandler<GlobalMetadataQuery, GlobalMetadata>
        {
            /// <summary>
            /// Remote address service client.
            /// </summary>
            
            readonly IServiceClient Client;

            /// <summary>
            /// Constructs a handler for the command.
            /// </summary>
            /// <param name="client">Remote address service client.</param>
            
            public Handler( IServiceClient client )
            {
                Client = client ?? throw new ArgumentNullException( nameof(client) );
            }

            /// <summary>
            /// Executes the command.
            /// </summary>
            
            public async Task<GlobalMetadata> Handle( GlobalMetadataQuery command, CancellationToken cancellationToken ) =>
                await Client.Query<GlobalMetadata>( "data" );
        }
    }
}