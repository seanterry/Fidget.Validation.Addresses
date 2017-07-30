using Newtonsoft.Json;
using System.Collections.Generic;
using Xunit;

namespace Fidget.Validation.Addresses.Metadata.Converters
{
    public class TildeDelimitedBooleanConverterTests
    {
        /// <summary>
        /// Model with property decorated for deserialization.
        /// </summary>
        
        class Model
        {
            [JsonConverter(typeof(TildeDelimitedBooleanConverter))]
            public IEnumerable<bool> Value { get; set; }
        }

        /// <summary>
        /// Model type containing an invalid decoration.
        /// </summary>
        
        class InvalidDecoration
        {
            [JsonConverter( typeof( TildeDelimitedBooleanConverter ) )]
            public int Value { get; set; }
        }

        public class ReadJson
        {
            IEnumerable<bool> invoke( string source )
            {
                var json = JsonConvert.SerializeObject( new { Value = source } );
                var model = JsonConvert.DeserializeObject<Model>( json );

                return model.Value;
            }

            public static IEnumerable<object[]> DeserializableCases => new object[][]
            {
                // null and empty collections should deserialize to null
                new object[] { null, null },
                new object[] { string.Empty, null },
                new object[] { "true", new bool[] { true } },
                new object[] { "false", new bool[] { false } },
                new object[] { "true~false~true", new bool[] { true, false, true } },
                new object[] { "false~true~false", new bool[] { false, true, false } },
            };

            [Theory]
            [MemberData(nameof(DeserializableCases))]
            public void Returns_deserializedCollection( string source, IEnumerable<bool> expected )
            {
                var actual = invoke( source );
                Assert.Equal( expected, actual );
            }

            [Fact]
            public void Throws_whenNonParseableAsBoolean()
            {
                Assert.Throws<JsonSerializationException>( ()=> invoke( "true~red~false" ) );
            }
            
            [Fact]
            public void Throws_whenWrongTypeDecorated()
            {
                var source = JsonConvert.SerializeObject( new { Value = "true~false" } );
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

            string invoke( IEnumerable<bool> source )
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
                new object[] { null, new bool[0] },
                new object[] { "true", new bool[] { true } },
                new object[] { "false", new bool[] { false } },
                new object[] { "true~false~true", new bool[] { true, false, true } },
                new object[] { "false~true~false", new bool[] { false, true, false } },
            };
            
            [Theory]
            [MemberData( nameof( SerializableCases ) )]
            public void Returns_serializedCollection( string expected, IEnumerable<bool> source )
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