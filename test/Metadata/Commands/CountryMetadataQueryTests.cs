using Fidget.Commander;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Metadata.Commands
{
    public class CountryMetadataQueryTests
    {
        FakeContext FakeContext = new FakeContext();
        
        IMetadataQueryContext context => FakeContext;
        ICommandHandler<CountryMetadataQuery,CountryMetadata> instance => new CountryMetadataQuery.Handler( context );

        public class Constructor : CountryMetadataQueryTests
        {
            [Fact]
            public void Requires_context()
            {
                FakeContext = null;
                Assert.Throws<ArgumentNullException>( nameof(context), ()=>instance );
            }

            [Fact]
            public void IsRegistered()
            {
                var actual = DependencyInjection.Container.GetInstance<ICommandHandler<CountryMetadataQuery, CountryMetadata>>();
                Assert.IsType<CountryMetadataQuery.Handler>( actual );
            }
        }

        public class Handle : CountryMetadataQueryTests
        {
            CountryMetadataQuery command = new CountryMetadataQuery { Country = "XW", Language = "en" };
            CancellationToken cancellationToken = CancellationToken.None;

            Task<CountryMetadata> invoke() => instance.Handle( command, cancellationToken );

            /// <summary>
            /// Cases where the country should not be matched.
            /// </summary>
            
            public static IEnumerable<object[]> NotMatchedCases()
            {
                var global = new GlobalMetadata();
                var key = "XW";
                
                // no global metadata
                yield return new object[] { null, null, null };

                // no key returned
                yield return new object[] { global, null, null };

                // no identifier returned
                yield return new object[] { global, key, null };
            }

            [Theory]
            [MemberData(nameof(NotMatchedCases))]
            public async Task WhenNotMatched_returns_null( GlobalMetadata global, string key, string id )
            {
                FakeContext.MockDispatcher.Setup( _=> _.Execute( GlobalMetadataQuery.Default, cancellationToken ) ).ReturnsAsync( global );
                FakeContext.MockBuilder.Setup( _=> _.GetChildKey( global, command.Country ) ).Returns( key );
                FakeContext.MockBuilder.Setup( _=> _.BuildIdentifier( global, key, command.Language ) ).Returns( id );

                var actual = await invoke();
                Assert.Null( actual );

                var getChildKeyTimes = global != null ? Times.Once() : Times.Never();
                var buildIdentifierTimes = key != null ? Times.Once() : Times.Never();
                FakeContext.MockDispatcher.Verify( _ => _.Execute( GlobalMetadataQuery.Default, cancellationToken ), Times.Once );
                FakeContext.MockBuilder.Verify( _ => _.GetChildKey( global, command.Country ), getChildKeyTimes );
                FakeContext.MockBuilder.Verify( _=> _.BuildIdentifier( global, key, command.Language ), buildIdentifierTimes );

                FakeContext.MockClient.Verify( _=> _.Query<CountryMetadata>( It.IsAny<string>() ), Times.Never );
            }

            /// <summary>
            /// Cases where the country is matched.
            /// </summary>
            
            public static IEnumerable<object[]> MatchedCases()
            {
                var global = new GlobalMetadata();
                var key = "XW";
                var id = "data/XW--en";

                yield return new object[] { global, key, id, null };
                yield return new object[] { global, key, id, new CountryMetadata() };
            }

            [Theory]
            [MemberData(nameof(MatchedCases))]
            public async Task WhenMatched_returns_clientResponse( GlobalMetadata global, string key, string id, CountryMetadata response )
            {
                FakeContext.MockDispatcher.Setup( _ => _.Execute( GlobalMetadataQuery.Default, cancellationToken ) ).ReturnsAsync( global ).Verifiable();
                FakeContext.MockBuilder.Setup( _ => _.GetChildKey( global, command.Country ) ).Returns( key ).Verifiable();
                FakeContext.MockBuilder.Setup( _ => _.BuildIdentifier( global, key, command.Language ) ).Returns( id ).Verifiable();
                FakeContext.MockClient.Setup( _ => _.Query<CountryMetadata>( id ) ).ReturnsAsync( response ).Verifiable();

                var actual = await invoke();
                Assert.Equal( response, actual );

                FakeContext.MockDispatcher.VerifyAll();
                FakeContext.MockBuilder.VerifyAll();
                FakeContext.MockClient.VerifyAll();
            }
        }
    }
}