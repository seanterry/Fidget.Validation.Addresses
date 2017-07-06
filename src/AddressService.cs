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
        /// Returns metadata for the specified country.
        /// </summary>
        /// <param name="countryKey">Key of the country to return.</param>
        /// <param name="language">Language code for the metadata to return.</param>

        public async Task<ICountryMetadata> GetCountryAsync( string countryKey, string language )
        {
            if ( countryKey == null ) throw new ArgumentNullException( nameof( countryKey ) );

            var id = language != null
                ? $"data/{countryKey}--{language}"
                : $"data/{countryKey}";

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
    }
}