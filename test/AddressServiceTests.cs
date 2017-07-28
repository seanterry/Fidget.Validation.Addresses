using Fidget.Commander;
using Fidget.Validation.Addresses.Metadata;
using Fidget.Validation.Addresses.Metadata.Commands;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses
{
    public class AddressServiceTests
    {
        Mock<ICommandDispatcher> MockDispatcher = new Mock<ICommandDispatcher>();

        ICommandDispatcher dispatcher => MockDispatcher?.Object;
        IAddressService instance => new AddressService( dispatcher );

        public class Constructor : AddressServiceTests
        {
            [Fact]
            public void Requires_dispatcher()
            {
                MockDispatcher = null;
                Assert.Throws<ArgumentNullException>( nameof(dispatcher), ()=>instance );
            }

            [Fact]
            public void IsRegistered()
            {
                var actual = DependencyInjection.Container.GetInstance<IAddressService>();
                Assert.IsType<AddressService>( actual );
            }
        }

        public class GetGlobalAsync : AddressServiceTests
        {
            CancellationToken cancellationToken = CancellationToken.None;

            public static IEnumerable<object[]> ResultCases = new object[][]
            {
                new object[] { null },
                new object[] { new GlobalMetadata() },
            };

            [Theory]
            [MemberData(nameof(ResultCases))]
            public async Task Returns_commandResult( GlobalMetadata result )
            {
                MockDispatcher.Setup( _=> _.Execute( GlobalMetadataQuery.Default, cancellationToken ) ).ReturnsAsync( result );

                var actual = await instance.GetGlobalAsync( cancellationToken );
                Assert.Equal( result, actual );
            }
        }

        public class GetCountryAsync : AddressServiceTests
        {
            CancellationToken cancellationToken = CancellationToken.None;

            public static IEnumerable<object[]> ResultCases()
            {
                var countries = new string[] { null, string.Empty, "XX" };
                var languages = new string[] { null, "en" };
                var results = new CountryMetadata[] { null, new CountryMetadata() };

                return
                    from country in countries
                    from language in languages
                    from result in results
                    select new object[] { country, language, result };
            }

            [Theory]
            [MemberData(nameof(ResultCases))]
            public async Task Returns_commandResult( string country, string language, CountryMetadata result )
            {
                var query = new CountryMetadataQuery
                {
                    Country = country,
                    Language = language,
                };

                MockDispatcher.Setup( _=> _.Execute( query, cancellationToken ) ).ReturnsAsync( result ).Verifiable();

                var actual = await instance.GetCountryAsync( country, language, cancellationToken );
                Assert.Equal( result, actual );
                MockDispatcher.VerifyAll();
            }
        }
    }
}
