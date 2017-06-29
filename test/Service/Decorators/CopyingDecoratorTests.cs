using Fidget.Extensions.Reflection;
using Fidget.Validation.Addresses.Service.Metadata;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Service.Decorators
{
    public class CopyingDecoratorTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();
        IServiceClient client => MockClient?.Object;

        IServiceClient create() => new CopyingDecorator( client );

        public class Constructor : CopyingDecoratorTests
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
                Assert.IsType<CopyingDecorator>( actual );
                Assert.IsAssignableFrom<IServiceClient>( actual );
            }
        }

        public class Query : CopyingDecoratorTests
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
            public async Task Returns_null_whenSourceIsNull()
            {
                var instance = create();
                MockClient.Setup( _=> _.Query<Metadata>( id ) ).ReturnsAsync( (Metadata)null );
                
                var actual = await invoke( instance );
                Assert.Null( actual );
            }

            [Fact]
            public async Task Returns_equivalentValue()
            { 
                var expected = new Metadata { Id = id };
                var instance = create();
                MockClient.Setup( _=> _.Query<Metadata>( id ) ).ReturnsAsync( expected );

                var actual = await invoke( instance );
                var reflector = TypeReflectorExtensions.Reflect<Metadata>();

                Assert.All( reflector, _=> _.Equal( expected, actual ) );
                Assert.NotSame( expected, actual );
            }
        }
    }
}
