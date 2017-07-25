using Fidget.Validation.Addresses.Adapters;
using Fidget.Validation.Addresses.Client;
using Fidget.Validation.Addresses.Client.Decorators;
using Fidget.Validation.Addresses.Service;
using Fidget.Validation.Addresses.Validation;
using Microsoft.Extensions.Caching.Memory;
using StructureMap;

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
                config.For<IServiceClient>().Use<ServiceClient>();
                config.For<IServiceClient>().DecorateAllWith<CachingDecorator>();
                config.For<IServiceClient>().DecorateAllWith<NullifyingDecorator>();
                config.For<IServiceClient>().DecorateAllWith<CopyingDecorator>();

                config.For<IGlobalAdapter>().Use<GlobalAdapter>();

                config.For<IServiceAdapter>().Use<ServiceAdapter>().Singleton();

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