using Fidget.Validation.Addresses.Service.Decorators;
using Microsoft.Extensions.Caching.Memory;

namespace Fidget.Validation.Addresses.Service
{
    partial class ServiceClient
    {
        /// <summary>
        /// Factory for creating the standard service client.
        /// </summary>
        
        public class Factory
        {
            /// <summary>
            /// Creates and returns the standard service client.
            /// </summary>
            
            public IServiceClient Create()
            {
                var client = new ServiceClient();
                var cache = new MemoryCache( new MemoryCacheOptions() );
                
                return 
                    new CopyingDecorator(
                        new NullifyingDecorator(
                            new CachingDecorator( client, cache )
                        )
                    );
            }
        }
    }
}