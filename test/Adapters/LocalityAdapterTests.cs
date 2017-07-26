using Fidget.Validation.Addresses.Client;
using Fidget.Validation.Addresses.Metadata;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Adapters
{
    public class LocalityAdapterTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();
        Mock<IProvinceAdapter> MockProvince = new Mock<IProvinceAdapter>();
        Mock<IKeyService> MockKeyService = new Mock<IKeyService>();

        IServiceClient client => MockClient?.Object;
        IProvinceAdapter province => MockProvince?.Object;
        IKeyService keyService => MockKeyService?.Object;

        ILocalityAdapter instance => new LocalityAdapter( client, province, keyService );

        public class Constructor : LocalityAdapterTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof( client ), () => instance );
            }

            [Fact]
            public void Requires_province()
            {
                MockProvince = null;
                Assert.Throws<ArgumentNullException>( nameof( province ), () => instance );
            }

            [Fact]
            public void Requires_keyService()
            {
                MockKeyService = null;
                Assert.Throws<ArgumentNullException>( nameof( keyService ), () => instance );
            }
        }

        public class QueryAsync : LocalityAdapterTests
        {
            public static IEnumerable<object[]> NotMatchedCases => new object[][]
            {
                new object[] { null, null, null, null, null },
                new object[] { "XW", null, null, null, null },
                new object[] { "XW", "XX", null, null, null },
                new object[] { "XW", "XX", null, null, new ProvinceMetadata { Id = "data/XW/XX" } },
                new object[] { "XW", "XX", "XY", null, null },
                new object[] { "XW", "XX", "XY", null, new ProvinceMetadata { Id = "data/XW/XX" } },
                new object[] { "XW", "XX", "XY", "en", null },
                new object[] { "XW", "XX", "XY", "en", new ProvinceMetadata { Id = "data/XW/XX--en" } },
            };

            [Theory]
            [MemberData( nameof( NotMatchedCases ) )]
            public async Task WhenNotMatched_returnsNull( string country, string province, string locality, string language, ProvinceMetadata provinceMeta )
            {
                string key = null;
                MockProvince.Setup( _ => _.QueryAsync( country, province, language ) ).ReturnsAsync( provinceMeta ).Verifiable();
                MockKeyService.Setup( _ => _.TryGetChildKey( provinceMeta, locality, out key ) ).Returns( false ).Verifiable();

                var actual = await instance.QueryAsync( country, province, locality, language );
                Assert.Null( actual );
                MockProvince.VerifyAll();
                MockKeyService.VerifyAll();
                MockClient.Verify( _ => _.Query<ProvinceMetadata>( It.IsAny<string>() ), Times.Never );
            }

            public static IEnumerable<object[]> MatchedCases()
            {
                var countries = new string[] { "XA", "XB" };
                var provinces = new string[] { "XC", "XD" };
                var localities = new string[] { "XX", "XY" };
                var languages = new string[] { null, "en" };

                return
                    from country in countries
                    from province in provinces
                    from locality in localities
                    from language in languages
                    select new object[] { country, province, locality, language, new LocalityMetadata() };
            }

            [Theory]
            [MemberData( nameof( MatchedCases ) )]
            public async Task WhenMatched_returnsClientResult( string country, string province, string locality, string language, LocalityMetadata result )
            {
                var provinceMeta = new ProvinceMetadata();
                var key = Guid.NewGuid().ToString();
                var id = $"data/{key}";
                MockProvince.Setup( _ => _.QueryAsync( country, province, language ) ).ReturnsAsync( provinceMeta ).Verifiable();
                MockKeyService.Setup( _ => _.TryGetChildKey( provinceMeta, locality, out key ) ).Returns( true ).Verifiable();
                MockKeyService.Setup( _ => _.BuildIdentifier( provinceMeta, key, language ) ).Returns( id ).Verifiable();
                MockClient.Setup( _ => _.Query<LocalityMetadata>( id ) ).ReturnsAsync( result ).Verifiable();

                var actual = await instance.QueryAsync( country, province, locality, language );
                Assert.Equal( result, actual );
                MockProvince.VerifyAll();
                MockKeyService.VerifyAll();
                MockClient.VerifyAll();
            }
        }

        public class Query
        {
            Mock<ILocalityAdapter> MockAdapter = new Mock<ILocalityAdapter>();

            ILocalityAdapter adapter => MockAdapter?.Object;
            string country;
            string province;
            string locality;
            string language;

            LocalityMetadata invoke() => adapter.Query( country, province, locality, language );

            [Fact]
            public void Requires_adapter()
            {
                MockAdapter = null;
                Assert.Throws<ArgumentNullException>( nameof( adapter ), () => invoke() );
            }

            public static IEnumerable<object[]> QueryCases => new object[][]
            {
                new object[] { "XW", "XX", "XY", null, null },
                new object[] { "XW", "XX", "XY", "en", null },
                new object[] { "XW", "XX", "XY", null, new LocalityMetadata() },
                new object[] { "XW", "XX", "XY", "en", new LocalityMetadata() },
            };

            [Theory]
            [MemberData( nameof( QueryCases ) )]
            public void Returns_adapterResponse( string country, string province, string locality, string language, LocalityMetadata result )
            {
                this.country = country;
                this.province = province;
                this.locality = locality;
                this.language = language;
                MockAdapter.Setup( _ => _.QueryAsync( country, province, locality, language ) ).ReturnsAsync( result ).Verifiable();

                var actual = invoke();
                Assert.Equal( result, actual );
                MockAdapter.VerifyAll();
            }
        }
    }
}