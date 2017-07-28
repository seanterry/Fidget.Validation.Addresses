using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fidget.Validation.Addresses.Metadata;
using Fidget.Commander;
using Fidget.Validation.Addresses.Metadata.Commands;
using System.Threading;

namespace Fidget.Validation.Addresses
{
    /// <summary>
    /// Service for address validation and metadata exploration.
    /// </summary>

    public class AddressService : IAddressService
    {
        /// <summary>
        /// Gets the default instance of the service.
        /// </summary>
        
        public static IAddressService Default { get; } = DependencyInjection.Container.GetInstance<IAddressService>();

        /// <summary>
        /// Command dispatcher.
        /// </summary>
        
        readonly ICommandDispatcher Dispatcher;

        /// <summary>
        /// Constructs a service for address validation and metadata exploration.
        /// </summary>
        /// <param name="dispatcher">Command dispatcher.</param>
        
        public AddressService( ICommandDispatcher dispatcher )
        {
            Dispatcher = dispatcher ?? throw new ArgumentNullException( nameof(dispatcher) );
        }

        /// <summary>
        /// Returns gobal metadata information.
        /// </summary>
        
        public async Task<GlobalMetadata> GetGlobalAsync( CancellationToken cancellationToken ) => 
            await Dispatcher.Execute( GlobalMetadataQuery.Default, cancellationToken );
    }
}