using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fidget.Validation.Addresses.Converters
{
    /// <summary>
    /// Converter for character arrays represented as strings.
    /// </summary>

    class CharArrayConverter : JsonConverter
    {
        /// <summary>
        /// Returns whether the data type can be converted by the current instance.
        /// </summary>
        /// <param name="objectType">Type of the target property.</param>
        
        public override bool CanConvert( Type objectType ) => objectType == typeof(IEnumerable<char>);

        /// <summary>
        /// Returns the deserialized value from JSON.
        /// </summary>
        /// <param name="reader">Source data.</param>
        /// <param name="objectType">Target data type.</param>
        /// <param name="existingValue">Current value of the target property.</param>
        /// <param name="serializer">Serialization settings.</param>
        
        public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer ) => serializer
            .Deserialize<string>( reader )?
            .ToCharArray();

        /// <summary>
        /// Serializes the given value to JSON.
        /// </summary>
        /// <param name="writer">Target data.</param>
        /// <param name="value">Value to serialize.</param>
        /// <param name="serializer">Serialization settings.</param>
        
        public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
        {
            if ( value is IEnumerable<char> collection )
            {
                serializer.Serialize( writer, new string( collection.ToArray() ) );
            }
        }
    }
}