using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Service.Adapters
{
    public class ProvinceAdapterTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();
        Mock<ICountryAdapter> MockCountry = new Mock<ICountryAdapter>();

        IServiceClient client => MockClient?.Object;
        ICountryAdapter country => MockCountry?.Object;
        IProvinceAdapter instance => new ProvinceAdapter( client, country );

        public class Constructor : ProvinceAdapterTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof(client), ()=>instance );
            }

            [Fact]
            public void Requires_country()
            {
                MockCountry = null;
                Assert.Throws<ArgumentNullException>( nameof(country), ()=>instance );
            }
        }

        public class Query : ProvinceAdapterTests
        {
            public static IEnumerable<object[]> QueryUnmatchedTestCases()
            {
                var countries = new CountryMetadata[] { null, new CountryMetadata { Key = "XW" } };
                var provinces = new string[] { null, "XX", };
                var languages = new string[] { null, "en" };

                return
                    from province in provinces
                    from language in languages
                    from countryMeta in countries
                    select new object[] { "XW", province, language, countryMeta };
            }

            [Theory]
            [MemberData( nameof( QueryUnmatchedTestCases ) )]
            public async Task WhenNotMatched_returns_null( string country, string province, string language, ICountryMetadata countryMeta )
            {
                string key = null;
                MockCountry.Setup( _ => _.Query( country, language ) ).ReturnsAsync( countryMeta as CountryMetadata );
                MockCountry.Setup( _ => _.TryGetProvinceKey( countryMeta, province, out key ) ).Returns( false );

                var actual = await instance.Query( country, province, language );
                Assert.Null( actual );

                // ensure no query was issued
                MockClient.Verify( _ => _.Query<CountryMetadata>( It.IsAny<string>() ), Times.Never );
            }

            public static IEnumerable<object[]> QueryMatchedCases()
            {
                var country = "xw";
                var province = "xx";
                var languages = new string[] { null, "en" };
                var results = new ProvinceMetadata[] { null, new ProvinceMetadata() };

                return
                    from language in languages
                    from result in results
                    select new object[] { country, province, language, result };
            }

            [Theory]
            [MemberData( nameof( QueryMatchedCases ) )]
            public async Task WhenMatched_returns_result( string country, string province, string language, IProvinceMetadata result )
            {
                // expected identifier
                string key = "XX";
                var id = $"data/XW/{key}{(language != null ? $"--{language}" : string.Empty)}";
                var countryMeta = new CountryMetadata { Id = "data/XW" };

                MockClient.Setup( _ => _.Query<ProvinceMetadata>( id ) ).ReturnsAsync( result as ProvinceMetadata );
                MockCountry.Setup( _ => _.Query( country, language ) ).ReturnsAsync( countryMeta );
                MockCountry.Setup( _ => _.TryGetProvinceKey( countryMeta, province, out key ) ).Returns( true );

                var actual = await instance.Query( country, province, language );
                Assert.Equal( result, actual );
            }
        }

        public class TryGetLocality : ProvinceAdapterTests
        {
            public static IEnumerable<object[]> NotMatchedCases()
            {
                string[] random() => Enumerable.Range( 0, 3 ).Select( _ => new Guid().ToString() ).ToArray();

                var provinces = new ProvinceMetadata[]
                {
                    null,
                    new ProvinceMetadata { ChildRegionKeys = null, ChildRegionNames = null, ChildRegionLatinNames = null },
                    new ProvinceMetadata { ChildRegionKeys = random(), ChildRegionNames = random(), ChildRegionLatinNames = random() },
                };

                return provinces.Select( _ => new object[] { _ } );
            }

            [Theory]
            [MemberData( nameof( NotMatchedCases ) )]
            public void WhenNotMatched_returns_false( IProvinceMetadata provinceMeta )
            {
                var actual = instance.TryGetLocalityKey( provinceMeta, "XY", out string key );
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
                        new ProvinceMetadata { ChildRegionKeys = keys, ChildRegionNames = names, ChildRegionLatinNames = lnames },
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
                        new ProvinceMetadata { ChildRegionKeys = keys, ChildRegionNames = names, ChildRegionLatinNames = lnames },
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
                        new ProvinceMetadata { ChildRegionKeys = keys, ChildRegionNames = names, ChildRegionLatinNames = lnames },
                        value,
                        keys[index],
                    };
            }

            [Theory]
            [MemberData( nameof( FoundByKeyCases ) )]
            [MemberData( nameof( FoundByNameCases ) )]
            [MemberData( nameof( FoundByLatinNameCases ) )]
            public void WhenMatched_returns_true_and_outputs_key( IProvinceMetadata meta, string value, string expected )
            {
                var found = instance.TryGetLocalityKey( meta, value, out string key );
                Assert.True( found );
                Assert.Equal( key, expected );
            }
        }
    }
}