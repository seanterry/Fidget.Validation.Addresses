using Fidget.Commander;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Metadata.Commands
{
    public class ProvinceMetadataQueryTests
    {
        FakeContext FakeContext = new FakeContext();

        IMetadataQueryContext context => FakeContext;
        ICommandHandler<ProvinceMetadataQuery, ProvinceMetadata> instance => new ProvinceMetadataQuery.Handler( context );

        public class For
        {
            AddressData address;
            ProvinceMetadataQuery invoke() => ProvinceMetadataQuery.For( address );

            static string random() => Convert.ToBase64String( Guid.NewGuid().ToByteArray() );

            [Fact]
            public void Requires_address()
            {
                address = null;
                Assert.Throws<ArgumentNullException>( nameof( address ), () => invoke() );
            }

            [Theory]
            [MemberData( nameof( CountryMetadataQueryTests.For.QueryCases ), MemberType =typeof(CountryMetadataQueryTests.For) )]
            public void Returns_query( AddressData address )
            {
                var expected = new ProvinceMetadataQuery
                {
                    Country = address.Country,
                    Province = address.Province,
                    Language = address.Language,
                };

                this.address = address;
                var actual = invoke();
                Assert.Equal( expected, actual );
            }
        }

        public class Constructor : ProvinceMetadataQueryTests
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
                var actual = DependencyInjection.Container.GetInstance<ICommandHandler<ProvinceMetadataQuery, ProvinceMetadata>>();
                Assert.IsType<ProvinceMetadataQuery.Handler>( actual );
            }
        }

        public class Handle : ProvinceMetadataQueryTests
        {
            ProvinceMetadataQuery command = new ProvinceMetadataQuery { Country = "XW", Province = "XY", Language = "en"  };
            CancellationToken cancellationToken = CancellationToken.None;

            Task<ProvinceMetadata> invoke() => instance.Handle( command, cancellationToken );

            /// <summary>
            /// Cases where the province should not be matched.
            /// </summary>

            public static IEnumerable<object[]> NotMatchedCases()
            {
                var parent = new CountryMetadata();
                var key = "XX";

                // no parent metadata
                yield return new object[] { null, null, null };

                // no key returned
                yield return new object[] { parent, null, null };

                // no identifier returned
                yield return new object[] { parent, key, null };
            }

            [Theory]
            [MemberData( nameof( NotMatchedCases ) )]
            public async Task WhenNotMatched_returns_null( CountryMetadata country, string key, string id )
            {
                var parentQuery = new CountryMetadataQuery 
                { 
                    Country = command.Country,
                    Language = command.Language,
                };

                FakeContext.MockDispatcher.Setup( _ => _.Execute( parentQuery, cancellationToken ) ).ReturnsAsync( country );
                FakeContext.MockBuilder.Setup( _ => _.GetChildKey( country, command.Province ) ).Returns( key );
                FakeContext.MockBuilder.Setup( _ => _.BuildIdentifier( country, key, command.Language ) ).Returns( id );

                var actual = await invoke();
                Assert.Null( actual );

                var getChildKeyTimes = country != null ? Times.Once() : Times.Never();
                var buildIdentifierTimes = key != null ? Times.Once() : Times.Never();
                FakeContext.MockDispatcher.Verify( _ => _.Execute( parentQuery, cancellationToken ), Times.Once );
                FakeContext.MockBuilder.Verify( _ => _.GetChildKey( country, command.Province ), getChildKeyTimes );
                FakeContext.MockBuilder.Verify( _ => _.BuildIdentifier( country, key, command.Language ), buildIdentifierTimes );

                FakeContext.MockClient.Verify( _ => _.Query<ProvinceMetadata>( It.IsAny<string>() ), Times.Never );
            }

            /// <summary>
            /// Cases where the province is matched.
            /// </summary>

            public static IEnumerable<object[]> MatchedCases()
            {
                var parent = new CountryMetadata();
                var key = "XX";
                var id = "data/XW/XX--en";

                yield return new object[] { parent, key, id, null };
                yield return new object[] { parent, key, id, new ProvinceMetadata() };
            }

            [Theory]
            [MemberData( nameof( MatchedCases ) )]
            public async Task WhenMatched_returns_clientResponse( CountryMetadata parent, string key, string id, ProvinceMetadata response )
            {
                var parentQuery = new CountryMetadataQuery
                {
                    Country = command.Country,
                    Language = command.Language,
                };

                FakeContext.MockDispatcher.Setup( _ => _.Execute( parentQuery, cancellationToken ) ).ReturnsAsync( parent ).Verifiable();
                FakeContext.MockBuilder.Setup( _ => _.GetChildKey( parent, command.Province ) ).Returns( key ).Verifiable();
                FakeContext.MockBuilder.Setup( _ => _.BuildIdentifier( parent, key, command.Language ) ).Returns( id ).Verifiable();
                FakeContext.MockClient.Setup( _ => _.Query<ProvinceMetadata>( id ) ).ReturnsAsync( response ).Verifiable();

                var actual = await invoke();
                Assert.Equal( response, actual );

                FakeContext.MockDispatcher.VerifyAll();
                FakeContext.MockBuilder.VerifyAll();
                FakeContext.MockClient.VerifyAll();
            }
        }
    }
}