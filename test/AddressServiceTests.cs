using Fidget.Extensions.Reflection;
using Fidget.Validation.Addresses.Service;
using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses
{
    public class AddressServiceTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();
        IServiceClient client => MockClient?.Object;

        IAddressService create() => new AddressService( client );

        public class Constructor : AddressServiceTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof(client), ()=>create() );
            }

            [Fact]
            public void Implements_IAddressService()
            {
                var actual = create();
                Assert.IsType<AddressService>( actual );
                Assert.IsAssignableFrom<IAddressService>( actual );
            }
        }

        public class GetGlobalAsync : AddressServiceTests
        {
            /// <summary>
            /// Client response setup.
            /// </summary>
            
            public static IEnumerable<object[]> GetGlobalResponses()
            {
                yield return new object[] { new GlobalMetadata { Id = "data" } };
                yield return new object[] { default(GlobalMetadata) };
            }

            async Task<IGlobalMetadata> invoke( IAddressService instance ) => await instance.GetGlobalAsync();

            [Theory]
            [MemberData(nameof(GetGlobalResponses))]
            public async Task Returns_valueFromClient( IGlobalMetadata expected )
            {
                MockClient.Setup( _=> _.Query<GlobalMetadata>( "data" ) ).ReturnsAsync( (GlobalMetadata)expected ).Verifiable();
                var instance = create();
                var actual = await invoke( instance );

                Assert.Equal( expected, actual );
                MockClient.VerifyAll();
            }
        }

        public class GetCountryAsync : AddressServiceTests
        {
            string countryKey;
            string language;
            CountryMetadata defaultCountry = new CountryMetadata
            {
                Id = "data/ZZ",
                Format = "%N%n%O%n%A%n%C",
                Required = "AC",
                Uppercase = "C",
                StateType = "province",
                LocalityType = "city",
                SublocalityType = "suburb",
                PostalCodeType = "postal",
            };

            async Task<ICountryMetadata> invoke( IAddressService instance ) => await instance.GetCountryAsync( countryKey, language );

            [Fact]
            public async Task Requires_countryKey()
            {
                countryKey = null;
                var instance = create();
                await Assert.ThrowsAsync<ArgumentNullException>( nameof(countryKey), async ()=> await invoke( instance ) );
            }

            /// <summary>
            /// Country scenarios.
            /// </summary>

            public static IEnumerable<object[]> GetCountryValues()
            {
                yield return new object[] { "XX", null, "data/XX", new CountryMetadata { Id = "data/XX" } };
                yield return new object[] { "XX", null, "data/XX", null };
                yield return new object[] { "XX", "abc", "data/XX--abc", new CountryMetadata { Id = "data/XX--abc" } };
                yield return new object[] { "XX", "abc", "data/XX--abc", null };
            }

            [Theory]
            [MemberData(nameof(GetCountryValues))]
            public async Task Returns_valueFromServiceClient( string countryKey, string language, string id, ICountryMetadata expected )
            {
                this.countryKey = countryKey;
                this.language = language;
                MockClient.Setup( _ => _.Query<CountryMetadata>( defaultCountry.Id ) ).ReturnsAsync( defaultCountry ).Verifiable();
                MockClient.Setup( _=> _.Query<CountryMetadata>( id ) ).ReturnsAsync( (CountryMetadata)expected ).Verifiable();
                var instance = create();
                var actual = await invoke( instance );

                Assert.Equal( expected, actual );
                MockClient.VerifyAll();
            }

            public static IEnumerable<object[]> GetFilledCountryValues()
            {
                yield return new object[] { "XX", new CountryMetadata { Id = "data/XX", Format = Guid.NewGuid().ToString() } };
                yield return new object[] { "XX", new CountryMetadata { Id = "data/XX", Required = Guid.NewGuid().ToString() } };
                yield return new object[] { "XX", new CountryMetadata { Id = "data/XX", Uppercase = Guid.NewGuid().ToString() } };
                yield return new object[] { "XX", new CountryMetadata { Id = "data/XX", StateType = Guid.NewGuid().ToString() } };
                yield return new object[] { "XX", new CountryMetadata { Id = "data/XX", LocalityType = Guid.NewGuid().ToString() } };
                yield return new object[] { "XX", new CountryMetadata { Id = "data/XX", SublocalityType = Guid.NewGuid().ToString() } };
                yield return new object[] { "XX", new CountryMetadata { Id = "data/XX", PostalCodeType = Guid.NewGuid().ToString() } };
                yield return new object[] { "XX", new CountryMetadata { Id = "data/XX" } };
            }

            /// <summary>
            /// Property values from the default country should be present on the result if those values are null from the service.
            /// </summary>
            
            [Theory]
            [MemberData(nameof(GetFilledCountryValues))]
            public async Task Returns_defaultValues_whenResultValuesNull( string countryKey, ICountryMetadata result )
            {
                this.countryKey = countryKey;
                language = null;
                MockClient.Setup( _=> _.Query<CountryMetadata>( defaultCountry.Id ) ).ReturnsAsync( defaultCountry ).Verifiable();
                MockClient.Setup( _=> _.Query<CountryMetadata>( result.Id ) ).ReturnsAsync( (CountryMetadata)result ).Verifiable();

                var expected = new CountryMetadata
                {
                    Format = result.Format ?? defaultCountry.Format,
                    Required = result.Required ?? defaultCountry.Required,
                    Uppercase = result.Uppercase ?? defaultCountry.Uppercase,
                    StateType = result.StateType ?? defaultCountry.StateType,
                    LocalityType = result.LocalityType ?? defaultCountry.LocalityType,
                    SublocalityType = result.SublocalityType ?? defaultCountry.SublocalityType,
                    PostalCodeType = result.PostalCodeType ?? defaultCountry.PostalCodeType,
                };

                var instance = create();
                var actual = await invoke( instance );
                
                Assert.Equal( expected.Format, result.Format );
                Assert.Equal( expected.Required, result.Required );
                Assert.Equal( expected.Uppercase, result.Uppercase );
                Assert.Equal( expected.StateType, result.StateType );
                Assert.Equal( expected.LocalityType, result.LocalityType );
                Assert.Equal( expected.SublocalityType, result.SublocalityType );
                Assert.Equal( expected.PostalCodeType, result.PostalCodeType );

                MockClient.VerifyAll();
            }
        }

        public class GetProvinceAsync : AddressServiceTests
        {
            string countryKey = Guid.NewGuid().ToString();
            string provinceKey = Guid.NewGuid().ToString();
            string language = Guid.NewGuid().ToString();
            async Task<IProvinceMetadata> invoke() => await create().GetProvinceAsync( countryKey, provinceKey, language );

            [Fact]
            public async Task Requires_countryKey()
            {
                countryKey = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof(countryKey), invoke );
            }

            [Fact]
            public async Task Requires_provinceKey()
            {
                provinceKey = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof( provinceKey ), invoke );
            }

            public static IEnumerable<object[]> GetArguments()
            {
                yield return new object[] { "XX", "ZZ", null, "data/XX/ZZ", null };
                yield return new object[] { "XX", "ZZ", null, "data/XX/ZZ", new ProvinceMetadata { Id = "data/XX/ZZ" } };
                yield return new object[] { "XX", "ZZ", "xyz", "data/XX/ZZ--xyz", null };
                yield return new object[] { "XX", "ZZ", "xyz", "data/XX/ZZ--xyz", new ProvinceMetadata { Id = "data/XX/ZZ--xyz" } };
            }

            [Theory]
            [MemberData(nameof(GetArguments))]
            public async Task Returns_clientResult( string countryKey, string provinceKey, string language, string id, IProvinceMetadata expected )
            {
                this.countryKey = countryKey;
                this.provinceKey = provinceKey;
                this.language = language;
                MockClient.Setup( _=> _.Query<ProvinceMetadata>( id ) ).ReturnsAsync( (ProvinceMetadata) expected ).Verifiable();

                var actual = await invoke();
                Assert.Equal( expected, actual );
                MockClient.VerifyAll();
            }
        }
    }
}