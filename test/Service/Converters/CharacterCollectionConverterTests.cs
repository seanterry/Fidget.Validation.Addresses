using Newtonsoft.Json;
using System.Collections.Generic;
using Xunit;

namespace Fidget.Validation.Addresses.Service.Converters
{
    public class CharacterCollectionConverterTests
    {
        class Model
        {
            [JsonConverter(typeof(CharacterCollectionConverter))]
            public IEnumerable<char> Value { get; set; }
        }

        public class ReadJson
        {
            IEnumerable<char> invoke( string source )
            {
                var json = JsonConvert.SerializeObject( new { Value = source } );
                var model = JsonConvert.DeserializeObject<Model>( json );

                return model.Value;
            }

            [Theory]
            [InlineData( null )]
            public void Returns_null_whenSourceIsNull( string source )
            {
                var actual = invoke( source );
                Assert.Null( actual );
            }

            public static IEnumerable<object[]> GetConvertibleValues()
            {
                yield return new object[] { "ABC", new char[] { 'A', 'B', 'C' } };
                yield return new object[] { "AZ", new char[] { 'A', 'Z' } };
                yield return new object[] { "", new char[0] };
            }

            [Theory]
            [MemberData(nameof(GetConvertibleValues))]
            public void Returns_deserializedCollection_whenSourceIsNotNull( string source, IEnumerable<char> expected )
            {
                var actual = invoke( source );
                Assert.Equal( expected, actual );
            }

            class InvalidDecoration
            {
                [JsonConverter(typeof(CharacterCollectionConverter))]
                public int Value { get; set; }
            }

            [Fact]
            public void Throws_WhenWrongTypeDecorated()
            {
                var source = JsonConvert.SerializeObject( new { Value = "ABC" } );
                void invokeWrongDataType() => JsonConvert.DeserializeObject<InvalidDecoration>( source );
                Assert.Throws<JsonSerializationException>( () => invokeWrongDataType() );
            }
        }
    }
}