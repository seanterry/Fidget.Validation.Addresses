using Fidget.Validation.Addresses.Metadata;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Adapters
{
    /// <summary>
    /// Defines an adapter for querying global metadata.
    /// </summary>

    public interface IGlobalAdapter
    {
        /// <summary>
        /// Returns the global metadata entry.
        /// </summary>

        Task<GlobalMetadata> QueryAsync();
    }
}