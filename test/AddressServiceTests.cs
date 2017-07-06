using Fidget.Validation.Addresses.Service;
using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses
{
    public class AddressServiceTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();
        IServiceClient client => MockClient?.Object;

        IAddressService create() => new AddressService( client );

        public class Constructor : AddressServiceTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof(client), ()=>create() );
            }

            [Fact]
            public void Implements_IAddressService()
            {
                var actual = create();
                Assert.IsType<AddressService>( actual );
                Assert.IsAssignableFrom<IAddressService>( actual );
            }
        }

        public class GetGlobal : AddressServiceTests
        {
            /// <summary>
            /// Client response setup.
            /// </summary>

            public static IEnumerable<object[]> GetGlobalResponses()
            {
                yield return new object[] { new GlobalMetadata { Id = "data" } };
                yield return new object[] { default( GlobalMetadata ) };
            }

            IGlobalMetadata invoke( IAddressService instance ) => instance.GetGlobal();

            [Theory]
            [MemberData( nameof( GetGlobalResponses ) )]
            public void Returns_valueFromClient( IGlobalMetadata expected )
            {
                MockClient.Setup( _ => _.Query<GlobalMetadata>( "data" ) ).ReturnsAsync( (GlobalMetadata)expected );
                var instance = create();
                var actual = invoke( instance );

                Assert.Equal( expected, actual );
            }
        }

        public class GetGlobalAsync : AddressServiceTests
        {
            /// <summary>
            /// Client response setup.
            /// </summary>
            
            public static IEnumerable<object[]> GetGlobalResponses()
            {
                yield return new object[] { new GlobalMetadata { Id = "data" } };
                yield return new object[] { default(GlobalMetadata) };
            }

            async Task<IGlobalMetadata> invoke( IAddressService instance ) => await instance.GetGlobalAsync();

            [Theory]
            [MemberData(nameof(GetGlobalResponses))]
            public async Task Returns_valueFromClient( IGlobalMetadata expected )
            {
                MockClient.Setup( _=> _.Query<GlobalMetadata>( "data" ) ).ReturnsAsync( (GlobalMetadata)expected );
                var instance = create();
                var actual = await invoke( instance );

                Assert.Equal( expected, actual );
            }
        }
    }
}