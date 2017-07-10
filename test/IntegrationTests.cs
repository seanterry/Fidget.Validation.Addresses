using Fidget.Validation.Addresses.Service.Metadata.Internal;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses
{
    public class IntegrationTests
    {
        static readonly IAddressService instance = new AddressService();

        public class Exploration : IntegrationTests
        {
            // known entry that goes down to a sublocality level
            string countryKey = "KR";
            string provinceKey = "충청북도";
            string localityKey = "청주시";
            string sublocalityKey = "상당구";
        
            [Fact]
            public async Task GetCountry()
            {
                var actual = await instance.GetCountryAsync( countryKey );
                Assert.IsType<CountryMetadata>( actual );
            }

            [Fact]
            public async Task GetProvince()
            {
                var actual = await instance.GetProvinceAsync( countryKey, provinceKey );
                Assert.IsType<ProvinceMetadata>( actual );
            }

            [Fact]
            public async Task GetLocality()
            {
                var actual = await instance.GetLocalityAsync( countryKey, provinceKey, localityKey );
                Assert.IsType<LocalityMetadata>( actual );
            }

            [Fact]
            public async Task GetSublocality()
            {
                var actual = await instance.GetSublocalityAsync( countryKey, provinceKey, localityKey, sublocalityKey );
                Assert.IsType<SublocalityMetadata>( actual );
            }
        }
    }
}