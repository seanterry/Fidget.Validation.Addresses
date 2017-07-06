using Newtonsoft.Json;

namespace Fidget.Validation.Addresses.Service.Metadata.Internal
{
    /// <summary>
    /// Defines the common elements of all address metadata.
    /// </summary>
    
    abstract class CommonMetadata : ICommonMetadata
    {
        /// <summary>
        /// Gets a value that uniquely identifies the data record to the remote service.
        /// </summary>
        /// <remarks>
        /// <see cref="https://github.com/googlei18n/libaddressinput/blob/master/common/src/main/java/com/google/i18n/addressinput/common/AddressDataKey.java"/>
        /// This is represented in the form of a path from parent IDs to the key.
        /// </remarks>

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}