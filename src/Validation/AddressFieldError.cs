namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Enumeration of errors that can be encountered in an address field.
    /// </summary>

    public enum AddressFieldError
    {
        /// <summary>
        /// The field but contains a value when the field is not valid for use in the country.
        /// </summary>
        /// <remarks>
        /// <see cref="!:https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/AddressProblemType.java"/>
        /// The field is not null and not whitespace, and the field should not be used by addresses in this country.
        /// </remarks>

        UnexpectedField,

        /// <summary>
        /// The field is required for the country but does not contain a value.
        /// </summary>
        /// <remarks>
        /// <see cref="!:https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/AddressProblemType.java"/>
        /// The field is null or whitespace, and the field is required for addresses in this country.
        /// </remarks>

        MissingRequiredField,

        /// <summary>
        /// The value of this field does not match the defined collection of valid values.
        /// </summary>
        /// <remarks>
        /// <see cref="!:https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/AddressProblemType.java"/>
        /// A list of values for the field is defined and the value does not occur in the list. 
        /// Applies to hierarchical elements.
        /// </remarks>

        UnkownValue,

        /// <summary>
        /// The value of this field does not match the required format.
        /// </summary>
        /// <remarks>
        /// <see cref="!:https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/AddressProblemType.java"/>
        /// A format for the field is defined and the value does not match.
        /// This is used to match postal codes against the general format pattern. 
        /// Formats indicate how many digits/letters should be present, and what punctuation is allowed.
        /// </remarks>

        InvalidFormat,

        /// <summary>
        /// The value of this field is in the correct format, but does not match the valid values for the region.
        /// </summary>
        /// <remarks>
        /// <see cref="!:https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/AddressProblemType.java"/>
        /// A specific pattern for the field is defined and the value does not match.
        /// This is used to match postal codes against a regular expression for the region.
        /// For example, in the US postal codes in the state of California start with a '9'.
        /// </remarks>

        MismatchingValue,
    }
}