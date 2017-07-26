using Fidget.Validation.Addresses.Client;
using Fidget.Validation.Addresses.Metadata;
using System;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Adapters
{
    /// <summary>
    /// Adapter for querying province metadata.
    /// </summary>

    class ProvinceAdapter : IProvinceAdapter
    {
        /// <summary>
        /// Service client that backs the current instance.
        /// </summary>
        
        readonly IServiceClient Client;

        /// <summary>
        /// Country metadata adapter.
        /// </summary>
        
        readonly ICountryAdapter Country;

        /// <summary>
        /// Metadata key service.
        /// </summary>
        
        readonly IKeyService KeyService;

        /// <summary>
        /// Constructs an adapter for querying province metadata.
        /// </summary>
        /// <param name="client">Service client that will back the instance.</param>
        /// <param name="country">Country metadata adapter.</param>
        /// <param name="keyService">Metadata key service.</param>

        public ProvinceAdapter( IServiceClient client, ICountryAdapter country, IKeyService keyService )
        {
            Client = client ?? throw new ArgumentNullException( nameof(client) );
            Country = country ?? throw new ArgumentNullException( nameof(country) );
            KeyService = keyService ?? throw new ArgumentNullException( nameof(keyService) );
        }

        /// <summary>
        /// Returns the metadata for the specified province if found, otherwise null.
        /// </summary>
        /// <param name="country">Key of the country containing the province.</param>
        /// <param name="province">Key or name of the province.</param>
        /// <param name="language">Metadata language.</param>
        
        public async Task<ProvinceMetadata> QueryAsync( string country, string province, string language )
        {
            var countryMeta = await Country.QueryAsync( country, language );
            
            if ( !KeyService.TryGetChildKey( countryMeta, province, out string key ) ) return null;
            
            var id = KeyService.BuildIdentifier( countryMeta, key, language );
            var result = await Client.Query<ProvinceMetadata>( id );

            return result;
        }
    }
}