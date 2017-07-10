namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Represents an address validation failure.
    /// </summary>
    
    public struct ValidationFailure
    {
        /// <summary>
        /// Gets the address field that failed validation.
        /// </summary>
        
        public AddressField Field { get; }

        /// <summary>
        /// Gets the reason for the failure.
        /// </summary>
        
        public AddressFieldError Error { get; }

        /// <summary>
        /// Constructs a value to represent an address validation failure.
        /// </summary>
        /// <param name="field">The address field that failed validation.</param>
        /// <param name="error">The reason for the failure.</param>
        
        public ValidationFailure( AddressField field, AddressFieldError error )
        {
            Field = field;
            Error = error;
        }
    }
}