using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Fidget.Validation.Addresses.Validation
{
    public class RequiredElementsValidatorTests
    {
        Mock<IMetadataAggregator> MockAggregator = new Mock<IMetadataAggregator>();
        IMetadataAggregator aggregator => MockAggregator?.Object;

        IAddressValidator create() => new RequiredElementsValidator( aggregator );

        public class Constructor : RequiredElementsValidatorTests
        {
            [Fact]
            public void Requires_aggregator()
            {
                MockAggregator = null;
                Assert.Throws<ArgumentNullException>( nameof(aggregator), ()=>create() );
            }
        }

        public class Validate : RequiredElementsValidatorTests
        {
            AddressData address;
            IGlobalMetadata global = new GlobalMetadata();
            ICountryMetadata country = new CountryMetadata();
            IProvinceMetadata province = new ProvinceMetadata();
            ILocalityMetadata locality = new LocalityMetadata();
            ISublocalityMetadata sublocality = new SublocalityMetadata();
            IEnumerable<ValidationFailure> invoke() => create().Validate( address, global, country, province, locality, sublocality );

            static readonly IEnumerable<AddressField> allFields = Enum.GetValues( typeof(AddressField) ).OfType<AddressField>().ToArray();

            public class RequiredElementsData : TheoryData<AddressField,AddressData>
            {
                public RequiredElementsData( params string[] values )
                {
                    foreach ( var value in values )
                    {
                        Add( AddressField.Country, new AddressData { Country = value } );
                        Add( AddressField.Locality, new AddressData { Locality = value } );
                        Add( AddressField.Name, new AddressData { Name = value } );
                        Add( AddressField.Organization, new AddressData { Organization = value } );
                        Add( AddressField.PostalCode, new AddressData { PostalCode = value } );
                        Add( AddressField.Province, new AddressData { Province = value } );
                        Add( AddressField.SortingCode, new AddressData { SortingCode = value } );
                        Add( AddressField.StreetAddress, new AddressData { StreetAddress = value } );
                        Add( AddressField.Sublocality, new AddressData { Sublocality = value } );
                    }
                }
            }
            
            /// <summary>
            /// When a required element has a null or whitespace value, it should fail validation.
            /// </summary>
            
            [Theory]
            [MemberData(nameof(RequiredElementsNullOrWhitespace_Data))]
            public void WhenRequiredElement_NullOrWhitespace_Failure( AddressField field, AddressData address )
            {
                this.address = address;
                MockAggregator.Setup( _=> _.GetRequiredFields( country, province, locality, sublocality ) ).Returns( new AddressField[] { field } ).Verifiable();

                var expected = RequiredElementsValidator.Failures[field];
                var actual = invoke();
                Assert.Contains( expected, actual );

                MockAggregator.VerifyAll();
            }
            
            public static RequiredElementsData RequiredElementsNullOrWhitespace_Data = new RequiredElementsData( null, "", "\t" );

            /// <summary>
            /// When a required element has a non-whitespace value, it should pass validation. 
            /// </summary>
            
            [Theory]
            [MemberData( nameof(RequiredElementsValue) )]
            public void WhenRequiredElement_HasValue_Pass( AddressField field, AddressData address )
            {
                this.address = address;
                MockAggregator.Setup( _ => _.GetRequiredFields( country, province, locality, sublocality ) ).Returns( new AddressField[] { field } ).Verifiable();

                var expected = RequiredElementsValidator.Failures[field];
                var actual = invoke();
                Assert.DoesNotContain( expected, actual );

                MockAggregator.VerifyAll();
            }

            public static RequiredElementsData RequiredElementsValue = new RequiredElementsData( Guid.NewGuid().ToString() );

            /// <summary>
            /// When a non-required field contains any value, it should pass validation.
            /// </summary>
            
            [Theory]
            [MemberData(nameof(NonRequiredElements))]
            public void WhenNonRequiredElement_AnyValue_Pass( AddressField field, AddressData address )
            {
                this.address = address;
                var others = allFields.Except( new AddressField[] { field } );
                MockAggregator.Setup( _ => _.GetRequiredFields( country, province, locality, sublocality ) ).Returns( others ).Verifiable();

                var expected = RequiredElementsValidator.Failures[field];
                var actual = invoke();
                Assert.DoesNotContain( expected, actual );

                MockAggregator.VerifyAll();
            }

            public static RequiredElementsData NonRequiredElements = new RequiredElementsData( null, "", "\t", Guid.NewGuid().ToString() );
        }
    }
}
