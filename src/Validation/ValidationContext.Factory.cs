using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Validation
{
    partial class ValidationContext
    {
        /// <summary>
        /// Factory for creating validation contexts.
        /// </summary>
        
        internal class Factory : IValidationContextFactory
        {
            /// <summary>
            /// Gets the collection of address validators.
            /// </summary>
            
            public IEnumerable<IAddressValidator> Validators { get; }

            /// <summary>
            /// Constructs a factory for creating validation contexts.
            /// </summary>
            /// <param name="validators">Collection of address validators.</param>
            
            public Factory( IEnumerable<IAddressValidator> validators )
            {
                Validators = validators ?? throw new ArgumentNullException( nameof(validators) );
            }

            /// <summary>
            /// Creates and returns a validation context.
            /// </summary>
            /// <param name="address">Address for which to create a validation context.</param>
            /// <param name="service">Service from which to acquire metadata.</param>

            public async Task<IValidationContext> Create( AddressData address, IAddressService service, string language )
            {
                if ( address == null ) throw new ArgumentNullException( nameof( address ) );
                if ( service == null ) throw new ArgumentNullException( nameof( service ) );

                var global = await service.GetGlobalAsync();
                var country = service.TryGetCountryKey( global, address.Country, out string countryKey ) ? await service.GetCountryAsync( countryKey, language ) : null;
                var province = service.TryGetChildKey( country, address.Province, out string provinceKey ) ? await service.GetProvinceAsync( countryKey, provinceKey, language ) : null;
                var locality = service.TryGetChildKey( province, address.Locality, out string localityKey ) ? await service.GetLocalityAsync( countryKey, provinceKey, localityKey, language ) : null;
                var sublocality = service.TryGetChildKey( locality, address.Sublocality, out string sublocalityKey ) ? await service.GetSublocalityAsync( countryKey, provinceKey, localityKey, sublocalityKey, language ) : null;

                return new ValidationContext( global, country, province, locality, sublocality );
            }
        }
    }
}