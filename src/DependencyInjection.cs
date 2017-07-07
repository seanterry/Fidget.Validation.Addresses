using Fidget.Validation.Addresses.Service;
using Fidget.Validation.Addresses.Service.Decorators;
using Microsoft.Extensions.Caching.Memory;
using StructureMap;
using StructureMap.Graph;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fidget.Validation.Addresses
{
    /// <summary>
    /// Static methods for configuring dependency injection.
    /// </summary>
    
    static class DependencyInjection
    {
        /// <summary>
        /// Creates and returns a configured StructureMap container.
        /// </summary>
        
        static Container CreateContainer()
        {
            void scan( IAssemblyScanner scanner )
            {
            }

            void configure( ConfigurationExpression config )
            {
                config.Scan( scan );

                config.For<IMemoryCache>().Use( new MemoryCache( new MemoryCacheOptions() ) ).Singleton();
                config.For<IServiceClient>().Use<ServiceClient>().Singleton();
                config.For<IServiceClient>().DecorateAllWith<CachingDecorator>().Singleton();
                config.For<IServiceClient>().DecorateAllWith<NullifyingDecorator>().Singleton();
                config.For<IServiceClient>().DecorateAllWith<CopyingDecorator>().Singleton();
            }

            return new Container( configure );
        }

        /// <summary>
        /// Dependency injection container to use at runtime.
        /// </summary>
        
        public static readonly Container Container = CreateContainer();
    }
}