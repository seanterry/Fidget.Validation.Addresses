using Fidget.Commander;
using Fidget.Validation.Addresses.Client;
using Moq;
using System;
using Xunit;

namespace Fidget.Validation.Addresses.Metadata.Commands
{
    public class MetadataQueryContextTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();
        Mock<IKeyBuilder> MockBuilder = new Mock<IKeyBuilder>();
        Mock<ICommandDispatcher> MockDispatcher = new Mock<ICommandDispatcher>();

        IServiceClient client => MockClient?.Object;
        IKeyBuilder builder => MockBuilder?.Object;
        ICommandDispatcher dispatcher => MockDispatcher?.Object;

        IMetadataQueryContext instance => new MetadataQueryContext( client, builder, dispatcher );

        public class Registration
        {
            [Fact]
            public void IsRegistered()
            {
                var actual = DependencyInjection.Container.GetInstance<IMetadataQueryContext>();
                Assert.IsType<MetadataQueryContext>( actual );
            }
        }

        [Fact]
        public void Requires_client()
        {
            MockClient = null;
            Assert.Throws<ArgumentNullException>( nameof( client ), () => instance );
        }

        [Fact]
        public void Requires_builder()
        {
            MockBuilder = null;
            Assert.Throws<ArgumentNullException>( nameof( builder ), () => instance );
        }

        [Fact]
        public void Requires_dispatcher()
        {
            MockDispatcher = null;
            Assert.Throws<ArgumentNullException>( nameof( dispatcher ), () => instance );
        }

        [Fact]
        public void Client_returns_expected()
        {
            var actual = instance.Client;
            Assert.Equal( client, actual );
        }

        [Fact]
        public void Builder_returns_expected()
        {
            var actual = instance.Builder;
            Assert.Equal( builder, actual );
        }

        [Fact]
        public void Dispatcher_returns_expected()
        {
            var actual = instance.Dispatcher;
            Assert.Equal( dispatcher, actual );
        }
    }
}