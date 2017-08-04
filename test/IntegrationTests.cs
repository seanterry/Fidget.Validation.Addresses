using Fidget.Commander;
using Fidget.Validation.Addresses.Metadata.Commands;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses
{
    public class IntegrationTests
    {
        // known entry that goes down to a sublocality level
        const string countryKey = "KR";
        const string provinceKey = "충청북도";
        const string localityKey = "청주시";
        const string sublocalityKey = "상당구";
        const string language = "ko";

        [Fact]
        public async Task GetSublocality()
        {
            var query = new SublocalityMetadataQuery
            {
                Country = countryKey,
                Province = provinceKey,
                Locality = localityKey,
                Sublocality = sublocalityKey,
                Language = language,
            };
            
            var actual = await DependencyInjection.Container.GetInstance<ICommandDispatcher>().Execute( query );
            Assert.NotNull( actual );
        }

        [Fact]
        public async Task GetProvince()
        {
            var query = new ProvinceMetadataQuery
            {
                Country = "CA",
                Province = "ON",
                Language = "fr",
            };

            var actual = await DependencyInjection.Container.GetInstance<ICommandDispatcher>().Execute( query );
            Assert.NotNull( actual );
        }

        [Fact]
        public async Task Validate()
        {
            var address = new AddressData
            {
                StreetAddress = "123 Anywhere St.",
                Locality = "Little Rock",
                Province = "AR",
                PostalCode = "72200",
                Country = "US",
            };

            var actual = await DependencyInjection.Container.GetInstance<IAddressService>().ValidateAsync( address );
            Assert.Empty( actual );
        }
    }
}