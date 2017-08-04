using Fidget.Commander;
using Fidget.Validation.Addresses.Metadata;
using Fidget.Validation.Addresses.Metadata.Commands;
using Fidget.Validation.Addresses.Validation;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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

        CancellationToken cancellationToken = CancellationToken.None;

        static string random() => Convert.ToBase64String( Guid.NewGuid().ToByteArray() );

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

        public class GetProvinceAsync : AddressServiceTests
        {
            public static IEnumerable<object[]> ResultCases()
            {
                var countries = new string[] { null, random() };
                var provinces = new string[] { null, random() };
                var languages = new string[] { null, random() };
                var results = new ProvinceMetadata[] { null, new ProvinceMetadata() };

                return
                    from country in countries
                    from province in provinces
                    from language in languages
                    from result in results
                    select new object[] { country, province, language, result };
            }

            [Theory]
            [MemberData( nameof( ResultCases ) )]
            public async Task Returns_commandResult( string country, string province, string language, ProvinceMetadata result )
            {
                var query = new ProvinceMetadataQuery
                {
                    Country = country,
                    Province = province,
                    Language = language,
                };

                MockDispatcher.Setup( _ => _.Execute( query, cancellationToken ) ).ReturnsAsync( result ).Verifiable();

                var actual = await instance.GetProvinceAsync( country, province, language, cancellationToken );
                Assert.Equal( result, actual );
                MockDispatcher.VerifyAll();
            }
        }

        public class GetLocalityAsync : AddressServiceTests
        {
            public static IEnumerable<object[]> ResultCases()
            {
                var countries = new string[] { null, random() };
                var provinces = new string[] { null, random() };
                var localities = new string[] { null, random() };
                var languages = new string[] { null, random() };
                var results = new LocalityMetadata[] { null, new LocalityMetadata() };

                return
                    from country in countries
                    from province in provinces
                    from locality in localities
                    from language in languages
                    from result in results
                    select new object[] { country, province, locality, language, result };
            }

            [Theory]
            [MemberData( nameof( ResultCases ) )]
            public async Task Returns_commandResult( string country, string province, string locality, string language, LocalityMetadata result )
            {
                var query = new LocalityMetadataQuery
                {
                    Country = country,
                    Province = province,
                    Locality = locality,
                    Language = language,
                };

                MockDispatcher.Setup( _ => _.Execute( query, cancellationToken ) ).ReturnsAsync( result ).Verifiable();

                var actual = await instance.GetLocalityAsync( country, province, locality, language, cancellationToken );
                Assert.Equal( result, actual );
                MockDispatcher.VerifyAll();
            }
        }

        public class GetSublocalityAsync : AddressServiceTests
        {
            public static IEnumerable<object[]> ResultCases()
            {
                var countries = new string[] { null, random() };
                var provinces = new string[] { null, random() };
                var localities = new string[] { null, random() };
                var sublocalities = new string[] { null, random() };
                var languages = new string[] { null, random() };
                var results = new SublocalityMetadata[] { null, new SublocalityMetadata() };

                return
                    from country in countries
                    from province in provinces
                    from locality in localities
                    from sublocality in sublocalities
                    from language in languages
                    from result in results
                    select new object[] { country, province, locality, sublocality, language, result };
            }

            [Theory]
            [MemberData( nameof( ResultCases ) )]
            public async Task Returns_commandResult( string country, string province, string locality, string sublocality, string language, SublocalityMetadata result )
            {
                var query = new SublocalityMetadataQuery
                {
                    Country = country,
                    Province = province,
                    Locality = locality,
                    Sublocality = sublocality,
                    Language = language,
                };

                MockDispatcher.Setup( _ => _.Execute( query, cancellationToken ) ).ReturnsAsync( result ).Verifiable();

                var actual = await instance.GetSublocalityAsync( country, province, locality, sublocality, language, cancellationToken );
                Assert.Equal( result, actual );
                MockDispatcher.VerifyAll();
            }
        }

        public class ValidateAsync : AddressServiceTests
        {
            AddressData address;
            Task<IEnumerable<AddressValidationFailure>> invoke() => instance.ValidateAsync( address, cancellationToken );

            [Fact]
            public async Task Requires_address()
            {
                address = null;
                await Assert.ThrowsAsync<ArgumentNullException>( nameof(address), invoke );
            }

            public static IEnumerable<object[]> ValidateResults()
            {
                var errors = Enum.GetValues( typeof(AddressFieldError) ).OfType<AddressFieldError>();
                var fields = Enum.GetValues( typeof(AddressField) ).OfType<AddressField>();
                var all =
                    from error in errors
                    from field in fields
                    select new AddressValidationFailure { Field = field, Error = error };

                yield return new object[] { all.ToArray() };
                yield return new object[] { new AddressValidationFailure[0] };
            }

            [Theory]
            [MemberData(nameof(ValidateResults))]
            public async Task Returns_commandResult( AddressValidationFailure[] result )
            {
                address = new AddressData { Country = "US", StreetAddress = "123 Anywhere St." };
                var command = new Validation.Commands.ValidateAddressCommand
                {
                    Address = address,
                };

                MockDispatcher.Setup( _=> _.Execute( command, cancellationToken ) ).ReturnsAsync( result ).Verifiable();

                var actual = await invoke();
                Assert.Equal( result, actual );
                MockDispatcher.VerifyAll();
            }
        }
    }
}