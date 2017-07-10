using Fidget.Validation.Addresses.Service;
using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using Fidget.Validation.Addresses.Validation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses
{
    /// <summary>
    /// Service for address validation and metadata exploration.
    /// </summary>

    public class AddressService : IAddressService
    {
        /// <summary>
        /// Remote address service client.
        /// </summary>
        
        readonly IServiceClient Client;

        /// <summary>
        /// Address validation service.
        /// </summary>
        
        readonly IAddressValidator Validator;

        /// <summary>
        /// Constructs a service for address validation and metadata exploration.
        /// </summary>
        /// <param name="client">Remote address service client.</param>
        
        internal AddressService( IServiceClient client, IAddressValidator validator )
        {
            Client = client ?? throw new ArgumentNullException( nameof(client) );
            Validator = validator ?? throw new ArgumentNullException( nameof(validator) );
        }

        /// <summary>
        /// Constructs a service for address validation and metadata exploration.
        /// </summary>
        
        public AddressService() : this( ServiceClient.Default, AddressValidator.Default ) {}

        /// <summary>
        /// Returns gobal metadata information.
        /// </summary>
        
        public async Task<IGlobalMetadata> GetGlobalAsync() => await Client.Query<GlobalMetadata>( "data" );

        /// <summary>
        /// Builds and returns the data service identifier based on the given keys and language.
        /// </summary>
        /// <param name="language">Language to encode into the identifier.</param>
        /// <param name="keys">Keys for the identifier.</param>
                
        string BuildIdentifier( string language, params string[] keys ) => $"data/{string.Join( "/", keys )}{(language != null ? $"--{language}" : string.Empty)}";

        /// <summary>
        /// Returns metadata for the specified country if it is available.
        /// </summary>
        /// <param name="countryKey">Key of the country to return.</param>
        /// <param name="language">Language code for the metadata to return.</param>

        public async Task<ICountryMetadata> GetCountryAsync( string countryKey, string language )
        {
            if ( countryKey == null ) throw new ArgumentNullException( nameof( countryKey ) );

            var id = BuildIdentifier( language, countryKey );
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

            var id = BuildIdentifier( language, countryKey, provinceKey );
            
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

            var id = BuildIdentifier( language, countryKey, provinceKey, localityKey );

            return await Client.Query<LocalityMetadata>( id );
        }

        /// <summary>
        /// Returns metadata for the specified sublocality if it is available.
        /// </summary>
        /// <param name="countryKey">Key of the parent country.</param>
        /// <param name="provinceKey">Key of the parent province.</param>
        /// <param name="localityKey">Key of the parent locality.</param>
        /// <param name="sublocalityKey">Key of the sublocality to return.</param>
        /// <param name="language">Language code for the metadata to return.</param>
        
        public async Task<ISublocalityMetadata> GetSublocalityAsync( string countryKey, string provinceKey, string localityKey, string sublocalityKey, string language )
        {
            if ( countryKey == null ) throw new ArgumentNullException( nameof( countryKey ) );
            if ( provinceKey == null ) throw new ArgumentNullException( nameof( provinceKey ) );
            if ( localityKey == null ) throw new ArgumentNullException( nameof( localityKey ) );
            if ( sublocalityKey == null ) throw new ArgumentNullException( nameof( sublocalityKey ) );

            var id = BuildIdentifier( language, countryKey, provinceKey, localityKey, sublocalityKey );

            return await Client.Query<SublocalityMetadata>( id );
        }

        /// <summary>
        /// Validates the given address.
        /// </summary>
        /// <param name="address">Address to validate.</param>
        /// <returns>The collection of validation errors, if any.</returns>
        
        public async Task<IEnumerable<ValidationFailure>> ValidateAsync( AddressData address )
        {
            throw new NotImplementedException();
        }
    }
}