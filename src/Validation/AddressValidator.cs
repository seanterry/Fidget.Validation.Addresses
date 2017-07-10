using Fidget.Validation.Addresses.Service.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Provides a means of validating an address against regional metadata.
    /// </summary>
    
    class AddressValidator : IAddressValidator
    {
        /// <summary>
        /// Default address validator instance.
        /// </summary>
        
        internal static readonly IAddressValidator Default = DependencyInjection.Container.GetInstance<IAddressValidator>();

        /// <summary>
        /// Validates the given address against regional metadata.
        /// </summary>
        /// <param name="address">Address to validate.</param>
        /// <param name="country">Country metadata.</param>
        /// <param name="province">Province metadata.</param>
        /// <param name="locality">Locality metadata.</param>
        /// <param name="sublocality">Sublocality metadata.</param>
        /// <returns>The collection of validation errors, if any.</returns>
        
        public IEnumerable<ValidationFailure> Validate( AddressData address, ICountryMetadata country, IProvinceMetadata province, ILocalityMetadata locality, ISublocalityMetadata sublocality )
        {
            throw new NotImplementedException();
        }
    }
}