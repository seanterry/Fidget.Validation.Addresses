using Fidget.Validation.Addresses.Client;
using Fidget.Validation.Addresses.Metadata;
using System;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Adapters
{
    /// <summary>
    /// Adapter for querying locality metadata.
    /// </summary>

    class LocalityAdapter : ILocalityAdapter
    {
        /// <summary>
        /// Service client that backs the current instance.
        /// </summary>

        readonly IServiceClient Client;

        /// <summary>
        /// Province metadata adapter.
        /// </summary>

        readonly IProvinceAdapter Province;

        /// <summary>
        /// Metadata key service.
        /// </summary>

        readonly IKeyService KeyService;

        /// <summary>
        /// Constructs an adapter for querying locality metadata.
        /// </summary>
        /// <param name="client">Service client that will back the instance.</param>
        /// <param name="province">Province metadata adapter.</param>
        /// <param name="keyService">Metadata key service.</param>

        public LocalityAdapter( IServiceClient client, IProvinceAdapter province, IKeyService keyService )
        {
            Client = client ?? throw new ArgumentNullException( nameof( client ) );
            Province = province ?? throw new ArgumentNullException( nameof( province ) );
            KeyService = keyService ?? throw new ArgumentNullException( nameof( keyService ) );
        }

        /// <summary>
        /// Returns the metadata for the specified province if found, otherwise null.
        /// </summary>
        /// <param name="country">Key of the containing country.</param>
        /// <param name="province">Key or name of the containing province.</param>
        /// <param name="locality">Key or name of the locality.</param>
        /// <param name="language">Metadata language.</param>

        public async Task<LocalityMetadata> QueryAsync( string country, string province, string locality, string language )
        {
            var parent = await Province.QueryAsync( country, province, language );

            if ( !KeyService.TryGetChildKey( parent, province, out string key ) ) return null;

            var id = KeyService.BuildIdentifier( parent, key, language );
            var result = await Client.Query<LocalityMetadata>( id );

            return result;
        }
    }
}