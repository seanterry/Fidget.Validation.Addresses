using Fidget.Validation.Addresses.Service;
using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using Fidget.Validation.Addresses.Validation;
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

        Mock<IAddressValidator> MockValidator = new Mock<IAddressValidator>();
        IAddressValidator validator => MockValidator?.Object;

        IAddressService create() => new AddressService( client, validator );

        public class Constructor : AddressServiceTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof(client), ()=>create() );
            }

            [Fact]
            public void Requires_validator()
            {
                MockValidator = null;
                Assert.Throws<ArgumentNullException>( nameof(validator), ()=>create() );
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

        public class GetLocalityAsync : AddressServiceTests
        {
            string countryKey = Guid.NewGuid().ToString();
            string provinceKey = Guid.NewGuid().ToString();
            string localityKey = Guid.NewGuid().ToString();
            string language = Guid.NewGuid().ToString();
            async Task<ILocalityMetadata> invoke() => await create().GetLocalityAsync( countryKey, provinceKey, localityKey, language );

            [Fact]
            public async Task Requires_countryKey()
            {
                countryKey = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof( countryKey ), invoke );
            }

            [Fact]
            public async Task Requires_provinceKey()
            {
                provinceKey = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof( provinceKey ), invoke );
            }

            [Fact]
            public async Task Requires_localityKey()
            {
                localityKey = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof( localityKey ), invoke );
            }

            public static IEnumerable<object[]> GetArguments()
            {
                yield return new object[] { "XX", "ZZ", "ZY", null, "data/XX/ZZ/ZY", null };
                yield return new object[] { "XX", "ZZ", "ZY", null, "data/XX/ZZ/ZY", new LocalityMetadata { Id = "data/XX/ZZ/ZY" } };
                yield return new object[] { "XX", "ZZ", "ZY", "xyz", "data/XX/ZZ/ZY--xyz", null };
                yield return new object[] { "XX", "ZZ", "ZY", "xyz", "data/XX/ZZ/ZY--xyz", new LocalityMetadata { Id = "data/XX/ZZ/ZY--xyz" } };
            }

            [Theory]
            [MemberData( nameof(GetArguments) )]
            public async Task Returns_clientResult( string countryKey, string provinceKey, string localityKey, string language, string id, ILocalityMetadata expected )
            {
                this.countryKey = countryKey;
                this.provinceKey = provinceKey;
                this.localityKey = localityKey;
                this.language = language;
                MockClient.Setup( _ => _.Query<LocalityMetadata>( id ) ).ReturnsAsync( (LocalityMetadata)expected ).Verifiable();

                var actual = await invoke();
                Assert.Equal( expected, actual );
                MockClient.VerifyAll();
            }
        }

        public class GetSublocalityAsync : AddressServiceTests
        {
            string countryKey = Guid.NewGuid().ToString();
            string provinceKey = Guid.NewGuid().ToString();
            string localityKey = Guid.NewGuid().ToString();
            string sublocalityKey = Guid.NewGuid().ToString();
            string language = Guid.NewGuid().ToString();
            async Task<ISublocalityMetadata> invoke() => await create().GetSublocalityAsync( countryKey, provinceKey, localityKey, sublocalityKey, language );

            [Fact]
            public async Task Requires_countryKey()
            {
                countryKey = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof( countryKey ), invoke );
            }

            [Fact]
            public async Task Requires_provinceKey()
            {
                provinceKey = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof( provinceKey ), invoke );
            }

            [Fact]
            public async Task Requires_localityKey()
            {
                localityKey = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof( localityKey ), invoke );
            }

            [Fact]
            public async Task Requires_sublocalityKey()
            {
                sublocalityKey = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof( sublocalityKey ), invoke );
            }

            public static IEnumerable<object[]> GetArguments()
            {
                yield return new object[] { "XX", "ZZ", "ZY", "XY", null, "data/XX/ZZ/ZY/XY", null };
                yield return new object[] { "XX", "ZZ", "ZY", "XY", null, "data/XX/ZZ/ZY/XY", new SublocalityMetadata { Id = "data/XX/ZZ/ZY/XY" } };
                yield return new object[] { "XX", "ZZ", "ZY", "XY", "xyz", "data/XX/ZZ/ZY/XY--xyz", null };
                yield return new object[] { "XX", "ZZ", "ZY", "XY", "xyz", "data/XX/ZZ/ZY/XY--xyz", new SublocalityMetadata { Id = "data/XX/ZZ/ZY--xyz" } };
            }

            [Theory]
            [MemberData( nameof(GetArguments) )]
            public async Task Returns_clientResult( string countryKey, string provinceKey, string localityKey, string sublocalityKey, string language, string id, ISublocalityMetadata expected )
            {
                this.countryKey = countryKey;
                this.provinceKey = provinceKey;
                this.localityKey = localityKey;
                this.sublocalityKey = sublocalityKey;
                this.language = language;
                MockClient.Setup( _ => _.Query<SublocalityMetadata>( id ) ).ReturnsAsync( (SublocalityMetadata)expected ).Verifiable();

                var actual = await invoke();
                Assert.Equal( expected, actual );
                MockClient.VerifyAll();
            }
        }

        public class ValidateAsync : AddressServiceTests
        {
            AddressData address;
            async Task<IEnumerable<ValidationFailure>> invoke() => await create().ValidateAsync( address );

            static CountryMetadata country = new CountryMetadata { Id = "data/XW", Key = "XW", ChildRegionKeys = new string[] { "XX", "XA" } };
            static ProvinceMetadata province = new ProvinceMetadata { Id = "data/XW/XX", Key = "XX", ChildRegionKeys = new string[] { "XY", "XA" } };
            static LocalityMetadata locality = new LocalityMetadata { Id = "data/XW/XX/XY", Key = "XY", ChildRegionKeys = new string[] { "XZ", "XA" } };
            static SublocalityMetadata sublocality = new SublocalityMetadata { Id = "data/XW/XX/XY/XZ", Key = "XZ" };

            [Fact]
            public async Task Requires_address()
            {
                address = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof(address), invoke );
            }
            
            public static IEnumerable<object[]> GetTestCases()
            {
                yield return new object[] { null, null, null, null };
                yield return new object[] { country, null, null, null };
                yield return new object[] { country, province, null, null };
                yield return new object[] { country, province, locality, null };
                yield return new object[] { country, province, locality, sublocality };
            }

            [Theory]
            [MemberData(nameof(GetTestCases))]
            public async Task Returns_validatorResult( ICountryMetadata country, IProvinceMetadata province, ILocalityMetadata locality, ISublocalityMetadata sublocality )
            {
                address = new AddressData { Country = "XW", Province = "XX", Locality = "XY", Sublocality = "XZ" };
                var expected = new ValidationFailure[3];
                
                if ( country is CountryMetadata cm ) MockClient.Setup( _=> _.Query<CountryMetadata>( cm.Id ) ).ReturnsAsync( cm ).Verifiable();
                if ( province is ProvinceMetadata pm ) MockClient.Setup( _=> _.Query<ProvinceMetadata>( pm.Id ) ).ReturnsAsync( pm ).Verifiable();
                if ( locality is LocalityMetadata lm ) MockClient.Setup( _ => _.Query<LocalityMetadata>( lm.Id ) ).ReturnsAsync( lm ).Verifiable();
                if ( sublocality is SublocalityMetadata sm ) MockClient.Setup( _ => _.Query<SublocalityMetadata>( sm.Id ) ).ReturnsAsync( sm ).Verifiable();

                MockValidator.Setup( _=> _.Validate( address, country, province, locality, sublocality ) ).Returns( expected ).Verifiable();

                var actual = await invoke();
                Assert.Same( expected, actual );

                MockValidator.VerifyAll();
                MockClient.VerifyAll();
            }
        }
    }
}