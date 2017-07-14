using Fidget.Validation.Addresses.Service.Metadata;
using System;
using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Defines an address validation decorator.
    /// </summary>

    abstract class AddressValidatorDecorator : IAddressValidatorEx
    {
        /// <summary>
        /// Gets the address validator decorated by the current instance.
        /// </summary>
        
        protected IAddressValidatorEx Next { get; }

        /// <summary>
        /// Initializes a new instance to decorate an address validator.
        /// </summary>
        /// <param name="next">Address validator decorated by the current instance.</param>
        
        protected AddressValidatorDecorator( IAddressValidatorEx next )
        {
            Next = next ?? throw new ArgumentNullException( nameof(next) );
        }

        /// <summary>
        /// Interface implementation.
        /// </summary>
        
        public abstract IEnumerable<ValidationFailure> Validate( AddressData address, IGlobalMetadata global, ICountryMetadata country, IProvinceMetadata province, ILocalityMetadata locality, ISublocalityMetadata sublocality );
    }
}