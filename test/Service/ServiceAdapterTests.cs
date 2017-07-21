using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
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
            async Task<IGlobalMetadata> invoke() => await instance.GetGlobal();

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
            GlobalMetadata global = new GlobalMetadata { Id = "data" };
            string country = "XX";
            string language = Guid.NewGuid().ToString();
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

            async Task<ICountryMetadata> invoke() => await instance.GetCountry( global, country, language );

            [Fact]
            public async Task Requires_global()
            {
                global = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof(global), invoke );
            }

            public static IEnumerable<object[]> WhenCountryNotInGlobalKeysCases => new object[][]
            {
                new object[] { null },
                new object[] { Enumerable.Empty<string>() },
                new object[] { new string[] { "XB" } },
            };

            [Theory]
            [MemberData(nameof(WhenCountryNotInGlobalKeysCases))]
            public async Task WhenCountryNotInGlobalKeys_returns_null( IEnumerable<string> countries )
            {
                global.Countries = countries;
                var result = await invoke();
                Assert.Null( result );
            }

            public static IEnumerable<object[]> WhenCountryInGlobalCases => new object[][]
            {
                new object[] { new string[] { "XX", "XB" }, null },
                new object[] { new string[] { "XX", "XB" }, new CountryMetadata { Id = "data/XX", Key = "XX" } },

                // also include scenario where the default value item requested
                new object[] { new string[] { "XX", "XB" }, new CountryMetadata { Id = "data/XX", Key = "ZZ" } },
            };

            [Theory]
            [MemberData(nameof(WhenCountryInGlobalCases))]
            public async Task WhenCountryInGlobal_returns_clientResult( IEnumerable<string> countries, ICountryMetadata expected )
            {
                global.Countries = countries;
                var id = $"{expected?.Id}--{language}";
                MockClient.Setup( _=> _.Query<CountryMetadata>( id ) ).ReturnsAsync( expected as CountryMetadata );

                var actual = await invoke();
                Assert.Equal( expected, actual );
            }

            public static IEnumerable<object[]> GetFilledCountryValues => new object[][]
            {
                new object[] { "XX", new CountryMetadata { Id = "data/XX", Format = Guid.NewGuid().ToString() } },
                new object[] { "XX", new CountryMetadata { Id = "data/XX", Required = new AddressField[] { AddressField.StreetAddress, AddressField.Locality, AddressField.Province } } },
                new object[] { "XX", new CountryMetadata { Id = "data/XX", Uppercase = new AddressField[] { AddressField.Locality, AddressField.Sublocality } } },
                new object[] { "XX", new CountryMetadata { Id = "data/XX", StateType = Guid.NewGuid().ToString() } },
                new object[] { "XX", new CountryMetadata { Id = "data/XX", LocalityType = Guid.NewGuid().ToString() } },
                new object[] { "XX", new CountryMetadata { Id = "data/XX", SublocalityType = Guid.NewGuid().ToString() } },
                new object[] { "XX", new CountryMetadata { Id = "data/XX", PostalCodeType = Guid.NewGuid().ToString() } },
                new object[] { "XX", new CountryMetadata { Id = "data/XX" } },
            };

            /// <summary>
            /// Property values from the default country should be present on the result if those values are null from the service.
            /// </summary>

            [Theory]
            [MemberData( nameof( GetFilledCountryValues ) )]
            public async Task Returns_defaultValues_whenResultValuesNull( string countryKey, ICountryMetadata result )
            {
                language = null;
                country = countryKey;
                global.Countries = new string[] { country };
                MockClient.Setup( _ => _.Query<CountryMetadata>( defaultCountry.Id ) ).ReturnsAsync( defaultCountry ).Verifiable();
                MockClient.Setup( _ => _.Query<CountryMetadata>( result.Id ) ).ReturnsAsync( (CountryMetadata)result ).Verifiable();

                var format = result.Format ?? defaultCountry.Format;
                var required = result.Required ?? defaultCountry.Required;
                var uppercase = result.Uppercase ?? defaultCountry.Uppercase;
                var stateType = result.StateType ?? defaultCountry.StateType;
                var localityType = result.LocalityType ?? defaultCountry.LocalityType;
                var sublocalityType = result.SublocalityType ?? defaultCountry.SublocalityType;
                var postalCodeType = result.PostalCodeType ?? defaultCountry.PostalCodeType;

                var actual = await invoke();
                Assert.Equal( format, actual.Format );
                Assert.Equal( required, actual.Required );
                Assert.Equal( uppercase, actual.Uppercase );
                Assert.Equal( stateType, actual.StateType );
                Assert.Equal( localityType, actual.LocalityType );
                Assert.Equal( sublocalityType, actual.SublocalityType );
                Assert.Equal( postalCodeType, actual.PostalCodeType );

                MockClient.VerifyAll();
            }
        }

        public class GetProvince : ServiceAdapterTests
        {
            ICountryMetadata country;
            string province;
            string language;

            async Task<IProvinceMetadata> invoke() => await instance.GetProvince( country, province, language );

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
            public async Task WhenCountryDoesNotContainProvince_returns_null( ICountryMetadata country, string province, string language )
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
            public async Task WhenCountryContainsProvince_returns_value( ICountryMetadata country, IProvinceMetadata expected, string language, string province )
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