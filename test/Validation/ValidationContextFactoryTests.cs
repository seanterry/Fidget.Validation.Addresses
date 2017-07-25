using Fidget.Validation.Addresses.Metadata;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Validation
{
    public class ValidationContextFactoryTests
    {
        IEnumerable<IAddressValidator> validators = new IAddressValidator[0];
        IValidationContextFactory instance => new ValidationContext.Factory( validators );

        public class Constructor : ValidationContextFactoryTests
        {
            [Fact]
            public void Requires_validators()
            {
                validators = null;
                Assert.Throws<ArgumentNullException>( nameof(validators), ()=>instance );
            }

            [Fact]
            public void Validators_populated()
            {
                var actual = instance;
                Assert.Same( validators, actual.Validators );
            }
        }

        public class Create : ValidationContextFactoryTests
        {
            Mock<IAddressService> MockService = new Mock<IAddressService>();

            AddressData address = new AddressData();
            IAddressService service => MockService?.Object;
            string language = Guid.NewGuid().ToString();

            async Task<IValidationContext> invoke() => await instance.Create( address, service, language );

            [Fact]
            public async Task Requires_address()
            {
                address = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof(address), invoke );
            }

            [Fact]
            public async Task Requires_service()
            {
                MockService = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof(service), invoke );
            }

            GlobalMetadata global = new GlobalMetadata();
            CountryMetadata country = new CountryMetadata { Key = Guid.NewGuid().ToString() };
            ProvinceMetadata province = new ProvinceMetadata { Key = Guid.NewGuid().ToString() };
            LocalityMetadata locality = new LocalityMetadata { Key = Guid.NewGuid().ToString() };
            SublocalityMetadata sublocality = new SublocalityMetadata { Key = Guid.NewGuid().ToString() };

            /// <summary>
            /// Provides test case data for whether keys should be found for the given fields.
            /// </summary>
            
            public static IEnumerable<object[]> CreateCases()
            {
                var bools = new bool[] { true, false };

                return new object[][]
                {
                    // child objects should only be found if their parents are found
                    new object[] { false, false, false, false },
                    new object[] { true, false, false, false },
                    new object[] { true, true, false, false },
                    new object[] { true, true, true, false },
                    new object[] { true, true, true, true },
                };
            }

            [Theory]
            [MemberData(nameof(CreateCases))]
            public async Task WhenKeyFound_ShouldPopulate( bool findCountry, bool findProvince, bool findLocality, bool findSublocality )
            {
                var address = new AddressData
                {
                    Country = "XW",
                    Province = "XX",
                    Locality = "XY",
                    Sublocality = "XZ",
                };

                var countryKey = country.Key;
                var provinceKey = province.Key;
                var localityKey = locality.Key;
                var sublocalityKey = sublocality.Key;

                MockService.Setup( _=> _.GetGlobalAsync() ).ReturnsAsync( global );
                MockService.Setup( _=> _.GetCountryAsync( country.Key, language ) ).ReturnsAsync( country );
                MockService.Setup( _=> _.GetProvinceAsync( country.Key, province.Key, language ) ).ReturnsAsync( province );
                MockService.Setup( _=> _.GetLocalityAsync( country.Key, province.Key, locality.Key, language ) ).ReturnsAsync( locality );
                MockService.Setup( _=> _.GetSublocalityAsync( country.Key, province.Key, locality.Key, sublocality.Key, language ) ).ReturnsAsync( sublocality );

                if ( findCountry ) MockService.Setup( _=> _.TryGetCountryKey( global, address.Country, out countryKey ) ).Returns( true );
                if ( findProvince ) MockService.Setup( _=> _.TryGetChildKey( country, address.Province, out provinceKey ) ).Returns( true );
                if ( findLocality ) MockService.Setup( _=> _.TryGetChildKey( province, address.Locality, out localityKey ) ).Returns( true );
                if ( findSublocality ) MockService.Setup( _=> _.TryGetChildKey( locality, address.Sublocality, out sublocalityKey ) ).Returns( true );

                var actual = await instance.Create( address, service, language );
                var context = Assert.IsType<ValidationContext>( actual );

                Assert.Equal( global, context.Global );
                if ( findCountry ) Assert.Equal( country, context.Country ); else Assert.Null( context.Country );
                if ( findProvince ) Assert.Equal( province, context.Province ); else Assert.Null( context.Province );
                if ( findLocality ) Assert.Equal( locality, context.Locality ); else Assert.Null( context.Locality );
                if ( findSublocality ) Assert.Equal( sublocality, context.Sublocality ); else Assert.Null( context.Sublocality );
            }
        }
    }
}