using Fidget.Validation.Addresses.Metadata.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Metadata
{
    /// <summary>
    /// Defines metadata available at any regional level.
    /// </summary>

    public abstract class RegionalMetadata : CommonMetadata
    {
        /// <summary>
        /// Gets the key of the region, which uniquely identifies it within its parent data element.
        /// </summary>
        /// <remarks>
        /// <see cref="!:https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/AddressDataKey.java"/>
        /// If there is an accepted abbreviation for this region, then the key will be set to this and name will be set to the local name for this region. 
        /// If there is no accepted abbreviation, then this key will be the local name and there will be no local name specified. 
        /// This value must be present.
        /// </remarks>

        [JsonProperty("key")]
        public string Key { get; internal set; }

        /// <summary>
        /// Gets the localized name of the region, if different from <see cref="Key"/>
        /// </summary>
        
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the latinized name of the region if <see cref="Name"/> is not in a latin script.
        /// </summary>
        
        [JsonProperty("lname")]
        public string LatinName { get; internal set; }

        /// <summary>
        /// Gets the language for the regional data (if known).
        /// </summary>
        /// <remarks>
        /// <see cref="!:https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/AddressDataKey.java"/>
        /// The default language of any data for this region, if known.
        /// In practice, this is the default language only when no specific language was requested. 
        /// Otherwise, it is the language that was requested.
        /// <para>This element is not defined on all entries.</para>
        /// </remarks>

        [JsonProperty("lang")]
        public string Language { get; internal set; }

        /// <summary>
        /// Gets the collection of address elements that are required for addresses in the region.
        /// If this value is null, the value from the parent region should be used.
        /// </summary>
        /// <remarks>
        /// <see cref="!:https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/AddressDataKey.java"/>
        /// Indicates which fields must be present in a valid address.
        /// </remarks>

        [JsonProperty("require")]
        [JsonConverter(typeof(AddressFieldCollectionConverter))]
        public IEnumerable<AddressField> Required { get; internal set; }

        /// <summary>
        /// Gets a regular expression pattern to use for validating the postal code.
        /// At the country level, this pattern should match the entire pattern.
        /// For all other levels, this is a prefix.
        /// </summary>
        /// <remarks>
        /// <see cref="!:https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/AddressDataKey.java"/>
        /// Encodes the postal code pattern if at the country level, 
        /// and the postal code prefix if at a level below country.
        /// </remarks>

        [JsonProperty("zip")]
        public string PostalCodePattern { get; internal set; }

        /// <summary>
        /// Gets a regular expression pattern that overrides the country <see cref="PostalCodePattern"/> for the region
        /// and its children.
        /// </summary>
        /// <remarks>
        /// <see cref="!:https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/AddressDataKey.java"/>
        /// Encodes the <see cref="PostalCodePattern"/> value for the subtree beneath this region.
        /// </remarks>

        [JsonProperty("xzip")]
        public string PostalCodePatternOverride { get; internal set; }

        /// <summary>
        /// Gets the <see cref="Key"/> values of this region's sub-regions (if any).
        /// </summary>
        /// <remarks>
        /// <see cref="!:https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/AddressDataKey.java"/>
        /// The <see cref="Key"/> values of all the children of this region.
        /// </remarks>

        [JsonProperty("sub_keys")]
        [JsonConverter(typeof(TildeDelimitedStringConverter))]
        public IEnumerable<string> ChildRegionKeys { get; internal set; }

        /// <summary>
        /// Gets the <see cref="Name"/> values of this region's sub-regions (if any).
        /// </summary>
        /// <remarks>
        /// <see cref="!:https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/AddressDataKey.java"/>
        /// Encodes the local name value of all the children of this region.
        /// <see cref="!:https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/FieldVerifier.java"/>
        /// If there are latin names but no local names, and there are the same number of latin names as there are keys, 
        /// then we assume the local names are the same as the keys.
        /// </remarks>

        [JsonProperty("sub_names")]
        [JsonConverter(typeof(TildeDelimitedStringConverter))]
        public IEnumerable<string> ChildRegionNames { get; internal set; }

        /// <summary>
        /// Gets the <see cref="LatinName"/> values of this region's sub-regions (if any).
        /// </summary>
        /// <remarks>
        /// <see cref="!:https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/AddressDataKey.java"/>
        /// Encodes the transliterated latin name value of all the children of this region, 
        /// if the local names are not in latin script already.
        /// </remarks>
        
        [JsonProperty("sub_lnames")]
        [JsonConverter(typeof(TildeDelimitedStringConverter))]
        public IEnumerable<string> ChildRegionLatinNames { get; internal set; }
        
        /// <summary>
        /// Gets whether child regions have their own children.
        /// </summary>
        /// <remarks>
        /// <see cref="!:https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/AddressDataKey.java"/>
        /// Indicates, for each child of this region, whether that child has additional children.
        /// </remarks>

        [JsonProperty("sub_mores")]
        [JsonConverter(typeof(TildeDelimitedBooleanConverter))]
        public IEnumerable<bool> ChildRegionExpands { get; internal set; }
    }
}