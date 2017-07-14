using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fidget.Validation.Addresses.Service.Metadata;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Validator for validating known regional values.
    /// </summary>
    
    class KnownRegionValidator : AddressValidatorDecorator
    {
        /// <summary>
        /// Constructs a validator for validating known regional values.
        /// </summary>
        /// <param name="next">Address validator decorated by the current instance.</param>

        public KnownRegionValidator( IAddressValidatorEx next ) : base( next ) {}

        /// <summary>
        /// Validates the given address.
        /// </summary>
        
        public override IEnumerable<ValidationFailure> Validate( AddressData address, IGlobalMetadata global, ICountryMetadata country, IProvinceMetadata province, ILocalityMetadata locality, ISublocalityMetadata sublocality )
        {
            throw new NotImplementedException();
        }
    }
}