using Fidget.Commander;
using Fidget.Extensions.Reflection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public class For
        {
            AddressData address;
            CountryMetadataQuery invoke() => CountryMetadataQuery.For( address );
            
            static string random() => Convert.ToBase64String( Guid.NewGuid().ToByteArray() );

            [Fact]
            public void Requires_address()
            {
                address = null;
                Assert.Throws<ArgumentNullException>( nameof(address), ()=>invoke() );
            }

            public static IEnumerable<object[]> QueryCases()
            {
                var countries = new string[] { null, random() };
                var provinces = new string[] { null, random() };
                var localities = new string[] { null, random() };
                var sublocalities = new string[] { null, random() };
                var languages = new string[] { null, random() };

                return
                    from country in countries
                    from province in provinces
                    from locality in localities
                    from sublocality in sublocalities
                    from language in languages
                    select new object[] { new AddressData 
                    { 
                        Country = country, 
                        Province = province,
                        Locality = locality,
                        Sublocality = sublocality,
                        Language = language 
                    }};
            }

            [Theory]
            [MemberData(nameof(QueryCases))]
            public void Returns_query( AddressData address )
            {
                var expected = new CountryMetadataQuery
                {
                    Country = address.Country,
                    Language = address.Language,
                };
                
                this.address = address;
                var actual = invoke();
                Assert.Equal( expected, actual );
            }
        }

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