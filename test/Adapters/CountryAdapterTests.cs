using Fidget.Validation.Addresses.Client;
using Fidget.Validation.Addresses.Metadata;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Adapters
{
    public class CountryAdapterTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();
        Mock<IGlobalAdapter> MockGlobal = new Mock<IGlobalAdapter>();
        Mock<IKeyService> MockKeyService = new Mock<IKeyService>();

        IServiceClient client => MockClient?.Object;
        IGlobalAdapter global => MockGlobal?.Object;
        IKeyService keyService => MockKeyService?.Object;

        ICountryAdapter instance => new CountryAdapter( client, global, keyService );

        public class Constructor : CountryAdapterTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof(client), ()=>instance );
            }

            [Fact]
            public void Requires_global()
            {
                MockGlobal = null;
                Assert.Throws<ArgumentNullException>( nameof(global), ()=>instance );
            }

            [Fact]
            public void Requires_keyService()
            {
                MockKeyService = null;
                Assert.Throws<ArgumentNullException>( nameof(keyService), ()=>instance );
            }
        }

        public class QueryDefaultAsync : CountryAdapterTests
        {
            async Task<CountryMetadata> invoke() => await instance.QueryDefaultAsync();

            [Fact]
            public async Task Returns_default()
            {
                var expected = new CountryMetadata();
                MockClient.Setup( _=> _.Query<CountryMetadata>( "data/ZZ" ) ).ReturnsAsync( expected ).Verifiable();

                var actual = await invoke();
                Assert.Equal( expected, actual );
                MockClient.VerifyAll();
            }
        }
         
        public class QueryDefault
        {
            Mock<ICountryAdapter> MockAdapter = new Mock<ICountryAdapter>();

            ICountryAdapter adapter => MockAdapter?.Object;
            
            [Fact]
            public void Requires_adapter()
            {
                MockAdapter = null;
                Assert.Throws<ArgumentNullException>( ()=> adapter.QueryDefault() );
            }

            public static IEnumerable<object[]> ResultCases => new object[][]
            {
                new object[] { null },
                new object[] { new CountryMetadata() },
            };

            [Theory]
            [MemberData(nameof(ResultCases))]
            public void Returns_adapterResponse( CountryMetadata result )
            {
                MockAdapter.Setup( _=> _.QueryDefaultAsync() ).ReturnsAsync( result ).Verifiable();

                var actual = adapter.QueryDefault();
                Assert.Equal( result, actual );
                MockAdapter.VerifyAll();
            }
        }

        public class QueryAsync : CountryAdapterTests
        {
            public static IEnumerable<object[]> NotMatchedCases()
            {
                var countries = new string[] { null, "XW" };
                var languages = new string[] { null, "en" };
                var globalMetas = new GlobalMetadata[] { null, new GlobalMetadata { Id = "data" } };
             
                return
                    from country in countries
                    from language in languages
                    from meta in globalMetas
                    select new object[] { meta, country, language };
            }

            [Theory]
            [MemberData(nameof(NotMatchedCases))]
            public async Task WhenNotMatched_returns_null( GlobalMetadata globalMeta, string country, string language )
            {
                string key = null;
                MockGlobal.Setup( _=> _.QueryAsync() ).ReturnsAsync( globalMeta ).Verifiable();
                MockKeyService.Setup( _=> _.TryGetCountryKey( globalMeta, country, out key ) ).Returns( false ).Verifiable();

                var actual = await instance.QueryAsync( country, language );
                Assert.Null( actual );
                MockGlobal.VerifyAll();
                MockKeyService.VerifyAll();
                MockClient.Verify( _=> _.Query<CountryMetadata>( It.IsAny<string>() ), Times.Never );
            }

            CountryMetadata defaults = new CountryMetadata
            {
                Id = "data/ZZ",
                Format = "%N%n%O%n%A%n%C",
                Required = new AddressField[] { AddressField.StreetAddress, AddressField.Locality },
                Uppercase = new AddressField[] { AddressField.Locality },
                StateType = "province",
                LocalityType = "city",
                SublocalityType = "suburb",
                PostalCodeType = "postal",
            };

            public static IEnumerable<object[]> MatchedCases()
            {
                var country = "xw";
                var languages = new string[] { null, "en" };

                // include fields that can be defaulted
                var results = new CountryMetadata[]
                {
                    new CountryMetadata { Format = Guid.NewGuid().ToString() },
                    new CountryMetadata { Required = new AddressField[] { AddressField.StreetAddress, AddressField.Locality, AddressField.Province } },
                    new CountryMetadata { Uppercase = new AddressField[] { AddressField.Locality, AddressField.Sublocality } },
                    new CountryMetadata { StateType = Guid.NewGuid().ToString() },
                    new CountryMetadata { LocalityType = Guid.NewGuid().ToString() },
                    new CountryMetadata { SublocalityType = Guid.NewGuid().ToString() },
                    new CountryMetadata { PostalCodeType = Guid.NewGuid().ToString() },
                    new CountryMetadata { },
                };

                return
                    from language in languages
                    from result in results
                    select new object[] { country, language, result };
            }

            [Theory]
            [MemberData(nameof(MatchedCases))]
            public async Task WhenMatched_returns_clientResponseWithDefaults( string country, string language, CountryMetadata result )
            {
                var key = "XW";
                var id = "data/XW";
                var globalMeta = new GlobalMetadata { Id = "data" };
                
                // mock setup
                MockGlobal.Setup( _=> _.QueryAsync() ).ReturnsAsync( globalMeta );
                MockKeyService.Setup( _=> _.TryGetCountryKey( globalMeta, country, out key ) ).Returns( true ).Verifiable();
                MockKeyService.Setup( _=> _.BuildIdentifier( globalMeta, key, language ) ).Returns( id ).Verifiable();
                MockClient.Setup( _=> _.Query<CountryMetadata>( "data/ZZ" ) ).ReturnsAsync( defaults ).Verifiable();
                MockClient.Setup( _=> _.Query<CountryMetadata>( id ) ).ReturnsAsync( result ).Verifiable();

                // for these properties, we use expect the default when the value is not defined on the country
                var format = result.Format ?? defaults.Format;
                var required = result.Required ?? defaults.Required;
                var uppercase = result.Uppercase ?? defaults.Uppercase;
                var stateType = result.StateType ?? defaults.StateType;
                var localityType = result.LocalityType ?? defaults.LocalityType;
                var sublocalityType = result.SublocalityType ?? defaults.SublocalityType;
                var postalCodeType = result.PostalCodeType ?? defaults.PostalCodeType;

                var actual = await instance.QueryAsync( country, language );
                Assert.Equal( result, actual );
                MockKeyService.VerifyAll();
                MockClient.VerifyAll();

                // check default values
                Assert.Equal( format, actual.Format );
                Assert.Equal( required, actual.Required );
                Assert.Equal( uppercase, actual.Uppercase );
                Assert.Equal( stateType, actual.StateType );
                Assert.Equal( localityType, actual.LocalityType );
                Assert.Equal( sublocalityType, actual.SublocalityType );
                Assert.Equal( postalCodeType, actual.PostalCodeType );
            }
        }

        public class Query
        {
            Mock<ICountryAdapter> MockAdapter = new Mock<ICountryAdapter>();

            ICountryAdapter adapter => MockAdapter?.Object;
            string country;
            string language;

            CountryMetadata invoke() => adapter.Query( country, language );

            [Fact]
            public void Requires_adapter()
            {
                MockAdapter = null;
                Assert.Throws<ArgumentNullException>( nameof(adapter), ()=> invoke() );
            }

            public static IEnumerable<object[]> ResultCases => new object[][]
            {
                new object[] { null, null, null },
                new object[] { null, "en", null },
                new object[] { "XW", null, null },
                new object[] { "XW", "en", null },
                new object[] { "XW", null, new CountryMetadata() },
                new object[] { "XW", "en", new CountryMetadata() },
            };

            [Theory]
            [MemberData(nameof(ResultCases))]
            public void Returns_adapterResponse( string country, string language, CountryMetadata result )
            {
                this.country = country;
                this.language = language;
                MockAdapter.Setup( _=> _.QueryAsync( country, language ) ).ReturnsAsync( result ).Verifiable();

                var actual = invoke();
                Assert.Equal( result, actual );
                MockAdapter.VerifyAll();
            }
        }
    }
}