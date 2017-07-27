using Fidget.Validation.Addresses.Metadata;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Client
{
    /// <summary>
    /// Defines a client for accessing the remote address data service.
    /// </summary>

    public interface IServiceClient
    {
        /// <summary>
        /// Queries the remote service and returns the response.
        /// </summary>
        /// <typeparam name="T">Type of the metadata response.</typeparam>
        /// <param name="id">Data record to return.</param>

        Task<T> Query<T>( string id ) where T : CommonMetadata;
    }
}