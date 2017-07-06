using Fidget.Validation.Addresses.Service.Metadata.Internal;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Service
{
    public class ServiceClientFactoryTests
    {
        ServiceClient.Factory create() => new ServiceClient.Factory();

        [Fact]
        public async Task Returns_workingClient()
        {
            var factory = create();
            var actual = factory.Create();

            var global = await actual.Query<GlobalMetadata>( "data" );
            Assert.NotNull( global );
        }
    }
}