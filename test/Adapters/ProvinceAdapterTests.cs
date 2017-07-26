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
    public class ProvinceAdapterTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();
        Mock<ICountryAdapter> MockCountry = new Mock<ICountryAdapter>();
        Mock<IKeyService> MockKeyService = new Mock<IKeyService>();

        IServiceClient client => MockClient?.Object;
        ICountryAdapter country => MockCountry?.Object;
        IKeyService keyService => MockKeyService?.Object;

        IProvinceAdapter instance => new ProvinceAdapter( client, country, keyService );

        public class Constructor : ProvinceAdapterTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof(client), ()=>instance );
            }

            [Fact]
            public void Requires_country()
            {
                MockCountry = null;
                Assert.Throws<ArgumentNullException>( nameof( country ), () => instance );
            }

            [Fact]
            public void Requires_keyService()
            {
                MockKeyService = null;
                Assert.Throws<ArgumentNullException>( nameof( keyService ), () => instance );
            }
        }

        public class QueryAsync : ProvinceAdapterTests
        {
            public static IEnumerable<object[]> NotMatchedCases => new object[][]
            {
                new object[] { null, null, null, null },
                new object[] { "XW", null, null, null },
                new object[] { "XW", null, null, new CountryMetadata { Id = "data/XW" } },
                new object[] { "XW", "XX", null, null },
                new object[] { "XW", "XX", null, new CountryMetadata { Id = "data/XW" } },
                new object[] { "XW", "XX", "en", null },
                new object[] { "XW", "XX", "en", new CountryMetadata { Id = "data/XW--en" } },
            };

            [Theory]
            [MemberData(nameof(NotMatchedCases))]
            public async Task WhenNotMatched_returnsNull( string country, string province, string language, CountryMetadata countryMeta )
            {
                string key = null;
                MockCountry.Setup( _=> _.QueryAsync( country, language ) ).ReturnsAsync( countryMeta ).Verifiable();
                MockKeyService.Setup( _=> _.TryGetChildKey( countryMeta, province, out key ) ).Returns( false ).Verifiable();

                var actual = await instance.QueryAsync( country, province, language );
                Assert.Null( actual );
                MockCountry.VerifyAll();
                MockKeyService.VerifyAll();
                MockClient.Verify( _=> _.Query<ProvinceMetadata>( It.IsAny<string>() ), Times.Never );
            }

            public static IEnumerable<object[]> MatchedCases()
            {
                var countries = new string[] { "XA", "XB" };
                var provinces = new string[] { "XW", "XX" };
                var languages = new string[] { null, "en" };

                return
                    from country in countries
                    from province in provinces
                    from language in languages
                    select new object[] { country, province, language, new ProvinceMetadata() };
            }

            [Theory]
            [MemberData(nameof(MatchedCases))]
            public async Task WhenMatched_returnsClientResult( string country, string province, string language, ProvinceMetadata result )
            {
                var countryMeta = new CountryMetadata();
                var key = Guid.NewGuid().ToString();
                var id = $"data/{country}/{key}";
                MockCountry.Setup( _=> _.QueryAsync( country, language ) ).ReturnsAsync( countryMeta ).Verifiable();
                MockKeyService.Setup( _=> _.TryGetChildKey( countryMeta, province, out key ) ).Returns( true ).Verifiable();
                MockKeyService.Setup( _=> _.BuildIdentifier( countryMeta, key, language ) ).Returns( id ).Verifiable();
                MockClient.Setup( _=> _.Query<ProvinceMetadata>( id ) ).ReturnsAsync( result ).Verifiable();

                var actual = await instance.QueryAsync( country, province, language );
                Assert.Equal( result, actual );
                MockCountry.VerifyAll();
                MockKeyService.VerifyAll();
                MockClient.VerifyAll();
            }
        }

        public class Query
        {
            Mock<IProvinceAdapter> MockAdapter = new Mock<IProvinceAdapter>();

            IProvinceAdapter adapter => MockAdapter?.Object;
            string country;
            string province;
            string language;

            ProvinceMetadata invoke() => adapter.Query( country, province, language );

            [Fact]
            public void Requires_adapter()
            {
                MockAdapter = null;
                Assert.Throws<ArgumentNullException>( nameof(adapter), ()=> invoke() );
            }

            public static IEnumerable<object[]> QueryCases => new object[][]
            {
                new object[] { "XW", "XX", null, null },
                new object[] { "XW", "XX", "en", null },
                new object[] { "XW", "XX", null, new ProvinceMetadata() },
                new object[] { "XW", "XX", "en", new ProvinceMetadata() },
            };

            [Theory]
            [MemberData(nameof(QueryCases))]
            public void Returns_adapterResponse( string country, string province, string language, ProvinceMetadata result )
            {
                this.country = country;
                this.province = province;
                this.language = language;
                MockAdapter.Setup( _=> _.QueryAsync( country, province, language ) ).ReturnsAsync( result ).Verifiable();

                var actual = invoke();
                Assert.Equal( result, actual );
                MockAdapter.VerifyAll();
            }
        }
    }
}
