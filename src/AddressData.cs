namespace Fidget.Validation.Addresses
{
    /// <summary>
    /// Represents a postal address.
    /// </summary>

    public class AddressData
    {
        /// <summary>
        /// Gets or sets the recipient name.
        /// </summary>
        
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the organization name.
        /// </summary>
        
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets the street address line(s).
        /// </summary>
        
        public string StreetAddress { get; set; }

        /// <summary>
        /// Gets or sets the dependent locality.
        /// </summary>
        
        public string Sublocality { get; set; }

        /// <summary>
        /// Gets or sets the city or locality.
        /// </summary>
        
        public string Locality { get; set; }

        /// <summary>
        /// Gets or sets the state or province.
        /// </summary>
        
        public string Province { get; set; }

        /// <summary>
        /// Gets or sets the ZIP or postal code.
        /// </summary>
        
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the sorting code.
        /// </summary>
        
        public string SortingCode { get; set; }

        /// <summary>
        /// Gets or sets the country for the address.
        /// </summary>
        
        public string Country { get; set; }
    }
}