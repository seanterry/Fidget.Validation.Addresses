using Fidget.Commander;
using Fidget.Validation.Addresses.Client;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Metadata.Commands
{
    public class GlobalMetadataQueryTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();

        IServiceClient client => MockClient?.Object;
        ICommandHandler<GlobalMetadataQuery,GlobalMetadata> instance => new GlobalMetadataQuery.Handler( client );
        
        public class Registration
        {
            [Fact]
            public void IsRegistered()
            {
                var actual = DependencyInjection.Container.GetInstance<ICommandHandler<GlobalMetadataQuery, GlobalMetadata>>();
                Assert.IsType<GlobalMetadataQuery.Handler>( actual );
            }
        }

        public class Constructor : GlobalMetadataQueryTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof(client), ()=>instance );
            }
        }

        public class Handle : GlobalMetadataQueryTests
        {
            GlobalMetadataQuery command;
            Task<GlobalMetadata> invoke() => instance.Handle( command, CancellationToken.None );

            public static IEnumerable<object[]> ClientResponses => new object[][]
            {
                new object[] { null },
                new object[] { new GlobalMetadata() },
            };

            [Theory]
            [MemberData(nameof(ClientResponses))]
            public async Task Returns_clientResponse( GlobalMetadata response )
            {
                MockClient.Setup( _=> _.Query<GlobalMetadata>( "data" ) ).ReturnsAsync( response ).Verifiable();

                command = GlobalMetadataQuery.Default;
                var actual = await invoke();
                Assert.Equal( response, actual );

                MockClient.VerifyAll();
            }
        }
    }
}