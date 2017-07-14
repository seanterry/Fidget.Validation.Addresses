using Fidget.Validation.Addresses.Service.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Validates basic information about an address at a global level.
    /// </summary>

    class GlobalAddressValidator : AddressValidatorDecorator
    {
        public GlobalAddressValidator( IAddressValidatorEx next ) : base( next ) {}

        /// <summary>
        /// Failure indicating the country is unknown.
        /// </summary>
        
        internal static readonly ValidationFailure UnknownCountry = new ValidationFailure( AddressField.Country, AddressFieldError.UnkownValue );

        /// <summary>
        /// Validates the given address.
        /// </summary>
        
        public override IEnumerable<ValidationFailure> Validate( AddressData address, IGlobalMetadata global, ICountryMetadata country, IProvinceMetadata province, ILocalityMetadata locality, ISublocalityMetadata sublocality )
        {
            if ( address == null ) throw new ArgumentNullException( nameof( address ) );
            if ( global == null ) throw new ArgumentNullException( nameof( global ) );

            var failures = Next.Validate( address, global, country, province, locality, sublocality ).ToList();

            if ( !global.Countries.Contains( address.Country, StringComparer.OrdinalIgnoreCase ) )
                return new ValidationFailure[] { UnknownCountry };

            // sanity check
            if ( country == null ) throw new ArgumentNullException( nameof( country ) );

            return Enumerable.Empty<ValidationFailure>();
        }
    }
}