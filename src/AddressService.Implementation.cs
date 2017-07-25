using Fidget.Validation.Addresses.Service;
using Fidget.Validation.Addresses.Metadata;
using Fidget.Validation.Addresses.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses
{
    partial class AddressService
    {
        /// <summary>
        /// Implementation of the address service.
        /// </summary>
        
        internal class Implementation : IAddressService
        {
            /// <summary>
            /// Adapter that backs the service.
            /// </summary>
            
            readonly IServiceAdapter Adapter;

            /// <summary>
            /// Service client.
            /// </summary>
            
            readonly IServiceClient Client;

            /// <summary>
            /// Factory for creating validation contexts.
            /// </summary>
            
            readonly IValidationContextFactory Factory;

            /// <summary>
            /// Constructs an implementation of the address service.
            /// </summary>
            /// <param name="client">Service client.</param>
            /// <param name="validators">Collection of address validators.</param>
            
            public Implementation( IServiceAdapter adapter, IServiceClient client, IValidationContextFactory factory )
            {
                Adapter = adapter ?? throw new ArgumentNullException( nameof(adapter) );
                Client = client ?? throw new ArgumentNullException( nameof(client) );
                Factory = factory ?? throw new ArgumentNullException( nameof(factory) );
            }

            /// <summary>
            /// Builds and returns the data service identifier based on the given keys and language.
            /// </summary>
            /// <param name="language">Language to encode into the identifier.</param>
            /// <param name="keys">Keys for the identifier.</param>

            string BuildIdentifier( string language, params string[] keys ) => $"data/{string.Join( "/", keys )}{(language != null ? $"--{language}" : string.Empty)}";

            /// <summary>
            /// Returns gobal metadata information.
            /// </summary>

            public async Task<GlobalMetadata> GetGlobalAsync() => await Adapter.GetGlobal();

            /// <summary>
            /// Returns metadata for the specified country if it is available.
            /// </summary>
            /// <param name="countryKey">Key of the country to return.</param>
            /// <param name="language">Language code for the metadata to return.</param>

            public async Task<CountryMetadata> GetCountryAsync( string countryKey, string language )
            {
                var global = await GetGlobalAsync();

                return await Adapter.GetCountry( countryKey, language );
            }
            
            /// <summary>
            /// Returns metadata for the specified province if it is available.
            /// </summary>
            /// <param name="countryKey">Key of the parent country.</param>
            /// <param name="province">Key or name of the province to return.</param>
            /// <param name="language">Language code for the metadata to return.</param>

            public async Task<ProvinceMetadata> GetProvinceAsync( string countryKey, string province, string language )
            {
                var country = await GetCountryAsync( countryKey, language );
                
                return await Adapter.GetProvince( country, province, language );
            }

            /// <summary>
            /// Returns metadata for the specified locality if it is available.
            /// </summary>
            /// <param name="countryKey">Key of the parent country.</param>
            /// <param name="provinceKey">Key of the parent province.</param>
            /// <param name="localityKey">Key of the locality to return.</param>
            /// <param name="language">Language code for the metadata to return.</param>

            public async Task<LocalityMetadata> GetLocalityAsync( string countryKey, string provinceKey, string localityKey, string language )
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

            public async Task<SublocalityMetadata> GetSublocalityAsync( string countryKey, string provinceKey, string localityKey, string sublocalityKey, string language )
            {
                if ( countryKey == null ) throw new ArgumentNullException( nameof( countryKey ) );
                if ( provinceKey == null ) throw new ArgumentNullException( nameof( provinceKey ) );
                if ( localityKey == null ) throw new ArgumentNullException( nameof( localityKey ) );
                if ( sublocalityKey == null ) throw new ArgumentNullException( nameof( sublocalityKey ) );

                var id = BuildIdentifier( language, countryKey, provinceKey, localityKey, sublocalityKey );

                return await Client.Query<SublocalityMetadata>( id );
            }

            /// <summary>
            /// Returns whether the given country identifier is known by the global metadata.
            /// </summary>
            /// <param name="global">Global metadata.</param>
            /// <param name="value">Country identifier.</param>
            /// <param name="countryKey">Key of the country, if found.</param>
            
            public bool TryGetCountryKey( GlobalMetadata global, string value, out string countryKey )
            {
                countryKey = global?.Countries?.Contains( value ) == true
                    ? value
                    : null;

                return countryKey != null;
            }

            /// <summary>
            /// Returns whether the specified regional identifier is a child of the given parent region.
            /// </summary>
            /// <param name="parent">Parent region.</param>
            /// <param name="value">Key, name, or latin name of the child region.</param>
            /// <param name="key">Key of the child region, if found.</param>
            
            public bool TryGetChildKey( RegionalMetadata parent, string value, out string key )
            {
                int? getKeyIndex( params IEnumerable<string>[] collections )
                {
                    foreach ( var collection in collections )
                    {
                        var candidate = collection != null
                            ? Array.FindIndex( collection as string[] ?? collection?.ToArray(), _ => _.Equals( value, StringComparison.OrdinalIgnoreCase ) )
                            : (int?)null;

                        if ( candidate >= 0 ) return candidate;
                    }

                    return null;
                }

                var index = getKeyIndex( parent?.ChildRegionKeys, parent?.ChildRegionNames, parent?.ChildRegionLatinNames );

                key = index.HasValue
                    ? parent.ChildRegionKeys.ElementAtOrDefault( index.Value )
                    : null;

                return key != null;
            }

            /// <summary>
            /// Validates the given address.
            /// </summary>
            /// <param name="address">Address to validate.</param>
            /// <returns>The collection of validation errors, if any.</returns>
            
            public async Task<IEnumerable<ValidationFailure>> ValidateAsync( AddressData address, string language = null )
            {
                if ( address == null ) throw new ArgumentNullException( nameof( address ) );

                var context = await Factory.Create( address, this, language );
                var failures = Factory.Validators.SelectMany( _=> _.Validate( address, context ) );
                
                return failures.ToArray();
            }
        }
    }
}