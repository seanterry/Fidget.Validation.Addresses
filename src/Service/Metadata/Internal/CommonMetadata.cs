using Newtonsoft.Json;

namespace Fidget.Validation.Addresses.Service.Metadata.Internal
{
    /// <summary>
    /// Defines the common elements of all address metadata.
    /// </summary>

    abstract class CommonMetadata : ICommonMetadata
    {
        /// <summary>
        /// Gets the key of the region or data record, in the form of a path from parent identifiers to the record key.
        /// </summary>
        
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}