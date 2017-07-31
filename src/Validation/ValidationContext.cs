using Fidget.Validation.Addresses.Metadata;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Context for validating an address.
    /// </summary>

    public struct ValidationContext
    {
        /// <summary>
        /// Gets or sets the address to be validated.
        /// </summary>
        
        public AddressData Address { get; set; }

        /// <summary>
        /// Gets or sets the global metadata entry.
        /// </summary>
        
        public GlobalMetadata GlobalMetadata { get; set; }

        /// <summary>
        /// Gets or sets the country metadata entry for the address.
        /// </summary>
        
        public CountryMetadata CountryMetadata { get; set; }

        /// <summary>
        /// Gets or sets the province metadata entry for the address.
        /// </summary>
        
        public ProvinceMetadata ProvinceMetadata { get; set; }

        /// <summary>
        /// Gets or sets the locality metadata entry for the address.
        /// </summary>
        
        public LocalityMetadata LocalityMetadata { get; set; }

        /// <summary>
        /// Gets or sets the sublocality metadata entry for the address.
        /// </summary>
        
        public SublocalityMetadata SublocalityMetadata { get; set; }
    }
}