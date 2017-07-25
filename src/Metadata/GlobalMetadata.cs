using Fidget.Validation.Addresses.Metadata.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Metadata
{
    /// <summary>
    /// Global-level metadata.
    /// </summary>
    
    public class GlobalMetadata : CommonMetadata
    {
        /// <summary>
        /// Gets the keys of countries for which data is available.
        /// </summary>
        
        [JsonProperty("countries")]
        [JsonConverter(typeof(TildeDelimitedStringConverter))]

        public IEnumerable<string> Countries { get; internal set; }
    }
}