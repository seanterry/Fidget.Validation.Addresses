using Fidget.Commander;
using Fidget.Validation.Addresses.Metadata.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Factory for creating address validation contexts.
    /// </summary>

    class ValidationContextFactory : IValidationContextFactory
    {
        /// <summary>
        /// Command dispatcher.
        /// </summary>
        
        readonly ICommandDispatcher Dispatcher;

        /// <summary>
        /// Constructs a factory for creating address validation contexts.
        /// </summary>
        /// <param name="dispatcher">Command dispatcher.</param>
        
        public ValidationContextFactory( ICommandDispatcher dispatcher )
        {
            Dispatcher = dispatcher ?? throw new ArgumentNullException( nameof(dispatcher) );
        }

        /// <summary>
        /// Creates and returns a context for validating the given address.
        /// </summary>
        /// <param name="address">Address to validate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        
        public async Task<ValidationContext> Create( AddressData address, CancellationToken cancellationToken ) => new ValidationContext
        {
            Address = address ?? throw new ArgumentNullException( nameof(address) ),
            GlobalMetadata = await Dispatcher.Execute( GlobalMetadataQuery.Default, cancellationToken ),
            CountryMetadata = await Dispatcher.Execute( CountryMetadataQuery.For( address ), cancellationToken ),
            ProvinceMetadata = await Dispatcher.Execute( ProvinceMetadataQuery.For( address ), cancellationToken ),
            LocalityMetadata = await Dispatcher.Execute( LocalityMetadataQuery.For( address ), cancellationToken ),
            SublocalityMetadata = await Dispatcher.Execute( SublocalityMetadataQuery.For( address ), cancellationToken ),
        };
    }
}