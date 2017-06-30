using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fidget.Validation.Addresses.Service.Converters
{
    public class TildeDelimitedBooleanConverterTests
    {
        class Model
        {
            [JsonConverter(typeof(TildeDelimitedBooleanConverter))]
            public IEnumerable<bool> Value { get; set; }
        }

        public class ReadJson
        {
            IEnumerable<bool> invoke( string source )
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
                yield return new object[] { "true", new bool[] { true } };
                yield return new object[] { "false", new bool[] { false } };
                yield return new object[] { "true~false~true", new bool[] { true, false, true } };
                yield return new object[] { "false~true~false", new bool[] { false, true, false } };
            }

            [Theory]
            [MemberData(nameof(GetConvertibleValues))]
            public void Returns_deserializedCollection_whenSourceIsNotNullOrEmpty( string source, IEnumerable<bool> expected )
            {
                var actual = invoke( source );
                Assert.Equal( expected, actual );
            }

            [Fact]
            public void Throws_whenNonParseableAsBoolean()
            {
                Assert.Throws<JsonSerializationException>( ()=> invoke( "true~red~false" ) );
            }

            class InvalidDecoration
            {
                [JsonConverter(typeof(TildeDelimitedBooleanConverter))]
                public int Value { get; set; }
            }

            [Fact]
            public void Throws_WhenWrongTypeDecorated()
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

            public static IEnumerable<object[]> GetNullOrEmptyCollections()
            {
                yield return new object[] { default(bool[]) };
                yield return new object[] { new bool[0] };
            }

            [Theory]
            [MemberData(nameof(GetNullOrEmptyCollections))]
            public void Returns_null_whenSourceIsNullOrEmptyCollection( IEnumerable<bool> source )
            {
                var actual = invoke( source );
                Assert.Null( actual );
            }

            public static IEnumerable<object[]> GetConvertibleValues()
            {
                yield return new object[] { "true", new bool[] { true } };
                yield return new object[] { "false", new bool[] { false } };
                yield return new object[] { "true~false~true", new bool[] { true, false, true } };
                yield return new object[] { "false~true~false", new bool[] { false, true, false } };
            }

            [Theory]
            [MemberData( nameof( GetConvertibleValues ) )]
            public void Returns_deserializedCollection_whenSourceIsNotNullOrEmpty( string expected, IEnumerable<bool> source )
            {
                var actual = invoke( source );
                Assert.Equal( expected, actual );
            }

            class InvalidDecoration
            {
                [JsonConverter(typeof(TildeDelimitedBooleanConverter))]
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