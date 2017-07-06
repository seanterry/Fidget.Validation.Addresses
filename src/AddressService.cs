using Fidget.Validation.Addresses.Service;
using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using System;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses
{
    /// <summary>
    /// Service for address validation and metadata exploration.
    /// </summary>

    public class AddressService : IAddressService
    {
        /// <summary>
        /// Default service client factory.
        /// </summary>
        
        static readonly ServiceClient.Factory ServiceClientFactory = new ServiceClient.Factory();

        /// <summary>
        /// Remote address service client.
        /// </summary>
        
        readonly IServiceClient Client;

        /// <summary>
        /// Constructs a service for address validation and metadata exploration.
        /// </summary>
        /// <param name="client">Remote address service client.</param>
        
        internal AddressService( IServiceClient client )
        {
            Client = client ?? throw new ArgumentNullException( nameof(client) );
        }

        /// <summary>
        /// Constructs a service for address validation and metadata exploration.
        /// </summary>
        
        public AddressService() : this( ServiceClient.Default ) {}

        /// <summary>
        /// Returns gobal metadata information.
        /// </summary>
        
        public async Task<IGlobalMetadata> GetGlobalAsync() => await Client.Query<GlobalMetadata>( "data" );

        /// <summary>
        /// Returns metadata for the specified country if it is available.
        /// </summary>
        /// <param name="countryKey">Key of the country to return.</param>
        /// <param name="language">Language code for the metadata to return.</param>

        public async Task<ICountryMetadata> GetCountryAsync( string countryKey, string language )
        {
            if ( countryKey == null ) throw new ArgumentNullException( nameof( countryKey ) );

            var id = $"data/{countryKey}{(language != null ? $"--{language}" : string.Empty)}";

            var defaults = await Client.Query<CountryMetadata>( "data/ZZ" );
            var result = await Client.Query<CountryMetadata>( id );

            if ( result != null && defaults != null )
            {
                result.Format = result.Format ?? defaults.Format;
                result.Required = result.Required ?? defaults.Required;
                result.Uppercase = result.Uppercase ?? defaults.Uppercase;
                result.StateType = result.StateType ?? defaults.StateType;
                result.LocalityType = result.LocalityType ?? defaults.LocalityType;
                result.SublocalityType = result.SublocalityType ?? defaults.SublocalityType;
                result.PostalCodeType = result.PostalCodeType ?? defaults.PostalCodeType;
            }
            
            return result;
        }

        /// <summary>
        /// Returns metadata for the specified province if it is available.
        /// </summary>
        /// <param name="countryKey">Key of the parent country.</param>
        /// <param name="provinceKey">Key of the province to return.</param>
        /// <param name="language">Language code for the metadata to return.</param>
        
        public async Task<IProvinceMetadata> GetProvinceAsync( string countryKey, string provinceKey, string language )
        {
            if ( countryKey == null ) throw new ArgumentNullException( nameof( countryKey ) );
            if ( provinceKey == null ) throw new ArgumentNullException( nameof( provinceKey ) );

            var id = $"data/{countryKey}/{provinceKey}{( language != null ? $"--{language}" : string.Empty )}";
            
            return await Client.Query<ProvinceMetadata>( id );
        }

        /// <summary>
        /// Returns metadata for the specified locality if it is available.
        /// </summary>
        /// <param name="countryKey">Key of the parent country.</param>
        /// <param name="provinceKey">Key of the parent province.</param>
        /// <param name="localityKey">Key of the locality to return.</param>
        /// <param name="language">Language code for the metadata to return.</param>
        
        public async Task<ILocalityMetadata> GetLocalityAsync( string countryKey, string provinceKey, string localityKey, string language )
        {
            if ( countryKey == null ) throw new ArgumentNullException( nameof( countryKey ) );
            if ( provinceKey == null ) throw new ArgumentNullException( nameof( provinceKey ) );
            if ( localityKey == null ) throw new ArgumentNullException( nameof( localityKey ) );

            var id = $"data/{countryKey}/{provinceKey}/{localityKey}{(language != null ? $"--{language}" : string.Empty)}";

            return await Client.Query<LocalityMetadata>( id );
        }
    }
}