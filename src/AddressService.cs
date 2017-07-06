using Fidget.Validation.Addresses.Service;
using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using System;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses
{
    /// <summary>
    /// Service for address validation and metadata exploration.
    /// </summary>

    public class AddressService : IAddressService
    {
        /// <summary>
        /// Default service client factory.
        /// </summary>
        
        static readonly ServiceClient.Factory ServiceClientFactory = new ServiceClient.Factory();

        /// <summary>
        /// Remote address service client.
        /// </summary>
        
        readonly IServiceClient Client;

        /// <summary>
        /// Constructs a service for address validation and metadata exploration.
        /// </summary>
        /// <param name="client">Remote address service client.</param>
        
        internal AddressService( IServiceClient client )
        {
            Client = client ?? throw new ArgumentNullException( nameof(client) );
        }

        /// <summary>
        /// Constructs a service for address validation and metadata exploration.
        /// </summary>
        
        public AddressService() : this( ServiceClient.Default ) {}

        /// <summary>
        /// Returns gobal metadata information.
        /// </summary>
        
        public async Task<IGlobalMetadata> GetGlobalAsync() => await Client.Query<GlobalMetadata>( "data" );

        /// <summary>
        /// Returns gobal metadata information.
        /// </summary>
        
        public IGlobalMetadata GetGlobal() => GetGlobalAsync().Result;
    }
}