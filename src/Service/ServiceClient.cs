using Fidget.Validation.Addresses.Service.Metadata;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Service
{
    /// <summary>
    /// Client for accessing the remote address data service.
    /// </summary>

    partial class ServiceClient : IServiceClient
    {
        /// <summary>
        /// Default service client instance.
        /// </summary>
        
        internal static readonly IServiceClient Default = new Factory().Create();

        /// <summary>
        /// Service root URI.
        /// </summary>
        
        const string Root = "https://chromium-i18n.appspot.com/ssl-address";

        /// <summary>
        /// Singleton HTTP client to use for all requests.
        /// </summary>
        
        static readonly HttpClient Client = new HttpClient();

        /// <summary>
        /// Queries the remote service and returns the response.
        /// </summary>
        /// <typeparam name="T">Type of the metadata response.</typeparam>
        /// <param name="id">Data record to return.</param>
        
        public async Task<T> Query<T>( string id ) where T: ICommonMetadata
        {
            var uri = $"{Root}/{id}";
            var json = await Client.GetStringAsync( uri );

            return JsonConvert.DeserializeObject<T>( json );
        }
    }
}