using Fidget.Commander;
using Fidget.Validation.Addresses.Client;

namespace Fidget.Validation.Addresses.Metadata.Commands
{
    /// <summary>
    /// Defines a context for common metadata query dependencies.
    /// </summary>

    public interface IMetadataQueryContext
    {
        /// <summary>
        /// Gets the remote address service client.
        /// </summary>

        IServiceClient Client { get; }

        /// <summary>
        /// Gets the key/identifier builder.
        /// </summary>

        IKeyBuilder Builder { get; }

        /// <summary>
        /// Gets the command dispatcher.
        /// </summary>

        ICommandDispatcher Dispatcher { get; }
    }
}