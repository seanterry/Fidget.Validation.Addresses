using Fidget.Validation.Addresses.Metadata;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Client.Decorators
{
    public class CachingDecoratorTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();
        Mock<IMemoryCache> MockCache = new Mock<IMemoryCache>();

        IServiceClient client => MockClient?.Object;
        IMemoryCache cache => MockCache?.Object;
        IServiceClient instance => new CachingDecorator( client, cache );
        
        public class Constructor : CachingDecoratorTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof( client ), () => instance );
            }

            [Fact]
            public void Requires_cache()
            {
                MockCache = null;
                Assert.Throws<ArgumentNullException>( nameof(cache), ()=> instance );
            }
        }

        public class Query : CachingDecoratorTests
        {
            class Metadata : CommonMetadata {}

            string id = Guid.NewGuid().ToString();
            async Task<Metadata> invoke() => await instance.Query<Metadata>( id );

            Mock<ICacheEntry> MockCacheEntry = new Mock<ICacheEntry>();
            ICacheEntry cacheEntry => MockCacheEntry?.Object;

            string key => $"{id}/{typeof(Metadata).FullName}";

            [Fact]
            public async Task Requires_id()
            {
                id = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof(id), () => invoke() );
            }

            [Fact]
            public async Task WhenCacheHit_returns_cachedValue()
            {
                var expected = new Metadata();
                object cached = new Lazy<Task<Metadata>>( () => Task.FromResult( expected ), LazyThreadSafetyMode.PublicationOnly );
                
                MockCache.Setup( _=> _.TryGetValue( key, out cached ) ).Returns( true );
                
                var actual = await invoke();
                Assert.Same( expected, actual );
            }

            /// <summary>
            /// Configures a cache miss scenario where the value is populated by the underlying client 
            /// </summary>
            
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
            public async Task WhenCacheMiss_returns_clientValue()
            {
                var expected = configureCacheMiss();
                var actual = await invoke();
                Assert.Same( expected, actual );

                // ensure value was cached for 12 hours
                MockCache.Verify( _ => _.CreateEntry( key ), Times.Once );
                Assert.Equal( 12, cacheEntry.AbsoluteExpirationRelativeToNow.Value.Hours );
            }

            [Fact]
            public async Task WhenInitializationFails_reinitializes()
            {
                var expected = configureCacheMiss();
                
                // reconfigure client to throw an exception on the first call
                var calls = 0;
                MockClient = new Mock<IServiceClient>();
                MockClient.Setup( _=> _.Query<Metadata>( id ) )
                    .ReturnsAsync( expected )
                    .Callback( () => { if ( calls++ == 0 ) throw new Exception(); } );

                // first call should fail
                await Assert.ThrowsAsync<Exception>( () => invoke() );

                // second call (via the lazy) should succeed
                var lazy = cacheEntry.Value as Lazy<Task<Metadata>>;
                var actual = await lazy.Value;

                Assert.Same( expected, actual );
            }
        }
    }
}