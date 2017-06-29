using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Service.Metadata.Internal
{
    class GlobalMetadata : CommonMetadata, IGlobalMetadata
    {
        /// <summary>
        /// Gets the keys of countries for which data is available.
        /// </summary>
        
        [JsonProperty("countries")]
        [JsonConverter(typeof(TildeDelimitedStringConverter))]

        public IEnumerable<string> Countries { get; set; }
    }
}