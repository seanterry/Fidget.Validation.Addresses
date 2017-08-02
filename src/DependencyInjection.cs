using Fidget.Commander;
using Fidget.Commander.Dispatch;
using Fidget.Validation.Addresses.Client;
using Fidget.Validation.Addresses.Client.Decorators;
using Fidget.Validation.Addresses.Metadata.Commands;
using Fidget.Validation.Addresses.Validation;
using Microsoft.Extensions.Caching.Memory;
using StructureMap;
using System;

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
                // register assembly contents
                config.Scan( scanner =>
                {
                    scanner.AssemblyContainingType<AddressData>();
                    scanner.ConnectImplementationsToTypesClosing( typeof( ICommandHandler<,> ) );
                    scanner.ConnectImplementationsToTypesClosing( typeof( ICommandDecorator<,> ) );
                    scanner.AddAllTypesOf<IAddressValidator>();
                });

                // register command pipeline
                config.For<IServiceProvider>().Use<StructureMapServiceProvider>();
                config.For( typeof(ICommandAdapter<,>) ).Use( typeof(CommandAdapter<,>) );
                config.For<ICommandAdapterFactory>().Use<CommandAdapterFactory>();
                config.For<ICommandDispatcher>().Use<CommandDispatcher>();

                // register service dependencies
                config.For<IMemoryCache>().Use( new MemoryCache( new MemoryCacheOptions() ) ).Singleton();
                config.For<IServiceClient>().Use<ServiceClient>();
                config.For<IServiceClient>().DecorateAllWith<CachingDecorator>();
                config.For<IServiceClient>().DecorateAllWith<NullifyingDecorator>();
                config.For<IServiceClient>().DecorateAllWith<CopyingDecorator>();
                config.For<IKeyBuilder>().Use<KeyBuilder>();
                config.For<IMetadataQueryContext>().Use<MetadataQueryContext>();
                config.For<IValidationContextFactory>().Use<ValidationContextFactory>();
                config.For<IAddressService>().Use<AddressService>();
            }

            return new Container( configure );
        }

        /// <summary>
        /// Dependency injection container to use at runtime.
        /// </summary>
        
        public static readonly IContainer Container = CreateContainer();
    }
}