using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Converters
{
    /// <summary>
    /// Converter for handling tilde(~)-delimited collections of strings.
    /// </summary>

    class TildeDelimitedConverter : JsonConverter
    {
        /// <summary>
        /// Returns whether the converter can handle properties of the given type.
        /// </summary>
        /// <param name="objectType">Target data type.</param>
        
        public override bool CanConvert( Type objectType ) => objectType == typeof(IEnumerable<string>);
        
        /// <summary>
        /// Separator collection.
        /// </summary>
        
        static readonly char[] Separators = new char[] { '~' };

        /// <summary>
        /// Converts the value from JSON to the target data type.
        /// </summary>
        /// <param name="reader">Source data.</param>
        /// <param name="objectType">Target data type.</param>
        /// <param name="existingValue">Current value of the target property.</param>
        /// <param name="serializer">Serialization settings.</param>
        
        public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer ) => serializer
                .Deserialize<string>( reader )?
                .Split( Separators, StringSplitOptions.RemoveEmptyEntries );
        
        /// <summary>
        /// Converts the property value to JSON.
        /// </summary>
        /// <param name="writer">Target data.</param>
        /// <param name="value">Value to serialize.</param>
        /// <param name="serializer">Serialization settings.</param>
        
        public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
        {
            if ( value is IEnumerable<string> array )
            {
                serializer.Serialize( writer, string.Join( "~", array ) );
            }
        }
    }
}