namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Structure that represents an address validation failure.
    /// </summary>

    public struct AddressValidationFailure
    {
        /// <summary>
        /// Gets or sets the address field that failed validation.
        /// </summary>
        
        public AddressField Field { get; internal set; }

        /// <summary>
        /// Gets or sets the error found on the field.
        /// </summary>
        
        public AddressFieldError Error { get; internal set; }
    }
}