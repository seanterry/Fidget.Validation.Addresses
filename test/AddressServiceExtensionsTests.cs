using Fidget.Validation.Addresses.Metadata;
using Fidget.Validation.Addresses.Validation;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace Fidget.Validation.Addresses
{
    public class AddressServiceExtensionsTests
    {
        Mock<IAddressService> MockService = new Mock<IAddressService>();
        IAddressService service => MockService?.Object;

        CancellationToken cancellationToken = CancellationToken.None;

        public class GetGlobal : AddressServiceExtensionsTests
        {
            [Fact]
            public void Requires_service()
            {
                MockService = null;
                Assert.Throws<ArgumentNullException>( nameof(service), ()=> service.GetGlobal() );
            }

            [Theory]
            [MemberData( nameof( AddressServiceTests.GetGlobalAsync.ResultCases ), MemberType =typeof(AddressServiceTests.GetGlobalAsync) )]
            public void Returns_serviceResult( GlobalMetadata result )
            {
                MockService.Setup( _=> _.GetGlobalAsync( cancellationToken ) ).ReturnsAsync( result ).Verifiable();
                
                var actual = service.GetGlobal();
                Assert.Equal( result, actual );

                MockService.VerifyAll();
            }
        }

        public class GetCountry : AddressServiceExtensionsTests
        {
            [Fact]
            public void Requires_service()
            {
                MockService = null;
                Assert.Throws<ArgumentNullException>( nameof( service ), () => service.GetCountry( "XW", "en" ) );
            }

            [Theory]
            [MemberData( nameof( AddressServiceTests.GetCountryAsync.ResultCases ), MemberType = typeof( AddressServiceTests.GetCountryAsync ) )]
            public void Returns_serviceResult( string country, string language, CountryMetadata result )
            {
                MockService.Setup( _ => _.GetCountryAsync( country, language, cancellationToken ) ).ReturnsAsync( result ).Verifiable();

                var actual = service.GetCountry( country, language );
                Assert.Equal( result, actual );

                MockService.VerifyAll();
            }
        }

        public class GetProvince : AddressServiceExtensionsTests
        {
            [Fact]
            public void Requires_service()
            {
                MockService = null;
                Assert.Throws<ArgumentNullException>( nameof( service ), () => service.GetProvince( "XW", "XX", "en" ) );
            }

            [Theory]
            [MemberData( nameof( AddressServiceTests.GetProvinceAsync.ResultCases ), MemberType = typeof( AddressServiceTests.GetProvinceAsync ) )]
            public void Returns_serviceResult( string country, string province, string language, ProvinceMetadata result )
            {
                MockService.Setup( _ => _.GetProvinceAsync( country, province, language, cancellationToken ) ).ReturnsAsync( result ).Verifiable();

                var actual = service.GetProvince( country, province, language );
                Assert.Equal( result, actual );

                MockService.VerifyAll();
            }
        }

        public class GetLocality : AddressServiceExtensionsTests
        {
            [Fact]
            public void Requires_service()
            {
                MockService = null;
                Assert.Throws<ArgumentNullException>( nameof( service ), () => service.GetLocality( "XW", "XX", "XY", "en" ) );
            }

            [Theory]
            [MemberData( nameof( AddressServiceTests.GetLocalityAsync.ResultCases ), MemberType = typeof( AddressServiceTests.GetLocalityAsync ) )]
            public void Returns_serviceResult( string country, string province, string locality, string language, LocalityMetadata result )
            {
                MockService.Setup( _ => _.GetLocalityAsync( country, province, locality, language, cancellationToken ) ).ReturnsAsync( result ).Verifiable();

                var actual = service.GetLocality( country, province, locality, language );
                Assert.Equal( result, actual );

                MockService.VerifyAll();
            }
        }

        public class GetSublocality : AddressServiceExtensionsTests
        {
            [Fact]
            public void Requires_service()
            {
                MockService = null;
                Assert.Throws<ArgumentNullException>( nameof( service ), () => service.GetSublocality( "XW", "XX", "XY", "XZ", "en" ) );
            }

            [Theory]
            [MemberData( nameof( AddressServiceTests.GetSublocalityAsync.ResultCases ), MemberType = typeof( AddressServiceTests.GetSublocalityAsync ) )]
            public void Returns_serviceResult( string country, string province, string locality, string sublocality, string language, SublocalityMetadata result )
            {
                MockService.Setup( _ => _.GetSublocalityAsync( country, province, locality, sublocality, language, cancellationToken ) ).ReturnsAsync( result ).Verifiable();

                var actual = service.GetSublocality( country, province, locality, sublocality, language );
                Assert.Equal( result, actual );

                MockService.VerifyAll();
            }
        }

        public class Validate : AddressServiceExtensionsTests
        {
            AddressData address = new AddressData { Country = "US", StreetAddress = "123 Anywhere St." };
            
            [Fact]
            public void Requires_service()
            {
                MockService = null;
                Assert.Throws<ArgumentNullException>( nameof(service), ()=> service.Validate( address ) );
            }

            [Theory]
            [MemberData(nameof(AddressServiceTests.ValidateAsync.ValidateResults),MemberType =typeof(AddressServiceTests.ValidateAsync))]
            public void Returns_serviceResult( AddressValidationFailure[] result )
            {
                MockService.Setup( _=> _.ValidateAsync( address, cancellationToken ) ).ReturnsAsync( result ).Verifiable();

                var actual = service.Validate( address );
                Assert.Equal( result, actual );

                MockService.VerifyAll();
            }
        }
    }
}