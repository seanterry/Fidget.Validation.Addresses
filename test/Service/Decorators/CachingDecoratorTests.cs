using Fidget.Validation.Addresses.Metadata;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Service.Decorators
{
    public class CachingDecoratorTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();
        Mock<IMemoryCache> MockCache = new Mock<IMemoryCache>();

        IServiceClient client => MockClient?.Object;
        IMemoryCache cache => MockCache?.Object;

        IServiceClient create() => new CachingDecorator( client, cache );

        public class Constructor : CachingDecoratorTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof( client ), () => create() );
            }

            [Fact]
            public void Requires_cache()
            {
                MockCache = null;
                Assert.Throws<ArgumentNullException>( nameof(cache), ()=> create() );
            }

            [Fact]
            public void Implements_IServiceClient()
            {
                var actual = create();
                Assert.IsType<CachingDecorator>( actual );
                Assert.IsAssignableFrom<IServiceClient>( actual );
            }
        }

        public class Query : CachingDecoratorTests
        {
            class Metadata : CommonMetadata {}

            string id = Guid.NewGuid().ToString();
            async Task<Metadata> invoke( IServiceClient instance ) => await instance.Query<Metadata>( id );

            Mock<ICacheEntry> MockCacheEntry = new Mock<ICacheEntry>();
            ICacheEntry cacheEntry => MockCacheEntry?.Object;

            string key => $"{id}/{typeof(Metadata).FullName}";

            [Fact]
            public async Task Requires_id()
            {
                id = null;
                var instance = create();
                await Assert.ThrowsAsync<ArgumentNullException>( nameof(id), () => invoke( instance ) );
            }

            [Fact]
            public async Task Returns_valueFromCache_onCacheHit()
            {
                var instance = create();
                var expected = new Metadata();
                object cached = new Lazy<Task<Metadata>>( () => Task.FromResult( expected ), LazyThreadSafetyMode.PublicationOnly );
                
                MockCache.Setup( _=> _.TryGetValue( key, out cached ) ).Returns( true );
                
                var actual = await invoke( instance );
                Assert.Same( expected, actual );
            }

            // configures a cache miss scenario where the value is populated by the underlying client
            Metadata configureCacheMiss()
            {
                var expected = new Metadata();
                object cached = null;
                MockCache.Setup( _ => _.TryGetValue( key, out cached ) ).Returns( false );
                MockCache.Setup( _ => _.CreateEntry( key ) ).Returns( cacheEntry );
                MockClient.Setup( _ => _.Query<Metadata>( id ) ).ReturnsAsync( expected );
                MockCacheEntry.SetupAllProperties();

                return expected;
            }

            [Fact]
            public async Task Returns_valueFromClient_onCacheMiss()
            {
                var instance = create();
                var expected = configureCacheMiss();
                
                var actual = await invoke( instance );
                Assert.Same( expected, actual );

                // ensure value was cached
                MockCache.Verify( _ => _.CreateEntry( key ), Times.Once );
            }

            [Fact]
            public async Task Caches_forTwelveHours()
            {
                var instance = create();
                var expected = configureCacheMiss();

                await invoke( instance );

                Assert.Equal( 12, cacheEntry.AbsoluteExpirationRelativeToNow.Value.Hours );
            }

            [Fact]
            public async Task Reinitializes_whenInitializeFails()
            {
                var expected = configureCacheMiss();
                
                // reconfigure client to throw an exception on the first call
                var calls = 0;
                MockClient = new Mock<IServiceClient>();
                MockClient.Setup( _=> _.Query<Metadata>( id ) )
                    .ReturnsAsync( expected )
                    .Callback( () => { if ( calls++ == 0 ) throw new Exception(); } );

                var instance = create();

                // first call should fail
                await Assert.ThrowsAsync<Exception>( () => invoke( instance ) );

                // second call (via the lazy) should succeed
                var lazy = cacheEntry.Value as Lazy<Task<Metadata>>;
                var actual = await lazy.Value;

                Assert.Same( expected, actual );
            }
        }
    }
}