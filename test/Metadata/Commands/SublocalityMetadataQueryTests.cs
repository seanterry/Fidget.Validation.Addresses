using Fidget.Commander;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Metadata.Commands
{
    public class SublocalityMetadataQueryTests
    {
        FakeContext FakeContext = new FakeContext();

        IMetadataQueryContext context => FakeContext;
        ICommandHandler<SublocalityMetadataQuery, SublocalityMetadata> instance => new SublocalityMetadataQuery.Handler( context );

        public class Constructor : SublocalityMetadataQueryTests
        {
            [Fact]
            public void Requires_context()
            {
                FakeContext = null;
                Assert.Throws<ArgumentNullException>( nameof( context ), () => instance );
            }

            [Fact]
            public void IsRegistered()
            {
                var actual = DependencyInjection.Container.GetInstance<ICommandHandler<SublocalityMetadataQuery, SublocalityMetadata>>();
                Assert.IsType<SublocalityMetadataQuery.Handler>( actual );
            }
        }

        public class Handle : SublocalityMetadataQueryTests
        {
            SublocalityMetadataQuery command = new SublocalityMetadataQuery { Country = "XW", Province = "XX", Locality = "XY", Sublocality = "XZ", Language = "en" };
            CancellationToken cancellationToken = CancellationToken.None;

            Task<SublocalityMetadata> invoke() => instance.Handle( command, cancellationToken );

            /// <summary>
            /// Cases where the sublocality should not be matched.
            /// </summary>

            public static IEnumerable<object[]> NotMatchedCases()
            {
                var parent = new LocalityMetadata();
                var key = "XZ";

                // no parent metadata
                yield return new object[] { null, null, null };

                // no key returned
                yield return new object[] { parent, null, null };

                // no identifier returned
                yield return new object[] { parent, key, null };
            }

            [Theory]
            [MemberData( nameof( NotMatchedCases ) )]
            public async Task WhenNotMatched_returns_null( LocalityMetadata parent, string key, string id )
            {
                var parentQuery = new LocalityMetadataQuery
                {
                    Country = command.Country,
                    Province = command.Province,
                    Locality = command.Locality,
                    Language = command.Language,
                };

                FakeContext.MockDispatcher.Setup( _ => _.Execute( parentQuery, cancellationToken ) ).ReturnsAsync( parent );
                FakeContext.MockBuilder.Setup( _ => _.GetChildKey( parent, command.Sublocality ) ).Returns( key );
                FakeContext.MockBuilder.Setup( _ => _.BuildIdentifier( parent, key, command.Language ) ).Returns( id );

                var actual = await invoke();
                Assert.Null( actual );

                var getChildKeyTimes = parent != null ? Times.Once() : Times.Never();
                var buildIdentifierTimes = key != null ? Times.Once() : Times.Never();
                FakeContext.MockDispatcher.Verify( _ => _.Execute( parentQuery, cancellationToken ), Times.Once );
                FakeContext.MockBuilder.Verify( _ => _.GetChildKey( parent, command.Sublocality ), getChildKeyTimes );
                FakeContext.MockBuilder.Verify( _ => _.BuildIdentifier( parent, key, command.Language ), buildIdentifierTimes );

                FakeContext.MockClient.Verify( _ => _.Query<SublocalityMetadata>( It.IsAny<string>() ), Times.Never );
            }

            /// <summary>
            /// Cases where the locality is matched.
            /// </summary>

            public static IEnumerable<object[]> MatchedCases()
            {
                var parent = new LocalityMetadata();
                var key = "XZ";
                var id = "data/XW/XX/XY/XZ--en";

                yield return new object[] { parent, key, id, null };
                yield return new object[] { parent, key, id, new SublocalityMetadata() };
            }

            [Theory]
            [MemberData( nameof( MatchedCases ) )]
            public async Task WhenMatched_returns_clientResponse( LocalityMetadata parent, string key, string id, SublocalityMetadata response )
            {
                var parentQuery = new LocalityMetadataQuery
                {
                    Country = command.Country,
                    Province = command.Province,
                    Locality = command.Locality,
                    Language = command.Language,
                };

                FakeContext.MockDispatcher.Setup( _ => _.Execute( parentQuery, cancellationToken ) ).ReturnsAsync( parent ).Verifiable();
                FakeContext.MockBuilder.Setup( _ => _.GetChildKey( parent, command.Sublocality ) ).Returns( key ).Verifiable();
                FakeContext.MockBuilder.Setup( _ => _.BuildIdentifier( parent, key, command.Language ) ).Returns( id ).Verifiable();
                FakeContext.MockClient.Setup( _ => _.Query<SublocalityMetadata>( id ) ).ReturnsAsync( response ).Verifiable();

                var actual = await invoke();
                Assert.Equal( response, actual );

                FakeContext.MockDispatcher.VerifyAll();
                FakeContext.MockBuilder.VerifyAll();
                FakeContext.MockClient.VerifyAll();
            }
        }
    }
}
