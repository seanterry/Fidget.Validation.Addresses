using Fidget.Commander.Dispatch;
using Fidget.Validation.Addresses.Client;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Metadata.Commands.Decorators
{
    public class DefaultCountryValuesDecoratorTests
    {
        Mock<IServiceClient> MockClient = new Mock<IServiceClient>();

        IServiceClient client => MockClient?.Object;
        ICommandDecorator<CountryMetadataQuery,CountryMetadata> instance => new DefaultCountryValuesDecorator( client );

        public class Constructor : DefaultCountryValuesDecoratorTests
        {
            [Fact]
            public void Requires_client()
            {
                MockClient = null;
                Assert.Throws<ArgumentNullException>( nameof(client), ()=>instance );
            }
        }

        public class Execute : DefaultCountryValuesDecoratorTests
        {
            Mock<CommandDelegate<CountryMetadataQuery, CountryMetadata>> MockContinuation = new Mock<CommandDelegate<CountryMetadataQuery, CountryMetadata>>();

            CountryMetadataQuery command = new CountryMetadataQuery { Country = "XW", Language = "en" };
            CancellationToken cancellationToken = CancellationToken.None;
            CommandDelegate<CountryMetadataQuery,CountryMetadata> continuation => MockContinuation?.Object;
            
            static CountryMetadata defaults => new CountryMetadata
            {
                Format = "A%nC%nS, Z",
                Required = new AddressField[] { AddressField.StreetAddress, AddressField.Locality, AddressField.Province, AddressField.PostalCode },
                Uppercase = new AddressField[] { AddressField.Locality },
                StateType = "province",
                LocalityType = "locality",
                SublocalityType = "neighborhood",
                PostalCodeType = "zip",
            };

            /// <summary>
            /// Cases where no override is performed.
            /// </summary>
            
            public static IEnumerable<object[]> NoOverrideCases()
            {
                // no country returned from continuation
                yield return new object[] { null, defaults };

                // no default returns from client
                yield return new object[] { new CountryMetadata(), null };
            }

            [Theory]
            [MemberData(nameof(NoOverrideCases))]
            public async Task WhenNotOverridable_returns_continuation( CountryMetadata country, CountryMetadata defaults )
            {
                // set up expected values
                var expected = new CountryMetadata
                {
                    Format = country?.Format,
                    Required = country?.Required,
                    Uppercase = country?.Uppercase,
                    StateType = country?.StateType,
                    LocalityType = country?.LocalityType,
                    SublocalityType = country?.SublocalityType,
                    PostalCodeType = country?.PostalCodeType,
                };

                MockContinuation.Setup( _=> _( command, cancellationToken ) ).ReturnsAsync( country ).Verifiable();
                MockClient.Setup( _=> _.Query<CountryMetadata>( "data/ZZ" ) ).ReturnsAsync( defaults );

                var actual = await instance.Execute( command, cancellationToken, continuation );
                Assert.Equal( country, actual );

                if ( country != null )
                {
                    Assert.Equal( expected.Format, country.Format );
                    Assert.Equal( expected.Required, country.Required );
                    Assert.Equal( expected.Uppercase, country.Uppercase );
                    Assert.Equal( expected.StateType, country.StateType );
                    Assert.Equal( expected.LocalityType, country.LocalityType );
                    Assert.Equal( expected.SublocalityType, country.SublocalityType );
                    Assert.Equal( expected.PostalCodeType, country.PostalCodeType );
                }
                
                var defaultsTimes = country != null ? Times.Once() : Times.Never();
                MockContinuation.VerifyAll();
                MockClient.Verify( _=> _.Query<CountryMetadata>( "data/ZZ" ), defaultsTimes );
            }

            /// <summary>
            /// Cases in which some or all of the properties on the continuation country are defaultable.
            /// </summary>
            
            public static IEnumerable<object[]> OverridableCases()
            {
                string random() => Convert.ToBase64String( Guid.NewGuid().ToByteArray() );

                var countries = new CountryMetadata[]
                {
                    new CountryMetadata { },
                    new CountryMetadata { Format = random() },
                    new CountryMetadata { Required = new AddressField[] { AddressField.SortingCode } },
                    new CountryMetadata { Uppercase = new AddressField[] { AddressField.SortingCode } },
                    new CountryMetadata { StateType = random() },
                    new CountryMetadata { LocalityType = random() },
                    new CountryMetadata { SublocalityType = random() },
                    new CountryMetadata { PostalCodeType = random() },
                };

                return countries.Select( _=> new object[] { _ } );
            }
            
            [Theory]
            [MemberData(nameof(OverridableCases))]
            public async Task WhenOverridable_defaultsNullValues( CountryMetadata country )
            {
                // set up expected values
                var expected = new CountryMetadata
                {
                    Format = country.Format ?? defaults.Format,
                    Required = country.Required ?? defaults.Required,
                    Uppercase = country.Uppercase ?? defaults.Uppercase,
                    StateType = country.StateType ?? defaults.StateType,
                    LocalityType = country.LocalityType ?? defaults.LocalityType,
                    SublocalityType = country.SublocalityType ?? defaults.SublocalityType,
                    PostalCodeType = country.PostalCodeType ?? defaults.PostalCodeType,
                };

                MockContinuation.Setup( _ => _( command, cancellationToken ) ).ReturnsAsync( country ).Verifiable();
                MockClient.Setup( _ => _.Query<CountryMetadata>( "data/ZZ" ) ).ReturnsAsync( defaults ).Verifiable();

                var actual = await instance.Execute( command, cancellationToken, continuation );
                Assert.Equal( country, actual );

                // ensure we have the expected values
                Assert.Equal( expected.Format, country.Format );
                Assert.Equal( expected.Required, country.Required );
                Assert.Equal( expected.Uppercase, country.Uppercase );
                Assert.Equal( expected.StateType, country.StateType );
                Assert.Equal( expected.LocalityType, country.LocalityType );
                Assert.Equal( expected.SublocalityType, country.SublocalityType );
                Assert.Equal( expected.PostalCodeType, country.PostalCodeType );
                
                MockContinuation.VerifyAll();
                MockClient.VerifyAll();
            }
        }
    }
}