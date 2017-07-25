using Fidget.Validation.Addresses.Metadata;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Service.Decorators
{
    public class NullifyingDecoratorTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();
        IServiceClient client => MockClient?.Object;

        IServiceClient create() => new NullifyingDecorator( client );

        public class Constructor : NullifyingDecoratorTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof( client ), () => create() );
            }

            [Fact]
            public void Implements_IServiceClient()
            {
                var actual = create();
                Assert.IsType<NullifyingDecorator>( actual );
                Assert.IsAssignableFrom<IServiceClient>( actual );
            }
        }

        public class Query : NullifyingDecoratorTests
        {
            class Metadata : CommonMetadata {}

            string id = Guid.NewGuid().ToString();
            async Task<Metadata> invoke( IServiceClient instance ) => await instance.Query<Metadata>( id );

            [Fact]
            public async Task Requires_id()
            {
                id = null;
                var instance = create();
                await Assert.ThrowsAsync<ArgumentNullException>( nameof( id ), () => invoke( instance ) );
            }

            [Fact]
            public async Task Returns_null_whenIdNull()
            {
                var instance = create();
                var expected = new Metadata { Id = null };
                MockClient.Setup( _=> _.Query<Metadata>( id ) ).ReturnsAsync( expected );

                var actual = await invoke( instance );
                Assert.Null( actual );
            }

            [Fact]
            public async Task Returns_valueFromClient_whenIdNotNull()
            {
                var instance = create();
                var expected = new Metadata { Id = id };
                MockClient.Setup( _ => _.Query<Metadata>( id ) ).ReturnsAsync( expected );

                var actual = await invoke( instance );
                Assert.Same( expected, actual );
            }
        }
    }
}
