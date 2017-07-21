using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using Fidget.Validation.Addresses.Validation;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Fidget.Validation.Addresses
{
    public class AddressServiceExtensionsTests
    {
        Mock<IAddressService> MockService = new Mock<IAddressService>();
        IAddressService service => MockService?.Object;

        public class GetGlobal : AddressServiceExtensionsTests
        {
            IGlobalMetadata invoke() => service.GetGlobal();

            [Fact]
            public void Requires_service()
            {
                MockService = null;
                Assert.Throws<ArgumentNullException>( nameof(service), ()=>invoke() );
            }

            [Fact]
            public void Returns_serviceResult()
            {
                IGlobalMetadata expected = new GlobalMetadata();
                MockService.Setup( _=> _.GetGlobalAsync() ).ReturnsAsync( expected );

                var actual = invoke();
                Assert.Same( expected, actual );
            }
        }

        public class GetCountry : AddressServiceExtensionsTests
        {
            string countryKey = Guid.NewGuid().ToString();
            string language = Guid.NewGuid().ToString();

            ICountryMetadata invoke() => service.GetCountry( countryKey, language );

            [Fact]
            public void Requires_service()
            {
                MockService = null;
                Assert.Throws<ArgumentNullException>( nameof( service ), () => invoke() );
            }

            [Theory]
            [MemberData(nameof(AddressServiceTests.GetCountryAsync.GetCountryValues),MemberType =typeof(AddressServiceTests.GetCountryAsync))]
            public void Returns_serviceResult( string countryKey, string language, ICountryMetadata expected )
            {
                MockService.Setup( _=> _.GetCountryAsync( countryKey, language ) ).ReturnsAsync( expected );

                var actual = service.GetCountry( countryKey, language );
                Assert.Equal( expected, actual );
            }
        }

        public class GetProvince : AddressServiceExtensionsTests
        {
            string country = Guid.NewGuid().ToString();
            string province = Guid.NewGuid().ToString();
            string language = Guid.NewGuid().ToString();

            IProvinceMetadata invoke() => service.GetProvince( country, province, language );

            [Fact]
            public void Requires_service()
            {
                MockService = null;
                Assert.Throws<ArgumentNullException>( nameof( service ), () => invoke() );
            }

            public static IEnumerable<object[]> GetArguments => new object[][]
            {
                new object[] { null, null, null, null },
                new object[] { "XW", "XX", "en", new ProvinceMetadata() },
            };

            [Theory]
            [MemberData(nameof(GetArguments))]
            public void Returns_serviceResult( string country, string province, string language, IProvinceMetadata expected )
            {
                this.country = country;
                this.province = province;
                this.language = language;
                MockService.Setup( _=> _.GetProvinceAsync( country, province, language ) ).ReturnsAsync( expected );

                var actual = service.GetProvince( country, province, language );
                Assert.Equal( expected, actual );
            }
        }

        public class GetLocality : AddressServiceExtensionsTests
        {
            string countryKey = Guid.NewGuid().ToString();
            string provinceKey = Guid.NewGuid().ToString();
            string localityKey = Guid.NewGuid().ToString();
            string language = Guid.NewGuid().ToString();

            ILocalityMetadata invoke() => service.GetLocality( countryKey, provinceKey, localityKey, language );

            [Fact]
            public void Requires_service()
            {
                MockService = null;
                Assert.Throws<ArgumentNullException>( nameof( service ), () => invoke() );
            }

            [Fact]
            public void Requires_countryKey()
            {
                countryKey = null;
                Assert.Throws<ArgumentNullException>( nameof( countryKey ), () => invoke() );
            }

            [Fact]
            public void Requires_provinceKey()
            {
                provinceKey = null;
                Assert.Throws<ArgumentNullException>( nameof( provinceKey ), () => invoke() );
            }

            [Fact]
            public void Requires_localityKey()
            {
                localityKey = null;
                Assert.Throws<ArgumentNullException>( nameof( localityKey ), () => invoke() );
            }

            [Theory]
            [MemberData(nameof(AddressServiceTests.GetLocalityAsync.GetArguments),MemberType =typeof(AddressServiceTests.GetLocalityAsync))]
            public void Returns_serviceResult( string countryKey, string provinceKey, string localityKey, string language, string ignored, ILocalityMetadata expected  )
            {
                this.countryKey = countryKey;
                this.provinceKey = provinceKey;
                this.localityKey = localityKey;
                this.language = language;
                MockService.Setup( _=> _.GetLocalityAsync( countryKey, provinceKey, localityKey, language ) ).ReturnsAsync( expected );

                var actual = invoke();
                Assert.Equal( expected, actual );
            }
        }

        public class GetSublocality : AddressServiceExtensionsTests
        {
            string countryKey = Guid.NewGuid().ToString();
            string provinceKey = Guid.NewGuid().ToString();
            string localityKey = Guid.NewGuid().ToString();
            string sublocalityKey = Guid.NewGuid().ToString();
            string language = Guid.NewGuid().ToString();

            ISublocalityMetadata invoke() => service.GetSublocality( countryKey, provinceKey, localityKey, sublocalityKey, language );

            [Fact]
            public void Requires_service()
            {
                MockService = null;
                Assert.Throws<ArgumentNullException>( nameof( service ), () => invoke() );
            }

            [Fact]
            public void Requires_countryKey()
            {
                countryKey = null;
                Assert.Throws<ArgumentNullException>( nameof( countryKey ), () => invoke() );
            }

            [Fact]
            public void Requires_provinceKey()
            {
                provinceKey = null;
                Assert.Throws<ArgumentNullException>( nameof( provinceKey ), () => invoke() );
            }

            [Fact]
            public void Requires_localityKey()
            {
                localityKey = null;
                Assert.Throws<ArgumentNullException>( nameof( localityKey ), () => invoke() );
            }

            [Fact]
            public void Requires_sublocalityKey()
            {
                sublocalityKey = null;
                Assert.Throws<ArgumentNullException>( nameof( sublocalityKey ), () => invoke() );
            }

            [Theory]
            [MemberData(nameof(AddressServiceTests.GetSublocalityAsync.GetArguments),MemberType =typeof(AddressServiceTests.GetSublocalityAsync))]
            public void Returns_serviceResult( string countryKey, string provinceKey, string localityKey, string sublocalityKey, string language, string ignored, ISublocalityMetadata expected  )
            {
                this.countryKey = countryKey;
                this.provinceKey = provinceKey;
                this.localityKey = localityKey;
                this.sublocalityKey = sublocalityKey;
                this.language = language;
                MockService.Setup( _=> _.GetSublocalityAsync( countryKey, provinceKey, localityKey, sublocalityKey, language ) ).ReturnsAsync( expected );

                var actual = invoke();
                Assert.Equal( expected, actual );
            }
        }

        public class Validate : AddressServiceExtensionsTests
        {
            AddressData address;
            string language = null;
            IEnumerable<ValidationFailure> invoke() => service.Validate( address );

            [Fact]
            public void Requires_service()
            {
                MockService = null;
                Assert.Throws<ArgumentNullException>( nameof(service), ()=>invoke() );
            }

            [Fact]
            public void Requires_address()
            {
                address = null;
                Assert.Throws<ArgumentNullException>( nameof(address), ()=>invoke() );
            }

            [Fact]
            public void Returns_serviceResult()
            {
                var expected = new ValidationFailure[2];
                address = new AddressData();

                MockService.Setup( _=> _.ValidateAsync( address, language ) ).ReturnsAsync( expected ).Verifiable();
                var actual = invoke();

                Assert.Same( expected, actual );
                MockService.VerifyAll();
            }
        }
    }
}