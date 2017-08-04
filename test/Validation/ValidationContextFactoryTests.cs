using Fidget.Commander;
using Fidget.Validation.Addresses.Metadata;
using Fidget.Validation.Addresses.Metadata.Commands;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Validation
{
    public class ValidationContextFactoryTests
    {
        Mock<ICommandDispatcher> MockDispatcher = new Mock<ICommandDispatcher>();

        ICommandDispatcher dispatcher => MockDispatcher?.Object;
        IValidationContextFactory instance => new ValidationContextFactory( dispatcher );

        public class Constructor : ValidationContextFactoryTests
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
                var actual = DependencyInjection.Container.GetInstance<IValidationContextFactory>();
                Assert.IsType<ValidationContextFactory>( actual );
            }
        }

        public class Create : ValidationContextFactoryTests
        {
            AddressData address;
            CancellationToken cancellationToken = CancellationToken.None;

            Task<ValidationContext> invoke() => instance.Create( address, cancellationToken );

            [Fact]
            public async Task Requires_address()
            {
                address = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof(address), invoke );
            }

            static string random() => Convert.ToBase64String( Guid.NewGuid().ToByteArray() );

            public static IEnumerable<object[]> CreateCases()
            {
                var countries = new string[] { null, random() };
                var provinces = new string[] { null, random() };
                var localities = new string[] { null, random() };
                var sublocalities = new string[] { null, random() };
                var languages = new string[] { null, random() };

                return
                    from country in countries
                    from province in provinces
                    from locality in localities
                    from sublocality in sublocalities
                    from language in languages
                    select new object[] { new AddressData
                    {
                        Country = country,
                        Province = province,
                        Locality = locality,
                        Sublocality = sublocality,
                        Language = language
                    }};
            }

            [Theory]
            [MemberData(nameof(CreateCases))]
            public async Task Returns_context( AddressData address )
            {
                this.address = address;

                // expected queries
                var countryQuery = CountryMetadataQuery.For( address );
                var provinceQuery = ProvinceMetadataQuery.For( address );
                var localityQuery = LocalityMetadataQuery.For( address );
                var sublocalityQuery = SublocalityMetadataQuery.For( address );

                var country = new CountryMetadata();
                var province = new ProvinceMetadata();
                var locality = new LocalityMetadata();
                var sublocality = new SublocalityMetadata();

                MockDispatcher.Setup( _=> _.Execute( countryQuery, cancellationToken ) ).ReturnsAsync( country ).Verifiable();
                MockDispatcher.Setup( _=> _.Execute( provinceQuery, cancellationToken ) ).ReturnsAsync( province ).Verifiable();
                MockDispatcher.Setup( _=> _.Execute( localityQuery, cancellationToken ) ).ReturnsAsync( locality ).Verifiable();
                MockDispatcher.Setup( _=> _.Execute( sublocalityQuery, cancellationToken ) ).ReturnsAsync( sublocality ).Verifiable();

                var expected = new ValidationContext
                {
                    Address = address,
                    CountryMetadata = country,
                    ProvinceMetadata = province,
                    LocalityMetadata = locality,
                    SublocalityMetadata = sublocality,
                };

                var actual = await invoke();

                Assert.Equal( expected, actual );
            }
        }
    }
}
