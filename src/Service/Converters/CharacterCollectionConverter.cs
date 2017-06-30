using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fidget.Validation.Addresses.Service.Converters
{
    /// <summary>
    /// Converter for handling collections of character data from the address service.
    /// </summary>

    class CharacterCollectionConverter : JsonConverter
    {
        /// <summary>
        /// Returns whether the converter can handle properties of the given type.
        /// </summary>
        /// <param name="objectType">Target property type.</param>

        public override bool CanConvert( Type objectType ) => objectType == typeof( IEnumerable<char> );

        /// <summary>
        /// Deserializes the property value.
        /// </summary>
        /// <param name="reader">JSON data source.</param>
        /// <param name="objectType">Type of the target property.</param>
        /// <param name="existingValue">Current value of the target property.</param>
        /// <param name="serializer">Serialization settings.</param>

        public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
        {
            var source = serializer.Deserialize<string>( reader );

            return source?.ToCharArray();
        }

        /// <summary>
        /// Serializes the property value.
        /// </summary>
        /// <param name="writer">Target JSON writer.</param>
        /// <param name="value">Value to serialize.</param>
        /// <param name="serializer">Serialization settings.</param>

        public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
        {
            if ( value is IEnumerable<char> collection )
            {
                serializer.Serialize( writer, new string( collection.ToArray() ) );
            }

            else if ( value != null )
                throw new JsonSerializationException( $"Unexpected data type {value.GetType()}" );
        }
    }
}