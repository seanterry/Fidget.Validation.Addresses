using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Service
{
    public class ServiceAdapterTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();
        IServiceClient client => MockClient?.Object;

        IServiceAdapter create() => new ServiceAdapter( client );

        public class Constructor : ServiceAdapterTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof(client), ()=>create() );
            }
        }

        public class GetGlobal : ServiceAdapterTests
        {
            async Task<IGlobalMetadata> invoke( IServiceAdapter instance ) => await instance.GetGlobal();

            [Fact]
            public async Task Returns_metadataFromClient()
            {
                var instance = create();
                var expected = new GlobalMetadata();
                MockClient.Setup( _=> _.Query<GlobalMetadata>( "data" ) ).ReturnsAsync( expected );

                var actual = await invoke( instance );
                Assert.Same( expected, actual );
            }
        }
    }
}