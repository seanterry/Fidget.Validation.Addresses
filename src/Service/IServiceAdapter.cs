using Fidget.Validation.Addresses.Service.Metadata;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Service
{
    /// <summary>
    /// Defines an adapter for interacting with the remote address service.
    /// </summary>

    public interface IServiceAdapter
    {
        /// <summary>
        /// Returns global-level address metadata.
        /// </summary>

        Task<IGlobalMetadata> GetGlobal();
    }
}