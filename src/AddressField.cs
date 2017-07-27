namespace Fidget.Validation.Addresses
{
    /// <summary>
    /// Enumeration of address fields
    /// </summary>
    /// <see cref="https://github.com/googlei18n/libaddressinput/wiki/AddressValidationMetadata"/>

    public enum AddressField
    {
        /// <summary>
        /// Country field.
        /// </summary>
        
        Country = 0,

        /// <summary>
        /// Recipient name.
        /// </summary>
        
        Name = 'N',

        /// <summary>
        /// Organization name.
        /// </summary>
        
        Organization = 'O',

        /// <summary>
        /// Street address line(s).
        /// </summary>
        
        StreetAddress = 'A',

        /// <summary>
        /// Dependent locality.
        /// </summary>
        
        Sublocality = 'D',

        /// <summary>
        /// City or locality.
        /// </summary>
        
        Locality = 'C',

        /// <summary>
        /// State, province, or other administrative area.
        /// </summary>
        
        Province = 'S',

        /// <summary>
        /// ZIP or postal code.
        /// </summary>
        
        PostalCode = 'Z',

        /// <summary>
        /// Sorting code.
        /// </summary>
        
        SortingCode = 'X',
    }
}