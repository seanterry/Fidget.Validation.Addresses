using Newtonsoft.Json;
using System.Collections.Generic;
using Xunit;

namespace Fidget.Validation.Addresses.Metadata.Converters
{
    public class TildeDelimitedStringConverterTests
    {
        /// <summary>
        /// Model type with decorated property.
        /// </summary>
        
        class Model
        {
            [JsonConverter(typeof(TildeDelimitedStringConverter))]
            public IEnumerable<string> Value { get; set; }
        }

        /// <summary>
        /// Model type with an invalid decoration.
        /// </summary>
        
        class InvalidDecoration
        {
            [JsonConverter( typeof( TildeDelimitedStringConverter ) )]
            public int Value { get; set; }
        }

        public class ReadJson
        {
            IEnumerable<string> invoke( string source )
            {
                var json = JsonConvert.SerializeObject( new { Value = source } );
                var model = JsonConvert.DeserializeObject<Model>( json );

                return model.Value;
            }

            public static IEnumerable<object[]> DeserializableCases => new object[][]
            {
                // null and empty strings should deserialize to null
                new object[] { null, null },
                new object[] { string.Empty, null },
                
                new object[] { "AA", new string[] { "AA" } },
                new object[] { "AA~BB~CC", new string[] { "AA", "BB", "CC" } },
                
                // individual empty elements should be represented as null in the collection
                new object[] { "AA~~CC", new string[] { "AA", null, "CC" } },
                new object[] { "~", new string[] { null, null } },
                new object[] { "~~", new string[] { null, null, null } },
            };

            [Theory]
            [MemberData(nameof(DeserializableCases))]
            public void Returns_deserializedCollection( string source, IEnumerable<string> expected )
            {
                var actual = invoke( source );
                Assert.Equal( expected, actual );
            }
            
            [Fact]
            public void Throws_WhenWrongTypeDecorated()
            {
                var source = JsonConvert.SerializeObject( new { Value = "AA" } );
                void invokeWrongDataType() => JsonConvert.DeserializeObject<InvalidDecoration>( source );
                Assert.Throws<JsonSerializationException>( () => invokeWrongDataType() );
            }
        }

        public class WriteJson
        {
            // deserialization target
            class Target
            {
                public string Value { get; set; }
            }

            string invoke( IEnumerable<string> source )
            {
                var model = new Model { Value = source };
                var json = JsonConvert.SerializeObject( model );
                var target = JsonConvert.DeserializeObject<Target>( json );

                return target.Value;
            }

            public static IEnumerable<object[]> SerializableCases => new object[][]
            {
                // null and empty collections should serialize to null
                new object[] { null, null },
                new object[] { null, new string[0] },

                new object[] { "AA", new string[] { "AA" } },
                new object[] { "AA~BB~CC", new string[] { "AA", "BB", "CC" } },

                // individual null elements should be represented as empty in the collection
                new object[] { "AA~~CC", new string[] { "AA", null, "CC" } },
                new object[] { "~", new string[] { null, null } },
                new object[] { "~~", new string[] { null, null, null } },

                new object[] { "AA~~CC", new string[] { "AA", "", "CC" } },
                new object[] { "~", new string[] { "", "" } },
                new object[] { "~~", new string[] { "", "", "" } },
            };

            [Theory]
            [MemberData( nameof( SerializableCases ) )]
            public void Returns_serializedCollection( string expected, IEnumerable<string> source )
            {
                var actual = invoke( source );
                Assert.Equal( expected, actual );
            }

            [Fact]
            public void Throws_WhenWrongTypeDecorated()
            {
                var source = new InvalidDecoration();
                void invokeWrongDataType() => JsonConvert.SerializeObject( source );
                Assert.Throws<JsonSerializationException>( () => invokeWrongDataType() );
            }
        }
    }
}