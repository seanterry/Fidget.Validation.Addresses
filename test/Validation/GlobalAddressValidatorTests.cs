using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using System;
using System.Collections.Generic;
using Xunit;

namespace Fidget.Validation.Addresses.Validation
{
    public class GlobalAddressValidatorTests
    {
        IAddressValidator create() => new GlobalAddressValidator();

        public class Validate : GlobalAddressValidatorTests
        {
            AddressData address = new AddressData();
            IGlobalMetadata global = new GlobalMetadata { Id = "data", Countries = new string[] { "XW", "XA" } };
            ICountryMetadata country = new CountryMetadata { Id = "data/XW", Key = "XW" };

            IEnumerable<ValidationFailure> invoke() => create().Validate( address, global, country, null, null, null );

            [Fact]
            public void Requires_address()
            {
                address = null;
                Assert.Throws<ArgumentNullException>( nameof(address), ()=>invoke() );
            }

            [Fact]
            public void Requires_global()
            {
                global = null;
                Assert.Throws<ArgumentNullException>( nameof(global), ()=>invoke() );
            }

            [Theory]
            [InlineData( null )]
            [InlineData( "" )]
            [InlineData( "\t" )]
            public void WhenCountryMissing_returns_MissingRequiredField( string value )
            {
                address.Country = value;
                var expected = GlobalAddressValidator.MissingCountry;
                var actual = invoke();

                Assert.Contains( expected, actual );
            }

            [Fact]
            public void WhenCountryUnknown_returns_UnknownValue()
            {
                address.Country = "XX";
                var expected = GlobalAddressValidator.UnknownCountry;
                var actual = invoke();

                Assert.Contains( expected, actual );
            }

            [Fact]
            public void WhenCountryKnown_requires_country()
            {
                address.Country = "XA";
                country = null;
                Assert.Throws<ArgumentNullException>( nameof(country), ()=>invoke() );
            }

            [Fact]
            public void WhenValid_returns_empty()
            {
                address.Country = country.Key;
                var actual = invoke();

                Assert.Empty( actual );
            }
        }
    }
}