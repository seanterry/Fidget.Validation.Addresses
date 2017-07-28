using Fidget.Validation.Addresses.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fidget.Validation.Addresses.Client
{
    public class KeyBuilderTests
    {
        IKeyBuilder instance => new KeyBuilder();

        public class Registration
        {
            [Fact]
            public void IsRegistered()
            {
                var actual = DependencyInjection.Container.GetInstance<IKeyBuilder>();
                Assert.IsType<KeyBuilder>( actual );
            }
        }

        public class GetChildKey : KeyBuilderTests
        {
            /// <summary>
            /// Cases where metadata is null.
            /// </summary>
            
            public static IEnumerable<object[]> MetadataNullCases = new object[][]
            {
                new object[] { null, null },
                new object[] { null, "XW" },
            };

            /// <summary>
            /// Global metadata with null or empty country keys.
            /// </summary>
            
            public static IEnumerable<object[]> NoCountriesInGlobalCases = new object[][]
            {
                new object[] { new GlobalMetadata { Countries = null }, "XW" },
                new object[] { new GlobalMetadata { Countries = new string[0] }, "XW" },
            };

            /// <summary>
            /// Regional metadata with null or empty child keys.
            /// </summary>
            
            public static IEnumerable<object[]> NoKeysInRegionCases()
            {
                var names = new string[] { "XW", "XX", "XY" };
                var collections = new string[][]
                {
                    null,
                    new string[0],
                };

                return
                    from keys in collections
                    select new object[] 
                    { 
                        new CountryMetadata { ChildRegionKeys = keys, ChildRegionNames = names, ChildRegionLatinNames = names, }, 
                        "XX",
                    };
            }

            /// <summary>
            /// Metadata with filled keys/names that don't match the search value.
            /// </summary>
            
            public static IEnumerable<object[]> NoMatchingValueCases()
            {
                var names = new string[] { "XW", "XX", "XY" };
                var parents = new CommonMetadata[]
                {
                    new GlobalMetadata { Countries = names },
                    new CountryMetadata { ChildRegionKeys = names, ChildRegionNames = names, ChildRegionLatinNames = names },
                };

                return
                    from parent in parents
                    select new object[] { parent, "ZZ" };
            }

            [Theory]
            [MemberData(nameof(MetadataNullCases))]
            [MemberData(nameof(NoCountriesInGlobalCases))]
            [MemberData(nameof(NoKeysInRegionCases))]
            [MemberData(nameof(NoMatchingValueCases))]
            public void WhenValueNotFound_returns_null( CommonMetadata parent, string value )
            {
                var actual = instance.GetChildKey( parent, value );
                Assert.Null( actual );
            }

            /// <summary>
            /// Cases where the key value should be matched for a country.
            /// Matching should occur without regard to case.
            /// </summary>
            
            public static IEnumerable<object[]> MatchedCountryKeyCases()
            {
                var keys = Enumerable.Range( 0, 2 )
                    .Select( _=> Convert.ToBase64String( Guid.NewGuid().ToByteArray() ) )
                    .ToArray();
                    
                return
                    from key in keys
                    from value in new string[] { key, key.ToLowerInvariant(), key.ToUpperInvariant() }
                    select new object[]
                    {
                        new GlobalMetadata { Countries = keys },
                        value,
                        key,
                    };
            }

            /// <summary>
            /// Cases where the value should match a key in the region.
            /// </summary>
            
            public static IEnumerable<object[]> MatchedRegionByKeyCases()
            {
                // creates an array of randomish strings
                string[] random() => Enumerable.Range( 0, 2 )
                    .Select( _ => Convert.ToBase64String( Guid.NewGuid().ToByteArray() ) )
                    .ToArray();

                var keys = random();
               
                return
                    from key in keys
                    // should match any case
                    from value in new string[] { key, key.ToLowerInvariant(), key.ToUpperInvariant() }
                    // set up a collision in names to assert that key matching takes precedence over all names
                    from names in new IEnumerable<string>[] { null, new string[0], keys.Reverse() }
                    select new object[]
                    {
                        new CountryMetadata { ChildRegionKeys = keys, ChildRegionNames = names, ChildRegionLatinNames = names },
                        value,
                        key,
                    };
            }

            /// <summary>
            /// Cases where the value should match a local name in the region.
            /// </summary>
            
            public static IEnumerable<object[]> MatchedRegionByLocalNameCases()
            {
                // creates an array of randomish strings
                string[] random() => Enumerable.Range( 0, 2 )
                    .Select( _ => Convert.ToBase64String( Guid.NewGuid().ToByteArray() ) )
                    .ToArray();

                var keys = random();
                var names = random();
                
                return
                    from index in new int[] { 0, 1 }
                    // should match any case
                    from value in new string[] { names[index], names[index].ToUpperInvariant(), names[index].ToLowerInvariant() }
                    // set up a collision in latin names to assert that local matching takes precedence over latin names
                    from lnames in new IEnumerable<string>[] { null, new string[0], names.Reverse() }
                    select new object[]
                    {
                        new CountryMetadata { ChildRegionKeys = keys, ChildRegionNames = names, ChildRegionLatinNames = lnames },
                        value,
                        keys[index],
                    };
            }

            /// <summary>
            /// Cases where the value should match a latin name in the region.
            /// </summary>
            
            public static IEnumerable<object[]> MatchedRegionByLatinNameCases()
            {
                // creates an array of randomish strings
                string[] random() => Enumerable.Range( 0, 2 )
                    .Select( _ => Convert.ToBase64String( Guid.NewGuid().ToByteArray() ) )
                    .ToArray();

                var keys = random();
                var lnames = random();

                return
                    from index in new int[] { 0, 1 }
                    // should match any case
                    from value in new string[] { lnames[index], lnames[index].ToUpperInvariant(), lnames[index].ToLowerInvariant() }
                    from names in new IEnumerable<string>[] { null, new string[0], random() }
                    select new object[]
                    {
                        new CountryMetadata { ChildRegionKeys = keys, ChildRegionNames = names, ChildRegionLatinNames = lnames },
                        value,
                        keys[index],
                    };
            }

            [Theory]
            [MemberData(nameof(MatchedCountryKeyCases))]
            [MemberData(nameof(MatchedRegionByKeyCases))]
            [MemberData(nameof(MatchedRegionByLocalNameCases))]
            [MemberData(nameof(MatchedRegionByLatinNameCases))]
            public void WhenValueFound_returns_key( CommonMetadata parent, string value, string key )
            {
                var actual = instance.GetChildKey( parent, value );
                Assert.Equal( key, actual );
            }
        }

        public class BuildIdentifier : KeyBuilderTests
        {
            CommonMetadata parent = new CountryMetadata { Id = "data/XX--en" };
            string key;
            string language;

            string invoke() => instance.BuildIdentifier( parent, key, language );

            [Fact]
            public void Requires_parent()
            {
                parent = null;
                Assert.Throws<ArgumentNullException>( nameof(parent), ()=>invoke() );
            }

            [Fact]
            public void Requires_key()
            {
                key = null;
                Assert.Throws<ArgumentNullException>( nameof(key), ()=>invoke() );
            }

            /// <summary>
            /// Test cases for identifiers with no language component.
            /// </summary>
            
            public static IEnumerable<object[]> IdentifierWithNoLanguageCases = new object[][]
            {
                new object[] { new CountryMetadata { Id = "data/XW" }, "XA", null, "data/XW/XA" },
                new object[] { new CountryMetadata { Id = "data/XX--en" }, "XB", null, "data/XX/XB" },
            };

            /// <summary>
            /// Test cases for identifiers with language components.
            /// </summary>
            
            public static IEnumerable<object[]> IdentifierWithLanguageCases = new object[][]
            {
                new object[] { new CountryMetadata { Id = "data/XW" }, "XA", "en", "data/XW/XA--en" },
                new object[] { new CountryMetadata { Id = "data/XW--en" }, "XB", "fr", "data/XW/XB--fr" },
            };

            [Theory]
            [MemberData(nameof(IdentifierWithNoLanguageCases))]
            [MemberData(nameof(IdentifierWithLanguageCases))]
            public void Returns_buildIdentifier( CommonMetadata parent, string key, string language, string id )
            {
                this.parent = parent;
                this.key = key;
                this.language = language;
                
                var actual = invoke();
                Assert.Equal( id, actual );
            }
        }
    }
}