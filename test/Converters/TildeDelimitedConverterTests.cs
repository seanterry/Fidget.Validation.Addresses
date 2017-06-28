using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fidget.Validation.Addresses.Converters
{
    public class TildeDelimitedConverterTests
    {
        class Model
        {
            [JsonConverter(typeof(TildeDelimitedConverter))]
            public object Value { get; set; }
        }
        
        public class Deserailize
        {
            /// <summary>
            /// Tests serialization with the given source value.
            /// </summary>
            /// <param name="value">Source value to use for deserialization.</param>
                        
            Model invoke( object value )
            {
                var source = new { Value = value };
                var json = JsonConvert.SerializeObject( source );

                return JsonConvert.DeserializeObject<Model>( json );
            }
            
            [Fact]
            public void WhenSourceNull_shouldDeserializeNull()
            {
                var source = (IEnumerable<string>)null;
                var actual = invoke( source );

                Assert.Null( actual.Value );
            }

            [Theory]
            [InlineData( 0 )]
            [InlineData( 1 )]
            [InlineData( 20 )]
            
            public void WhenSourceTildeDelimitedString_shouldSplit( int elements )
            {
                var expected = Enumerable.Range( 0, elements ).Select( _=> Guid.NewGuid().ToString() ).ToArray();
                var actual = invoke( string.Join( "~", expected ) );

                Assert.Equal( expected, actual.Value );
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

            string invoke( Model model )
            {
                var json = JsonConvert.SerializeObject( model );
                var target = JsonConvert.DeserializeObject<Target>( json );

                return target.Value;
            }

            [Fact]
            public void WhenSourceNull_shouldSerializeNull()
            {
                var source = (IEnumerable<string>)null;
                var model = new Model { Value = source };
                var actual = invoke( model );

                Assert.Null( actual );
            }

            [Theory]
            [InlineData( 0 )]
            [InlineData( 1 )]
            [InlineData( 20 )]

            public void WhenSourceStringArray_souldDelimitCollection( int elements )
            {
                var source = Enumerable.Range( 0, elements ).Select( _ => Guid.NewGuid().ToString() ).ToArray();
                var expected = string.Join( "~", source );
                var model = new Model { Value = source };
                var actual = invoke( model );

                Assert.Equal( expected, actual );
            }

            [Fact]
            public void WhenSourceNotStringArray_shouldNotSerialize()
            {
                var source = new object();
                var model = new Model { Value = source };
                var actual = invoke( model );

                Assert.Null( actual );
            }
        }
    }
}