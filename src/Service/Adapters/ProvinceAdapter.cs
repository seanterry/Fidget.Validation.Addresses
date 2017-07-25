using Fidget.Validation.Addresses.Client;
using Fidget.Validation.Addresses.Metadata;
using System;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Service.Adapters
{
    /// <summary>
    /// Adapter for working with province metadata.
    /// </summary>

    class ProvinceAdapter : RegionalAdapter, IProvinceAdapter
    {
        /// <summary>
        /// Country adapter.
        /// </summary>
        
        readonly ICountryAdapter Country;

        /// <summary>
        /// Constructs an adapter for working with province metadata.
        /// </summary>
        /// <param name="client">Client that will back the current instance.</param>
        /// <param name="country">Country metadata adapter.</param>
        
        public ProvinceAdapter( IServiceClient client, ICountryAdapter country ) : base( client )
        {
            Country = country ?? throw new ArgumentNullException( nameof(country) );
        }

        /// <summary>
        /// Queries the service for the specified province.
        /// </summary>
        /// <param name="country"> Key of the country.</param>
        /// <param name="province">Key or name of the province.</param>
        /// <param name="language">Language for the metadata.</param>
        /// <returns>The specified province, if found; otherwise null.</returns>

        public async Task<ProvinceMetadata> Query( string country, string province, string language )
        {
            var countryMeta = await Country.Query( country, language );

            if ( Country.TryGetProvinceKey( countryMeta, province, out string key ) )
            {
                var id = BuildIdentifier( countryMeta.Id, key, language );
                return await Client.Query<ProvinceMetadata>( id );
            }

            else return null;
        }

        /// <summary>
        /// Returns whether the given locality value can be located in the province metadata.
        /// </summary>
        /// <param name="province">Province metadata (always returns false if not provided).</param>
        /// <param name="locality">Key or name of the locality (case insinsitive).</param>
        /// <param name="key">Key of the locality, if found.</param>
        
        public bool TryGetLocalityKey( ProvinceMetadata province, string locality, out string key ) =>
            TryGetChildKey( province, locality, out key );
    }
}