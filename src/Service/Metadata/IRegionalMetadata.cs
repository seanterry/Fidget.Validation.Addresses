using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Service.Metadata
{
    /// <summary>
    /// Defines metadata available at any regional level.
    /// </summary>

    public interface IRegionalMetadata : ICommonMetadata
    {
        /// <summary>
        /// Gets the key of the region, which uniquely identifies it within its parent data element.
        /// </summary>

        string Key { get; }

        /// <summary>
        /// Gets the language for the regional data (if known).
        /// </summary>

        string Language { get; }

        /// <summary>
        /// Gets the collection of address elements that are required for addresses in the region.
        /// If this value is null, the value from the parent region should be used.
        /// </summary>
        
        IEnumerable<char> Required { get; }

        /// <summary>
        /// Gets a regular expression pattern to use for validating the postal code.
        /// At the country level, this pattern should match the entire pattern.
        /// For all other levels, this is a prefix.
        /// </summary>
        
        string PostalCodePattern { get; }

        /// <summary>
        /// Gets a regular expression pattern that overrides the country <see cref="PostalCodePattern"/> for the region
        /// and its children.
        /// </summary>
        /// <remarks>
        /// <see cref="https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/AddressDataKey.java"/>
        /// Encodes the <see cref="PostalCodePattern"/> value for the subtree beneath this region.
        /// </remarks>

        string PostalCodePatternOverride { get; }

        /// <summary>
        /// Gets the <see cref="Key"/> values of this region's sub-regions (if any).
        /// </summary>

        IEnumerable<string> ChildRegionKeys { get; }

        /// <summary>
        /// Gets the <see cref="Name"/> values of this region's sub-regions (if any).
        /// </summary>

        IEnumerable<string> ChildRegionNames { get; }

        /// <summary>
        /// Gets the <see cref="LatinName"/> values of this region's sub-regions (if any).
        /// </summary>

        IEnumerable<string> ChildRegionLatinNames { get; }

        /// <summary>
        /// Gets whether child regions have their own children.
        /// </summary>

        IEnumerable<bool> ChildRegionExpands { get; }
    }
}