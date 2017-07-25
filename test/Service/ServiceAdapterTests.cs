using Fidget.Validation.Addresses.Metadata;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Service
{
    public class ServiceAdapterTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();
        IServiceClient client => MockClient?.Object;
        IServiceAdapter instance => new ServiceAdapter( client );

        public class Constructor : ServiceAdapterTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof(client), ()=>instance );
            }
        }

        public class GetGlobal : ServiceAdapterTests
        {
            async Task<GlobalMetadata> invoke() => await instance.GetGlobal();

            [Fact]
            public async Task Returns_metadataFromClient()
            {
                var expected = new GlobalMetadata();
                MockClient.Setup( _=> _.Query<GlobalMetadata>( "data" ) ).ReturnsAsync( expected );

                var actual = await invoke();
                Assert.Same( expected, actual );
            }
        }

        public class GetCountry : ServiceAdapterTests
        {
            CountryMetadata defaultCountry = new CountryMetadata
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

            public static IEnumerable<object[]> WhenCountryNotInGlobalCases()
            {
                var languages = new string[] { null, "en" };
                var countriesThatShouldNotMatch = new string[] { null, string.Empty, "XB" };

                // collections of countries that should not match any country
                var globalChildren = new IEnumerable<string>[]
                {
                    null,
                    Enumerable.Empty<string>(),
                    new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
                };

                return
                    from country in countriesThatShouldNotMatch
                    from children in globalChildren
                    from language in languages
                    select new object[] 
                    { 
                        new GlobalMetadata { Countries = children },
                        country,
                        language,
                    };
            }

            [Theory]
            [InlineData( null, "XW", null )]
            [MemberData(nameof(WhenCountryNotInGlobalCases))]
            public async Task WhenCountryNotInGlobal_returns_null( GlobalMetadata global, string country, string language )
            {
                MockClient.Setup( _=> _.Query<GlobalMetadata>( "data" ) ).ReturnsAsync( global as GlobalMetadata );
                var result = await instance.GetCountry( country, language );
                Assert.Null( result );
            }

            public static IEnumerable<object[]> WhenCountryInGlobal()
            {
                var languages = new string[] { null, "en", };
                var countries = new string[] { "XW", "xw", };

                return
                    from country in countries
                    from language in languages
                    select new object[]
                    {
                        new GlobalMetadata { Countries = new string[] { "XA", "XW", "XB" } },
                        country,
                        language,
                        new CountryMetadata { Id = $"data/XW{( language != null ? $"--{language}" : null )}" },
                    };
            }

            [Theory]
            [MemberData(nameof(WhenCountryInGlobal))]
            public async Task WhenCountryInGlobal_returns_clientResult( GlobalMetadata global, string country, string language, CountryMetadata expected )
            {
                MockClient.Setup( _=> _.Query<GlobalMetadata>( "data" ) ).ReturnsAsync( global as GlobalMetadata );
                MockClient.Setup( _=> _.Query<CountryMetadata>( defaultCountry.Id ) ).ReturnsAsync( defaultCountry );
                MockClient.Setup( _=> _.Query<CountryMetadata>( expected.Id ) ).ReturnsAsync( expected as CountryMetadata );

                var actual = await instance.GetCountry( country, language );
                Assert.Equal( expected, actual );
            }

            public static IEnumerable<object[]> WhenCountryInGlobalCases_Defaults()
            {
                var languages = new string[] { null, "en", };
                var countries = new string[] { "XW", "xw", };
                var global = new GlobalMetadata { Countries = new string[] { "XA", "XW", "XB" } };
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
                    from country in countries
                    from language in languages
                    select new object[]
                    {
                        global,
                        country,
                        language,
                        new CountryMetadata
                        {
                            Id = $"data/XW{( language != null ? $"--{language}" : null )}",
                            Format = result.Format,
                            Required = result.Required,
                            Uppercase = result.Uppercase,
                            StateType = result.StateType,
                            LocalityType = result.LocalityType,
                            SublocalityType = result.SublocalityType,
                            PostalCodeType = result.PostalCodeType,
                        },
                    };
            }

            [Theory]
            [MemberData( nameof( WhenCountryInGlobalCases_Defaults ) )]
            public async Task WhenCountryInGlobal_returns_clientResultWithDefaults( GlobalMetadata global, string country, string language, CountryMetadata expected )
            {
                MockClient.Setup( _ => _.Query<GlobalMetadata>( "data" ) ).ReturnsAsync( global as GlobalMetadata );
                MockClient.Setup( _ => _.Query<CountryMetadata>( defaultCountry.Id ) ).ReturnsAsync( defaultCountry );
                MockClient.Setup( _ => _.Query<CountryMetadata>( expected.Id ) ).ReturnsAsync( expected as CountryMetadata );

                var format = expected.Format ?? defaultCountry.Format;
                var required = expected.Required ?? defaultCountry.Required;
                var uppercase = expected.Uppercase ?? defaultCountry.Uppercase;
                var stateType = expected.StateType ?? defaultCountry.StateType;
                var localityType = expected.LocalityType ?? defaultCountry.LocalityType;
                var sublocalityType = expected.SublocalityType ?? defaultCountry.SublocalityType;
                var postalCodeType = expected.PostalCodeType ?? defaultCountry.PostalCodeType;

                var actual = await instance.GetCountry( country, language );
                Assert.Equal( expected, actual );
                Assert.Equal( format, actual.Format );
                Assert.Equal( required, actual.Required );
                Assert.Equal( uppercase, actual.Uppercase );
                Assert.Equal( stateType, actual.StateType );
                Assert.Equal( localityType, actual.LocalityType );
                Assert.Equal( sublocalityType, actual.SublocalityType );
                Assert.Equal( postalCodeType, actual.PostalCodeType );
            }
        }

        public class GetProvince : ServiceAdapterTests
        {
            CountryMetadata country;
            string province;
            string language;

            async Task<ProvinceMetadata> invoke() => await instance.GetProvince( country, province, language );

            public static IEnumerable<object[]> CountryDoesNotContainProvinceCases()
            {
                var values = new string[] { "XX", "XY", "XZ" };
                var countries = new CountryMetadata[]
                {
                    null,
                    new CountryMetadata(),
                    new CountryMetadata { ChildRegionKeys = values, ChildRegionNames = values, ChildRegionLatinNames = values },
                };

                var provinces = new string[] { null, string.Empty, "XA" };
                var languages = new string[] { null, "en" };

                return
                    from country in countries
                    from province in provinces
                    from language in languages
                    select new object[] { country, province, language };
            }

            [Theory]
            [MemberData(nameof(CountryDoesNotContainProvinceCases))]
            public async Task WhenCountryDoesNotContainProvince_returns_null( CountryMetadata country, string province, string language )
            {
                this.country = country;
                this.province = province;
                this.language = language;

                var actual = await invoke();
                Assert.Null( actual );
                MockClient.Verify( _=> _.Query<ProvinceMetadata>( It.IsAny<string>() ), Times.Never );
            }

            public static IEnumerable<object[]> CountryContainsProvinceCases()
            {
                var languages = new string[] { null, "en" };
                var keys = new string[] { "XA", "XB", "XC" };
                var provinces = keys.Select( _=> new ProvinceMetadata { Key = _, Name = $"{_}Name", LatinName = $"{_}Latin" } ).ToArray();
                var country = new CountryMetadata 
                { 
                    Id = "data/XX", 
                    ChildRegionKeys = provinces.Select( _=> _.Key ).ToArray(),
                    ChildRegionNames = provinces.Select( _=> _.Name ).ToArray(),
                    ChildRegionLatinNames = provinces.Select( _=> _.LatinName ).ToArray(),
                };
                
                return
                    from expected in provinces
                    from language in languages
                    from province in new string[] { expected.Key, expected.Name, expected.LatinName }
                    select new object[] { country, expected, language, province };
            }

            [Theory]
            [MemberData(nameof(CountryContainsProvinceCases))]
            public async Task WhenCountryContainsProvince_returns_value( CountryMetadata country, ProvinceMetadata expected, string language, string province )
            {
                this.country = country;
                this.language = language;
                this.province = province;
                var id = $"{country.Id}/{expected.Key}{( language != null ? $"--{language}" : string.Empty )}";

                MockClient.Setup( _=> _.Query<ProvinceMetadata>( id ) ).ReturnsAsync( expected as ProvinceMetadata ).Verifiable();
                var actual = await invoke();
                Assert.Equal( expected, actual );
            }
        }
    }
}