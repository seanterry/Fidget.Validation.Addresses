using Fidget.Validation.Addresses.Metadata;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Service.Adapters
{
    public class GlobalAdapterTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();

        IServiceClient client => MockClient?.Object;
        IGlobalAdapter instance => new GlobalAdapter( client );

        public class Constructor : GlobalAdapterTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof(client), ()=>instance );
            }
        }

        public class Query : GlobalAdapterTests
        {
            public static IEnumerable<object[]> QueryTestCases() => new object[][]
            {
                new object[] { null },
                new object[] { new GlobalMetadata { Id = "data" } },
            };

            [Theory]
            [MemberData(nameof(QueryTestCases))]
            public async Task Returns_clientQuery( GlobalMetadata response )
            {
                MockClient.Setup( _=> _.Query<GlobalMetadata>( "data" ) ).ReturnsAsync( response as GlobalMetadata );

                var actual = await instance.Query();
                Assert.Equal( response, actual );
            }
        }

        public class TryGetCountryKey : GlobalAdapterTests
        {
            public static IEnumerable<object[]> CountryNotFoundCases()
            {
                var country = "XW";
                var globals = new GlobalMetadata[]
                {
                    null,
                    new GlobalMetadata { Countries = null },
                    new GlobalMetadata { Countries = new string[0] },
                    new GlobalMetadata { Countries = new string[] { "XA", "XB" } },
                };

                return
                    from global in globals
                    select new object[] { global, country };
            }

            [Theory]
            [MemberData(nameof(CountryNotFoundCases))]
            public void WhenCountryNotFound_returns_false( GlobalMetadata global, string country )
            {
                var found = instance.TryGetCountryKey( global, country, out string key );
                Assert.False( found );
                Assert.Null( key );
            }

            public static IEnumerable<object[]> CountryFoundCases()
            {
                var key = "XW";
                var countries = new string[] { "XW", "xw" };
                var globals = new GlobalMetadata[]
                {
                    new GlobalMetadata { Countries = new string[] { "XW" } },
                    new GlobalMetadata { Countries = new string[] { "XA", "XW", "XB" } },
                };

                return
                    from country in countries
                    from global in globals
                    select new object[] { global, country, key };
            }

            [Theory]
            [MemberData(nameof(CountryFoundCases))]
            public void WhenCountryFound_returns_true( GlobalMetadata global, string country, string expected )
            {
                var found = instance.TryGetCountryKey( global, country, out string actual );
                Assert.True( found );
                Assert.Equal( expected, actual );
            }
        }
    }
}