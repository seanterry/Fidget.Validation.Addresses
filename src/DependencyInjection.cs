using Fidget.Validation.Addresses.Service;
using Fidget.Validation.Addresses.Service.Decorators;
using Fidget.Validation.Addresses.Validation;
using Microsoft.Extensions.Caching.Memory;
using StructureMap;
using System.Collections;
using System.Collections.Generic;

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
            void configure( ConfigurationExpression config )
            {
                config.For<IMemoryCache>().Use( new MemoryCache( new MemoryCacheOptions() ) ).Singleton();
                config.For<IServiceClient>().Use<ServiceClient>().Singleton();
                config.For<IServiceClient>().DecorateAllWith<CachingDecorator>().Singleton();
                config.For<IServiceClient>().DecorateAllWith<NullifyingDecorator>().Singleton();
                config.For<IServiceClient>().DecorateAllWith<CopyingDecorator>().Singleton();
                
                config.For<IAddressValidator>().Use<RequiredElementsValidator>().Singleton();
                config.For<IAddressValidator>().Use<AllowedElementsValidator>().Singleton();
                config.For<IValidationContextFactory>().Use<ValidationContext.Factory>().Singleton();
                config.For<IAddressService>().Use<AddressService.Implementation>().Singleton();
            }

            return new Container( configure );
        }

        /// <summary>
        /// Dependency injection container to use at runtime.
        /// </summary>
        
        public static readonly IContainer Container = CreateContainer();
    }
}