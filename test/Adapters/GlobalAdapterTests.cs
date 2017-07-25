using Fidget.Validation.Addresses.Client;
using Fidget.Validation.Addresses.Metadata;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Adapters
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

        public static IEnumerable<object[]> QueryCases => new object[][]
        {
            new object[] { null },
            new object[] { new GlobalMetadata { Id = "data" } },
        };

        public class QueryAsync : GlobalAdapterTests
        {
            

            [Theory]
            [MemberData(nameof(QueryCases),MemberType =typeof(GlobalAdapterTests))]
            public async Task Returns_clientResponse( GlobalMetadata expected )
            {
                MockClient.Setup( _=> _.Query<GlobalMetadata>( "data" ) ).ReturnsAsync( expected ).Verifiable();

                var actual = await instance.QueryAsync();
                Assert.Equal( expected, actual );
                MockClient.VerifyAll();
            }
        }

        public class Query
        {
            Mock<IGlobalAdapter> MockAdapter = new Mock<IGlobalAdapter>();
            
            IGlobalAdapter adapter => MockAdapter?.Object;
            GlobalMetadata invoke() => adapter.Query();

            [Fact]
            public void Requires_adapter()
            {
                MockAdapter = null;
                Assert.Throws<ArgumentNullException>( nameof(adapter), ()=>invoke() );
            }

            [Theory]
            [MemberData( nameof( QueryCases ), MemberType = typeof( GlobalAdapterTests ) )]
            public void Returns_adapterResponse( GlobalMetadata expected )
            {
                MockAdapter.Setup( _=> _.QueryAsync() ).ReturnsAsync( expected ).Verifiable();

                var actual = invoke();
                Assert.Equal( expected, actual );
                MockAdapter.VerifyAll();
            }
        }
    }
}