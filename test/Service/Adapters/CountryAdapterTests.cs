using Fidget.Validation.Addresses.Client;
using Fidget.Validation.Addresses.Metadata;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Service.Adapters
{
    public class CountryAdapterTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();
        Mock<IGlobalAdapter> MockGlobal = new Mock<IGlobalAdapter>();

        IServiceClient client => MockClient?.Object;
        IGlobalAdapter global => MockGlobal?.Object;
        ICountryAdapter instance => new CountryAdapter( client, global );

        public class Constructor : CountryAdapterTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof(client), ()=>instance );
            }

            [Fact]
            public void Requires_global()
            {
                MockGlobal = null;
                Assert.Throws<ArgumentNullException>( nameof(global), ()=>instance );
            }
        }

        public class Query : CountryAdapterTests
        {
            CountryMetadata defaultMeta = new CountryMetadata
            {
                Id = "data/ZZ",
                Format = "%N%n%O%n%A%n%C",
                Required = new AddressField[] { AddressField.StreetAddress, AddressField.Locality },
                Uppercase = new AddressField[] { AddressField.Locality },
                StateType = "province",
                LocalityType = "city",
                SublocalityType = "suburb",
                PostalCodeType = "postal",
            };

            [Fact]
            public async Task WhenDefaultRequested_returns_default()
            {
                MockClient.Setup( _=> _.Query<CountryMetadata>( "data/ZZ" ) ).ReturnsAsync( defaultMeta );
                var actual = await instance.Query( "ZZ", null );
                Assert.Same( defaultMeta, actual );
            }

            public static IEnumerable<object[]> QueryUnmatchedTestCases()
            {
                var globals = new GlobalMetadata[] { null, new GlobalMetadata() };
                var countries = new string[] { null, "XW", };
                var languages = new string[] { null, "en" };
                
                return
                    from country in countries
                    from language in languages
                    from globalMeta in globals
                    select new object[] { country, language, globalMeta };
            }

            [Theory]
            [MemberData(nameof(QueryUnmatchedTestCases))]
            public async Task WhenNotMatched_returns_null( string country, string language, GlobalMetadata globalMeta )
            {
                string key = null;
                MockClient.Setup( _ => _.Query<CountryMetadata>( "data/ZZ" ) ).ReturnsAsync( defaultMeta );
                MockGlobal.Setup( _=> _.Query() ).ReturnsAsync( globalMeta as GlobalMetadata );
                MockGlobal.Setup( _=> _.TryGetCountryKey( globalMeta, country, out key ) ).Returns( false );

                var actual = await instance.Query( country, language );
                Assert.Null( actual );

                // ensure no query was issued
                MockClient.Verify( _=> _.Query<CountryMetadata>( It.IsNotIn( new string[] { "data/ZZ" } ) ), Times.Never );
            }

            public static IEnumerable<object[]> QueryMatchedCases()
            {
                var country = "xw";
                var languages = new string[] { null, "en" };
                var results = new CountryMetadata[] { null, new CountryMetadata() };

                return
                    from language in languages
                    from result in results
                    select new object[] { country, language, result };
            }

            [Theory]
            [MemberData(nameof(QueryMatchedCases))]
            public async Task WhenMatched_returns_result( string country, string language, CountryMetadata result )
            {
                // expected identifier
                var key = "XW";
                var id = $"data/{key}{( language != null ? $"--{language}" : string.Empty )}";
                var globalMeta = new GlobalMetadata { Id = "data" };

                MockClient.Setup( _ => _.Query<CountryMetadata>( "data/ZZ" ) ).ReturnsAsync( defaultMeta );
                MockClient.Setup( _ => _.Query<CountryMetadata>( id ) ).ReturnsAsync( result as CountryMetadata );
                MockGlobal.Setup( _ => _.Query() ).ReturnsAsync( globalMeta );
                MockGlobal.Setup( _ => _.TryGetCountryKey( globalMeta, country, out key ) ).Returns( true );

                var actual = await instance.Query( country, language );
                Assert.Equal( result, actual );
            }

            public static IEnumerable<object[]> DefaultValueCases()
            {
                var results = new CountryMetadata[]
                {
                    new CountryMetadata { Format = Guid.NewGuid().ToString() },
                    new CountryMetadata { Required = new AddressField[] { AddressField.StreetAddress, AddressField.Locality, AddressField.Province } },
                    new CountryMetadata { Uppercase = new AddressField[] { AddressField.Locality, AddressField.Sublocality } },
                    new CountryMetadata { StateType = Guid.NewGuid().ToString() },
                    new CountryMetadata { LocalityType = Guid.NewGuid().ToString() },
                    new CountryMetadata { SublocalityType = Guid.NewGuid().ToString() },
                    new CountryMetadata { PostalCodeType = Guid.NewGuid().ToString() },
                    new CountryMetadata { },
                };

                return
                    from result in results
                    select new object[] { result };
            }

            [Theory]
            [MemberData(nameof(DefaultValueCases))]
            public async Task WhenReturning_coalesces_defaultValues( CountryMetadata result )
            {
                var key = "XW";
                var id = "data/XW";
                var globalMeta = new GlobalMetadata { Id = "data" };
                
                MockClient.Setup( _ => _.Query<CountryMetadata>( "data/ZZ" ) ).ReturnsAsync( defaultMeta );
                MockClient.Setup( _ => _.Query<CountryMetadata>( id ) ).ReturnsAsync( result as CountryMetadata );
                MockGlobal.Setup( _ => _.Query() ).ReturnsAsync( globalMeta );
                MockGlobal.Setup( _ => _.TryGetCountryKey( globalMeta, key, out key ) ).Returns( true );

                // expected return values
                var format = result.Format ?? defaultMeta.Format;
                var required = result.Required ?? defaultMeta.Required;
                var uppercase = result.Uppercase ?? defaultMeta.Uppercase;
                var stateType = result.StateType ?? defaultMeta.StateType;
                var localityType = result.LocalityType ?? defaultMeta.LocalityType;
                var sublocalityType = result.SublocalityType ?? defaultMeta.SublocalityType;
                var postalCodeType = result.PostalCodeType ?? defaultMeta.PostalCodeType;

                var actual = await instance.Query( "XW", null );
                Assert.Equal( format, actual.Format );
                Assert.Equal( required, actual.Required );
                Assert.Equal( uppercase, actual.Uppercase );
                Assert.Equal( stateType, actual.StateType );
                Assert.Equal( localityType, actual.LocalityType );
                Assert.Equal( sublocalityType, actual.SublocalityType );
                Assert.Equal( postalCodeType, actual.PostalCodeType );
            }
        }

        public class TryGetProvinceKey : CountryAdapterTests
        {
            public static IEnumerable<object[]> NotMatchedCases()
            {
                string[] random() => Enumerable.Range( 0, 3 ).Select( _=> new Guid().ToString() ).ToArray();

                var countries = new CountryMetadata[]
                {
                    null,
                    new CountryMetadata { ChildRegionKeys = null, ChildRegionNames = null, ChildRegionLatinNames = null },
                    new CountryMetadata { ChildRegionKeys = random(), ChildRegionNames = random(), ChildRegionLatinNames = random() },
                };

                return countries.Select( _=> new object[] { _ } );
            }

            [Theory]
            [MemberData(nameof(NotMatchedCases))]
            public void WhenNotMatched_returns_false( CountryMetadata countryMeta )
            {
                var actual = instance.TryGetProvinceKey( countryMeta, "XX", out string key );
                Assert.False( actual );
                Assert.Null( key );
            }

            /// <summary>
            /// Cases where the value should be found by key.
            /// The same values are included in each name field in varying positions to ensure that a key match is supreme.
            /// </summary>
            
            public static IEnumerable<object[]> FoundByKeyCases()
            {
                string[] random() => Enumerable.Range( 0, 2 ).Select( _ => new Guid().ToString() ).ToArray();

                var keys = random();
                var children = new IEnumerable<string>[] { null, keys, keys.Reverse() };

                return
                    from expected in keys
                    from value in new string[] { expected.ToLowerInvariant(), expected.ToUpperInvariant() }
                    from names in children
                    from lnames in children
                    select new object[] 
                    {
                        new CountryMetadata { ChildRegionKeys = keys, ChildRegionNames = names, ChildRegionLatinNames = lnames },
                        value,
                        expected,
                    };
            }

            /// <summary>
            /// Cases where the value should be found by name.
            /// The same values are included in the latin name field in varying positions to ensure that a key match is supreme.
            /// </summary>

            public static IEnumerable<object[]> FoundByNameCases()
            {
                string[] random() => Enumerable.Range( 0, 2 ).Select( _ => new Guid().ToString() ).ToArray();
                var indexes = new int[] { 0, 1 };
                var keys = random();
                var names = random();
                var children = new IEnumerable<string>[] { null, names, names.Reverse() };

                return
                    from index in indexes
                    from value in new string[] { names[index].ToLowerInvariant(), names[index].ToUpperInvariant() }
                    from lnames in children
                    select new object[]
                    {
                        new CountryMetadata { ChildRegionKeys = keys, ChildRegionNames = names, ChildRegionLatinNames = lnames },
                        value,
                        keys[index],
                    };
            }

            /// <summary>
            /// Cases where the value should be found by its latin name.
            /// Includes cases where the local names are not present to ensure fallback.
            /// </summary>
            
            public static IEnumerable<object[]> FoundByLatinNameCases()
            {
                string[] random() => Enumerable.Range( 0, 2 ).Select( _ => new Guid().ToString() ).ToArray();
                var indexes = new int[] { 0, 1 };
                var keys = random();
                var lnames = random();
                var children = new IEnumerable<string>[] { null, random() };

                return 
                    from index in indexes
                    from value in new string[] { lnames[index].ToLowerInvariant(), lnames[index].ToUpperInvariant() }
                    from names in children
                    select new object[]
                    {
                        new CountryMetadata { ChildRegionKeys = keys, ChildRegionNames = names, ChildRegionLatinNames = lnames },
                        value,
                        keys[index],
                    };
            }

            [Theory]
            [MemberData(nameof(FoundByKeyCases))]
            [MemberData(nameof(FoundByNameCases))]
            [MemberData(nameof( FoundByLatinNameCases))]
            public void WhenMatched_returns_true_and_outputs_key( CountryMetadata countryMeta, string value, string expected )
            {
                var found = instance.TryGetProvinceKey( countryMeta, value, out string key );
                Assert.True( found );
                Assert.Equal( key, expected );
            }
        }
    }
}