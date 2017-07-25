using Fidget.Validation.Addresses.Metadata;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Client
{
    /// <summary>
    /// Client for accessing the remote address data service.
    /// </summary>

    class ServiceClient : IServiceClient
    {
        /// <summary>
        /// Root URI for the google address data service.
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
        
        public async Task<T> Query<T>( string id ) where T: CommonMetadata
        {
            var uri = $"{Root}/{id}";
            var json = await Client.GetStringAsync( uri );

            return JsonConvert.DeserializeObject<T>( json );
        }
    }
}