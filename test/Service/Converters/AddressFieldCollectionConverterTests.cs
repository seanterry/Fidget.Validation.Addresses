using Newtonsoft.Json;
using System.Collections.Generic;
using Xunit;

namespace Fidget.Validation.Addresses.Service.Converters
{
    public class AddressFieldCollectionConverterTests
    {
        class Model
        {
            [JsonConverter( typeof( AddressFieldCollectionConverter ) )]
            public IEnumerable<AddressField> Value { get; set; }
        }

        public class ReadJson
        {
            IEnumerable<AddressField> invoke( string source )
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
                yield return new object[] { "ACSZ", new AddressField[] { AddressField.StreetAddress, AddressField.Locality, AddressField.Province, AddressField.PostalCode } };
                yield return new object[] { "AZ", new AddressField[] { AddressField.StreetAddress, AddressField.PostalCode } };
                yield return new object[] { "", new AddressField[0] };
            }

            [Theory]
            [MemberData( nameof( GetConvertibleValues ) )]
            public void Returns_deserializedCollection_whenSourceIsNotNull( string source, IEnumerable<AddressField> expected )
            {
                var actual = invoke( source );
                Assert.Equal( expected, actual );
            }

            class InvalidDecoration
            {
                [JsonConverter( typeof( AddressFieldCollectionConverter ) )]
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

        public class WriteJson
        {
            // deserialization target
            class Target
            {
                public string Value { get; set; }
            }

            string invoke( IEnumerable<AddressField> source )
            {
                var model = new Model { Value = source };
                var json = JsonConvert.SerializeObject( model );
                var target = JsonConvert.DeserializeObject<Target>( json );

                return target.Value;
            }

            [Theory]
            [InlineData( null )]
            public void Returns_null_whenSourceIsNull( IEnumerable<AddressField> source )
            {
                var actual = invoke( source );
                Assert.Null( actual );
            }

            public static IEnumerable<object[]> GetConvertibleValues()
            {
                yield return new object[] { "ACSZ", new AddressField[] { AddressField.StreetAddress, AddressField.Locality, AddressField.Province, AddressField.PostalCode } };
                yield return new object[] { "AZ", new AddressField[] { AddressField.StreetAddress, AddressField.PostalCode } };
                yield return new object[] { "", new AddressField[0] };
            }

            [Theory]
            [MemberData( nameof( GetConvertibleValues ) )]
            public void Returns_deserializedCollection_whenSourceIsNotNull( string expected, IEnumerable<AddressField> source )
            {
                var actual = invoke( source );
                Assert.Equal( expected, actual );
            }

            class InvalidDecoration
            {
                [JsonConverter( typeof( AddressFieldCollectionConverter ) )]
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