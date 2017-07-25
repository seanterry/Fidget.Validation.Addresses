using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fidget.Validation.Addresses.Metadata.Converters
{
    /// <summary>
    /// Converter for handling collections of tilde (~) delimited data from the address service.
    /// </summary>

    class TildeDelimitedStringConverter : JsonConverter
    {
        /// <summary>
        /// Returns whether the converter can handle properties of the given type.
        /// </summary>
        /// <param name="objectType">Target property type.</param>
        
        public override bool CanConvert( Type objectType ) => objectType == typeof(IEnumerable<string>);
        
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
            
            // converts candidate strings
            // empty strings should be represented as null
            string convert( string candidate ) => string.IsNullOrEmpty( candidate ) ? null : candidate;

            return string.IsNullOrEmpty( source ) 
                ? null 
                : source.Split( '~' ).Select( _=> convert(_) ).ToArray();
        }

        /// <summary>
        /// Serializes the property value.
        /// </summary>
        /// <param name="writer">Target JSON writer.</param>
        /// <param name="value">Value to serialize.</param>
        /// <param name="serializer">Serialization settings.</param>
        
        public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
        {
            if ( value is IEnumerable<string> collection )
            {
                // only serialize if there are any elements; empty collection should be represented as null
                if ( collection.Any() ) serializer.Serialize( writer, string.Join( "~", collection ) );
            }

            else if ( value != null ) 
                throw new JsonSerializationException( $"Unexpected data type {value.GetType()}" );
        }
    }
}