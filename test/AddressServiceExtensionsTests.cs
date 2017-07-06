using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
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

            [Fact]
            public void Requires_countryKey()
            {
                countryKey = null;
                Assert.Throws<ArgumentNullException>( nameof(countryKey), ()=>invoke() );
            }

            [Theory]
            [MemberData(nameof(AddressServiceTests.GetCountryAsync.GetCountryValues),MemberType =typeof(AddressServiceTests.GetCountryAsync))]
            public void Returns_serviceResult( string countryKey, string language, string ignored, ICountryMetadata expected )
            {
                MockService.Setup( _=> _.GetCountryAsync( countryKey, language ) ).ReturnsAsync( expected );

                var actual = service.GetCountry( countryKey, language );
                Assert.Equal( expected, actual );
            }
        }

        public class GetProvince : AddressServiceExtensionsTests
        {
            string countryKey = Guid.NewGuid().ToString();
            string provinceKey = Guid.NewGuid().ToString();
            string language = Guid.NewGuid().ToString();

            IProvinceMetadata invoke() => service.GetProvince( countryKey, provinceKey, language );

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

            [Theory]
            [MemberData(nameof(AddressServiceTests.GetProvinceAsync.GetArguments),MemberType =typeof(AddressServiceTests.GetProvinceAsync))]
            public void Returns_serviceResult( string countryKey, string provinceKey, string language, string ignored, IProvinceMetadata expected  )
            {
                this.countryKey = countryKey;
                this.provinceKey = provinceKey;
                this.language = language;
                MockService.Setup( _=> _.GetProvinceAsync( countryKey, provinceKey, language ) ).ReturnsAsync( expected );

                var actual = invoke();
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
    }
}