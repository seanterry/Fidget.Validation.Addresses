using Newtonsoft.Json;
using System.Collections.Generic;
using Xunit;

namespace Fidget.Validation.Addresses.Metadata.Converters
{
    public class AddressFieldCollectionConverterTests
    {
        /// <summary>
        /// Model type decorated with the converter.
        /// </summary>
        
        class Model
        {
            [JsonConverter( typeof( AddressFieldCollectionConverter ) )]
            public IEnumerable<AddressField> Value { get; set; }
        }

        public static IEnumerable<object[]> ConvertibleCases => new object[][]
        {
            new object[] { null, null },
            new object[] { "", new AddressField[0] },
            new object[] { "ACSZ", new AddressField[] { AddressField.StreetAddress, AddressField.Locality, AddressField.Province, AddressField.PostalCode } },
            new object[] { "AZ", new AddressField[] { AddressField.StreetAddress, AddressField.PostalCode } },
        };

        public class ReadJson
        {
            /// <summary>
            /// Invokes deserialization of a model type.
            /// </summary>
            /// <param name="source">Source string to be deserialized to the <see cref="Model.Value"/> property.</param>
            /// <returns>The deserialized value.</returns>
            
            IEnumerable<AddressField> invoke( string source )
            {
                var json = JsonConvert.SerializeObject( new { Value = source } );
                var model = JsonConvert.DeserializeObject<Model>( json );

                return model.Value;
            }
            
            [Theory]
            [MemberData(nameof(ConvertibleCases), MemberType =typeof(AddressFieldCollectionConverterTests))]
            public void Returns_deserializedCollection( string source, IEnumerable<AddressField> expected )
            {
                var actual = invoke( source );
                Assert.Equal( expected, actual );
            }
            
            /// <summary>
            /// Model containing a value that cannot be deserialized.
            /// </summary>
            
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
            /// <summary>
            /// Deserialization target.
            /// </summary>
            
            class Target
            {
                public string Value { get; set; }
            }

            /// <summary>
            /// Invokes serialization of the given collection.
            /// </summary>
            
            string invoke( IEnumerable<AddressField> source )
            {
                var model = new Model { Value = source };
                var json = JsonConvert.SerializeObject( model );
                var target = JsonConvert.DeserializeObject<Target>( json );

                return target.Value;
            }

            [Theory]
            [MemberData( nameof( ConvertibleCases ), MemberType = typeof( AddressFieldCollectionConverterTests ) )]
            public void Returns_serializedCollection( string expected, IEnumerable<AddressField> source )
            {
                var actual = invoke( source );
                Assert.Equal( expected, actual );
            }

            /// <summary>
            /// Model containing a value that cannot be deserialized.
            /// </summary>
            
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