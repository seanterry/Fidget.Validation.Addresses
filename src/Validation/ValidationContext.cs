using Fidget.Validation.Addresses.Service.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Context containing validation metadata.
    /// </summary>
    
    partial class ValidationContext : IValidationContext
    {
        /// <summary>
        /// Global metadata.
        /// </summary>
        
        internal readonly IGlobalMetadata Global;

        /// <summary>
        /// Country metadata.
        /// </summary>
        
        internal readonly ICountryMetadata Country;

        /// <summary>
        /// Province metadata.
        /// </summary>
        
        internal readonly IProvinceMetadata Province;

        /// <summary>
        /// Locality metadata.
        /// </summary>
        
        internal readonly ILocalityMetadata Locality;

        /// <summary>
        /// Gets the sublocality metadata.
        /// </summary>
        
        internal readonly ISublocalityMetadata Sublocality;

        /// <summary>
        /// Constructs a context for address validation.
        /// </summary>
        /// <param name="global">Global metadata.</param>
        /// <param name="country">Country metadata.</param>
        /// <param name="province">Province metadata.</param>
        /// <param name="locality">Locality metadata.</param>
        /// <param name="sublocality">Sublocality metadata.</param>
        
        public ValidationContext( IGlobalMetadata global, ICountryMetadata country, IProvinceMetadata province, ILocalityMetadata locality, ISublocalityMetadata sublocality )
        {
            Global = global ?? throw new ArgumentNullException( nameof(global) );
            Country = country;
            Province = province;
            Locality = locality;
            Sublocality = sublocality;
        }

        /// <summary>
        /// Default collection of required elements.
        /// This should always indicate the country.
        /// </summary>
        
        static readonly IEnumerable<AddressField> DefaultRequiredElements = new AddressField[] { AddressField.Country };

        /// <summary>
        /// Returns the elements that are required to be valid.
        /// </summary>
        
        public IEnumerable<AddressField> GetRequiredFields() => DefaultRequiredElements
            .Union( Country?.Required ?? DefaultRequiredElements )
            .Union( Province?.Required ?? DefaultRequiredElements )
            .Union( Locality?.Required ?? DefaultRequiredElements )
            .Union( Sublocality?.Required ?? DefaultRequiredElements )
            .Distinct()
            .ToArray();

        /// <summary>
        /// Returns the elements that are allowed in a valid address for the country.
        /// </summary>
        
        public IEnumerable<AddressField> GetAllowedFields() => Enum.GetValues( typeof(AddressField) )
            .OfType<AddressField>()
            .Where( field => field == AddressField.Country || Country?.Format?.Contains( $"%{(char)field}" ) == true )
            .ToArray();
    }
}