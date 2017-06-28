using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;

namespace Fidget.Validation.Addresses.Converters
{
    public class CharArrayConverterTests
    {
        class Model
        {
            [JsonConverter(typeof(CharArrayConverter))]
            public object Value { get; set; }
        }

        public class Deserailize
        {
            /// <summary>
            /// Serializes the given source value.
            /// </summary>
            /// <param name="value">Source value to use for deserialization.</param>

            object invoke( object value )
            {
                var source = new { Value = value };
                var json = JsonConvert.SerializeObject( source );

                return JsonConvert
                    .DeserializeObject<Model>( json )
                    .Value;
            }

            [Fact]
            public void WhenSourceNull_shouldDeserializeNull()
            {
                var source = (IEnumerable<char>)null;
                var actual = invoke( source );

                Assert.Null( actual );
            }

            [Theory]
            [InlineData( 0 )]
            [InlineData( 1 )]
            [InlineData( 16 )]

            public void WhenSourceCharArray_shouldSplit( int elements )
            {
                var expected = Guid.NewGuid().ToString().ToCharArray( 0, elements );
                var actual = invoke( new string( expected ) );

                Assert.Equal( expected, actual );
            }

            [Fact]
            public void WhenSourceNotString_shouldThrow()
            {
                Assert.Throws<JsonSerializationException>( () => invoke( new object() ) );
            }
        }

        public class Serialize
        {
            class Target
            {
                public string Value { get; set; }
            }

            string invoke( object value )
            {
                var model = new Model { Value = value };
                var json = JsonConvert.SerializeObject( model );
                
                return JsonConvert
                    .DeserializeObject<Target>( json )
                    .Value;
            }

            [Fact]
            public void WhenSourceNull_shouldSerializeNull()
            {
                var source = (IEnumerable<char>)null;
                var actual = invoke( source );

                Assert.Null( actual );
            }

            [Theory]
            [InlineData( 0 )]
            [InlineData( 1 )]
            [InlineData( 16 )]

            public void WhenSourceCharArray_souldDelimitCollection( int elements )
            {
                var source = Guid.NewGuid().ToString().ToCharArray( 0, elements );
                var expected = new string( source );
                var actual = invoke( source );

                Assert.Equal( expected, actual );
            }

            [Fact]
            public void WhenSourceNotCharArray_shouldNotSerialize()
            {
                var source = new object();
                var actual = invoke( source );

                Assert.Null( actual );
            }
        }
    }
}
