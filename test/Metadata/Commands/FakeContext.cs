using Fidget.Commander;
using Fidget.Validation.Addresses.Client;
using Moq;

namespace Fidget.Validation.Addresses.Metadata.Commands
{
    /// <summary>
    /// Fake metadata query context for testing.
    /// </summary>

    class FakeContext : IMetadataQueryContext
    {
        public Mock<IServiceClient> MockClient { get; } = new Mock<IServiceClient>();
        public Mock<IKeyBuilder> MockBuilder { get; } = new Mock<IKeyBuilder>();
        public Mock<ICommandDispatcher> MockDispatcher { get; } = new Mock<ICommandDispatcher>();

        public IServiceClient Client => MockClient.Object;
        public IKeyBuilder Builder => MockBuilder.Object;
        public ICommandDispatcher Dispatcher => MockDispatcher.Object;
    }
}