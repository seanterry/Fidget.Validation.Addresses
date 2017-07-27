using Fidget.Validation.Addresses.Metadata;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Client.Decorators
{
    public class NullifyingDecoratorTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();
        IServiceClient client => MockClient?.Object;

        IServiceClient instance => new NullifyingDecorator( client );

        public class Constructor : NullifyingDecoratorTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof( client ), () => instance );
            }
        }

        public class Query : NullifyingDecoratorTests
        {
            class Metadata : CommonMetadata {}

            string id = Guid.NewGuid().ToString();
            async Task<Metadata> invoke() => await instance.Query<Metadata>( id );

            [Fact]
            public async Task Requires_id()
            {
                id = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof( id ), invoke );
            }

            [Fact]
            public async Task Returns_null_whenIdNull()
            {
                var expected = new Metadata { Id = null };
                MockClient.Setup( _=> _.Query<Metadata>( id ) ).ReturnsAsync( expected );

                var actual = await invoke();
                Assert.Null( actual );
            }

            [Fact]
            public async Task Returns_valueFromClient_whenIdNotNull()
            {
                var expected = new Metadata { Id = id };
                MockClient.Setup( _ => _.Query<Metadata>( id ) ).ReturnsAsync( expected );

                var actual = await invoke();
                Assert.Same( expected, actual );
            }
        }
    }
}