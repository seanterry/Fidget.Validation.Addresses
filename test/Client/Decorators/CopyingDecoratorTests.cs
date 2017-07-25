using Fidget.Extensions.Reflection;
using Fidget.Validation.Addresses.Metadata;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Client.Decorators
{
    public class CopyingDecoratorTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();
        IServiceClient client => MockClient?.Object;

        IServiceClient instance => new CopyingDecorator( client );

        public class Constructor : CopyingDecoratorTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof( client ), () => instance );
            }
        }

        public class Query : CopyingDecoratorTests
        {
            class Metadata : CommonMetadata {}

            string id = Guid.NewGuid().ToString();
            async Task<Metadata> invoke() => await instance.Query<Metadata>( id );

            [Fact]
            public async Task Requires_id()
            {
                id = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof( id ), () => invoke() );
            }

            [Fact]
            public async Task WhenSourceIsNull_returnsNull()
            {
                MockClient.Setup( _=> _.Query<Metadata>( id ) ).ReturnsAsync( (Metadata)null );
                
                var actual = await invoke();
                Assert.Null( actual );
            }

            [Fact]
            public async Task Returns_equivalentValue()
            { 
                var expected = new Metadata { Id = id };
                MockClient.Setup( _=> _.Query<Metadata>( id ) ).ReturnsAsync( expected );

                var actual = await invoke();
                var reflector = TypeReflectorExtensions.Reflect<Metadata>();

                Assert.All( reflector, _=> _.Equal( expected, actual ) );
                Assert.NotSame( expected, actual );
            }
        }
    }
}
