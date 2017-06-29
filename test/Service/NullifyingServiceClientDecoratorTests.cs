using Fidget.Validation.Addresses.Service.Metadata;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Service
{
    public class NullifyingServiceClientDecoratorTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();
        IServiceClient client => MockClient?.Object;

        IServiceClient create() => new NullifyingServiceClientDecorator( client );

        public class Constructor : NullifyingServiceClientDecoratorTests
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
                Assert.IsType<NullifyingServiceClientDecorator>( actual );
                Assert.IsAssignableFrom<IServiceClient>( actual );
            }
        }

        public class Query : NullifyingServiceClientDecoratorTests
        {
            class Metadata : ICommonMetadata
            {
                public string Id { get; set; }
            }

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
