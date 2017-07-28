using Fidget.Commander;
using Fidget.Validation.Addresses.Metadata;
using Fidget.Validation.Addresses.Metadata.Commands;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses
{
    public class AddressServiceTests
    {
        Mock<ICommandDispatcher> MockDispatcher = new Mock<ICommandDispatcher>();

        ICommandDispatcher dispatcher => MockDispatcher?.Object;
        IAddressService instance => new AddressService( dispatcher );

        public class Constructor : AddressServiceTests
        {
            [Fact]
            public void Requires_dispatcher()
            {
                MockDispatcher = null;
                Assert.Throws<ArgumentNullException>( nameof(dispatcher), ()=>instance );
            }

            [Fact]
            public void IsRegistered()
            {
                var actual = DependencyInjection.Container.GetInstance<IAddressService>();
                Assert.IsType<AddressService>( actual );
            }
        }

        public class GetGlobalAsync : AddressServiceTests
        {
            CancellationToken cancellationToken = CancellationToken.None;

            Task<GlobalMetadata> invoke() => instance.GetGlobalAsync( cancellationToken );

            public static IEnumerable<object[]> ResultCases = new object[][]
            {
                new object[] { null },
                new object[] { new GlobalMetadata() },
            };

            [Theory]
            [MemberData(nameof(ResultCases))]
            public async Task Returns_commandResult( GlobalMetadata result )
            {
                MockDispatcher.Setup( _=> _.Execute( GlobalMetadataQuery.Default, cancellationToken ) ).ReturnsAsync( result );

                var actual = await invoke();
                Assert.Equal( result, actual );
            }
        }
    }
}
