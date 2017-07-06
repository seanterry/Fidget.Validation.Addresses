using Newtonsoft.Json;
using System.Collections.Generic;
using Xunit;

namespace Fidget.Validation.Addresses.Service.Converters
{
    public class TildeDelimitedStringConverterTests
    {
        class Model
        {
            [JsonConverter(typeof(TildeDelimitedStringConverter))]
            public IEnumerable<string> Value { get; set; }
        }
        
        public class ReadJson
        {
            IEnumerable<string> invoke( string source )
            {
                var json = JsonConvert.SerializeObject( new { Value = source } );
                var model = JsonConvert.DeserializeObject<Model>( json );

                return model.Value;
            }

            [Theory]
            [InlineData( null )]
            [InlineData( "" )]
            public void Returns_null_whenSourceIsNullOrEmpty( string source )
            {
                var actual = invoke( source );
                Assert.Null( actual );
            }

            public static IEnumerable<object[]> GetConvertibleValues()
            {
                yield return new object[] { "AA", new string[] { "AA" } };
                yield return new object[] { "AA~BB~CC", new string[] { "AA", "BB", "CC" } };
                
                // individual empty elements should be represented as null in the collection
                yield return new object[] { "AA~~CC", new string[] { "AA", null, "CC" } };
                yield return new object[] { "~", new string[] { null, null } };
                yield return new object[] { "~~", new string[] { null, null, null } };
            }

            [Theory]
            [MemberData(nameof(GetConvertibleValues))]
            public void Returns_deserializedCollection_whenSourceIsNotNullOrEmpty( string source, IEnumerable<string> expected )
            {
                var actual = invoke( source );
                Assert.Equal( expected, actual );
            }

            class InvalidDecoration
            {
                [JsonConverter(typeof(TildeDelimitedStringConverter))]
                public int Value { get; set; }
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

            public static IEnumerable<object[]> GetNullOrEmptyCollections()
            {
                yield return new object[] { default(string[]) };
                yield return new object[] { new string[0] };
            }

            [Theory]
            [MemberData(nameof(GetNullOrEmptyCollections))]
            public void Returns_null_whenSourceIsNullOrEmptyCollection( IEnumerable<string> source )
            {
                var actual = invoke( source );
                Assert.Null( actual );
            }

            public static IEnumerable<object[]> GetConvertibleValues()
            {
                yield return new object[] { "AA", new string[] { "AA" } };
                yield return new object[] { "AA~BB~CC", new string[] { "AA", "BB", "CC" } };

                // individual null elements should be represented as empty in the collection
                yield return new object[] { "AA~~CC", new string[] { "AA", null, "CC" } };
                yield return new object[] { "~", new string[] { null, null } };
                yield return new object[] { "~~", new string[] { null, null, null } };

                yield return new object[] { "AA~~CC", new string[] { "AA", "", "CC" } };
                yield return new object[] { "~", new string[] { "", "" } };
                yield return new object[] { "~~", new string[] { "", "", "" } };
            }

            [Theory]
            [MemberData( nameof( GetConvertibleValues ) )]
            public void Returns_deserializedCollection_whenSourceIsNotNullOrEmpty( string expected, IEnumerable<string> source )
            {
                var actual = invoke( source );
                Assert.Equal( expected, actual );
            }

            class InvalidDecoration
            {
                [JsonConverter(typeof(TildeDelimitedStringConverter))]
                public int Value { get; set; }
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