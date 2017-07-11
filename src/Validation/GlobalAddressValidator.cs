using Fidget.Validation.Addresses.Service.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Validates basic information about an address at a global level.
    /// </summary>

    class GlobalAddressValidator : IAddressValidator
    {
        /// <summary>
        /// Default address validator instance.
        /// </summary>
        
        internal static readonly IAddressValidator Default = DependencyInjection.Container.GetInstance<IAddressValidator>();

        /// <summary>
        /// Failure indicating the country field is missing.
        /// </summary>
        
        internal static readonly ValidationFailure MissingCountry = new ValidationFailure( AddressField.Country, AddressFieldError.MissingRequiredField );

        /// <summary>
        /// Failure indicating the country is unknown.
        /// </summary>
        
        internal static readonly ValidationFailure UnknownCountry = new ValidationFailure( AddressField.Country, AddressFieldError.UnkownValue );

        /// <summary>
        /// Validates the given address against regional metadata.
        /// </summary>
        /// <param name="address">Address to validate.</param>
        /// <param name="global">Global metadata.</param>
        /// <param name="country">Country metadata.</param>
        /// <param name="province">Province metadata.</param>
        /// <param name="locality">Locality metadata.</param>
        /// <param name="sublocality">Sublocality metadata.</param>
        /// <returns>The collection of validation errors, if any.</returns>
        
        public IEnumerable<ValidationFailure> Validate( AddressData address, IGlobalMetadata global, ICountryMetadata country, IProvinceMetadata province, ILocalityMetadata locality, ISublocalityMetadata sublocality )
        {
            if ( address == null ) throw new ArgumentNullException( nameof( address ) );
            if ( global == null ) throw new ArgumentNullException( nameof( global ) );

            if ( string.IsNullOrWhiteSpace( address.Country ) ) 
                return new ValidationFailure[] { MissingCountry };
            
            if ( !global.Countries.Contains( address.Country, StringComparer.OrdinalIgnoreCase ) )
                return new ValidationFailure[] { UnknownCountry };

            // sanity check
            if ( country == null ) throw new ArgumentNullException( nameof( country ) );

            return Enumerable.Empty<ValidationFailure>();
        }
    }
}