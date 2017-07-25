using Fidget.Validation.Addresses.Metadata;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fidget.Validation.Addresses.Adapters
{
    public class KeyServiceTests
    {
        IKeyService instance => new KeyService();

        public class TryGetCountryKey : KeyServiceTests
        {
            public static IEnumerable<object[]> NotMatchedCases()
            {
                var countries = new string[] { null, string.Empty, "XW" };
                var globals = new GlobalMetadata[]
                {
                    null,
                    new GlobalMetadata { Countries = null },
                    new GlobalMetadata { Countries = new string[0] },
                    new GlobalMetadata { Countries = new string[] { "XA", "XB" } },
                };

                return
                    from country in countries
                    from globalMeta in globals
                    select new object[] { globalMeta, country }; 
            }

            [Theory]
            [MemberData(nameof(NotMatchedCases))]
            public void WhenNotMatched_returns_false( GlobalMetadata globalMeta, string country )
            {
                var found = instance.TryGetCountryKey( globalMeta, country, out string key );
                Assert.False( found );
                Assert.Null( key );
            }

            public static IEnumerable<object[]> MatchedCases()
            {
                var keys = new string[] { "XW", "XY", "XZ" };
                
                return
                    from key in keys
                    from country in new string[] { key.ToLowerInvariant(), key.ToUpperInvariant() }
                    select new object[]
                    {
                        new GlobalMetadata { Countries = keys },
                        country,
                        key,
                    };
            }

            [Theory]
            [MemberData(nameof(MatchedCases))]
            public void WhenMatched_returns_true( GlobalMetadata globalMeta, string country, string expected )
            {
                var found = instance.TryGetCountryKey( globalMeta, country, out string key );
                Assert.True( found );
                Assert.Equal( expected, key );
            }
        }

        public class TryGetChildKey : KeyServiceTests
        {
            public static IEnumerable<object[]> NullMetadataCases()
            {
                return
                    from value in new string[] { null, string.Empty, "XW" }
                    select new object[] { null, value };
            }

            public static IEnumerable<object[]> NotMatchedCases()
            {
                var values = new string[] { null, string.Empty, "XW" };
                var collections = new string[][]
                {
                    null,
                    new string[0],
                    new string[] { "XA", "XB", "XC" },
                };
                
                return 
                    from value in values
                    from keys in collections
                    from names in collections
                    from lnames in collections
                    select new object[]
                    {
                        new CountryMetadata { ChildRegionKeys = keys, ChildRegionNames = names, ChildRegionLatinNames = lnames },
                        value,
                    };
            }

            [Theory]
            [MemberData(nameof(NullMetadataCases))]
            [MemberData(nameof(NotMatchedCases))]
            public void WhenNotMatched_returns_false( RegionalMetadata meta, string value )
            {
                var found = instance.TryGetChildKey( meta, value, out string key );
                Assert.False( found );
                Assert.Null( key );
            }

            public static IEnumerable<object[]> MatchedByKeyCases()
            {
                var keys = new string[] { "XA", "XB" };

                // all manner of variations for names to verify that matching by key takes precedence
                var collections = new string[][] { null, new string[0], keys, keys.Reverse().ToArray() };

                return
                    from key in keys
                    from value in new string[] { key.ToLowerInvariant(), key.ToUpperInvariant() }
                    from names in collections
                    from lnames in collections
                    select new object[]
                    {
                        new CountryMetadata { ChildRegionKeys = keys, ChildRegionNames = names, ChildRegionLatinNames = lnames },
                        value,
                        key,
                    };
            }

            public static IEnumerable<object[]> MatchedByNameCases()
            {
                var keys = new string[] { "XA", "XB" };
                var names = new string[] { "Name1", "Name2" };

                // all manner of variations for names to verify that matching by name takes precedence over latin names
                var collections = new string[][] { null, new string[0], names, names.Reverse().ToArray() };

                return
                    from index in new int[] { 0, 1 }
                    from value in new string[] { names[index], names[index].ToLowerInvariant(), names[index].ToUpperInvariant() }
                    from lnames in collections
                    select new object[]
                    {
                        new CountryMetadata { ChildRegionKeys = keys, ChildRegionNames = names, ChildRegionLatinNames = lnames },
                        value,
                        keys[index],
                    };
            }

            public static IEnumerable<object[]> MatchedByLatinNameCases()
            {
                var keys = new string[] { "XA", "XB" };
                var lnames = new string[] { "LatinName1", "LatinName2" };
                var collections = new string[][] { null, new string[0], new string[] { "Name1", "Name2" } };

                return
                    from index in new int[] { 0, 1 }
                    from value in new string[] { lnames[index], lnames[index].ToLowerInvariant(), lnames[index].ToUpperInvariant() }
                    from names in collections
                    select new object[]
                    {
                        new CountryMetadata { ChildRegionKeys = keys, ChildRegionNames = names, ChildRegionLatinNames = lnames },
                        value,
                        keys[index],
                    };
            }

            [Theory]
            [MemberData(nameof(MatchedByKeyCases))]
            [MemberData(nameof(MatchedByNameCases))]
            [MemberData(nameof(MatchedByLatinNameCases))]
            public void WhenMatched_returns_true( RegionalMetadata meta, string value, string expected )
            {
                var found = instance.TryGetChildKey( meta, value, out string key );
                Assert.True( found );
                Assert.Equal( expected, key );
            }
        }
    }
}