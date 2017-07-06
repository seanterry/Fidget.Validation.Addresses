namespace Fidget.Validation.Addresses.Service.Metadata
{
    /// <summary>
    /// Defines metadata available to sub-regions that overrides values from the parent country.
    /// </summary>

    public interface IOverridableMetadata
    {
        /// <summary>
        /// Gets a regular expression pattern that overrides the country <see cref="PostalCodePattern"/> for the region
        /// and its children.
        /// </summary>
        /// <remarks>
        /// <see cref="https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/AddressDataKey.java"/>
        /// Encodes the <see cref="PostalCodePattern"/> value for the subtree beneath this region.
        /// </remarks>

        string PostalCodePatternOverride { get; }
    }
}