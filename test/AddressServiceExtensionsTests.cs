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
            [InlineData( "XX", null )]
            [InlineData( "XX", "xxx" )]
            public void Returns_serviceResult( string countryKey, string language )
            {
                ICountryMetadata expected = new CountryMetadata();
                MockService.Setup( _=> _.GetCountryAsync( countryKey, language ) ).ReturnsAsync( expected );

                var actual = service.GetCountry( countryKey, language );
                Assert.Same( expected, actual );
            }
        }
    }
}