using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Tests of the metadata aggregator.
    /// </summary>

    public class MetadataAggregatorTests
    {
        IMetadataAggregator create() => new MetadataAggregator();

        public class GetRequiredFields : MetadataAggregatorTests
        {
            // exclude country since it is a special case
            static readonly AddressField[] all = Enum.GetValues( typeof( AddressField ) )
                .OfType<AddressField>()
                .Except( new AddressField[] { AddressField.Country } )
                .ToArray();

            static readonly AddressField[] empty = new AddressField[0];

            static ICountryMetadata country( AddressField[] required = null ) => new CountryMetadata { Required = required };
            static IProvinceMetadata province( AddressField[] required = null ) => new ProvinceMetadata { Required = required };
            static ILocalityMetadata locality( AddressField[] required = null ) => new LocalityMetadata { Required = required };
            static ISublocalityMetadata sublocality( AddressField[] required = null ) => new SublocalityMetadata { Required = required };

            public static IEnumerable<object[]> Always_Returns_Country_Data()
            {
                yield return new object[] { null, null, null, null };
                yield return new object[] { country(), null, null, null };
                yield return new object[] { null, province(), null, null };
                yield return new object[] { null, null, locality(), null };
                yield return new object[] { null, null, null, sublocality() };
                yield return new object[] { country(), province(), locality(), sublocality() };
                yield return new object[] { country( empty ), province( empty ), locality( empty ), sublocality( empty ) };
                yield return new object[] { country( all ), province( all ), locality( all ), sublocality( all ) };
            }

            [Theory]
            [MemberData(nameof(Always_Returns_Country_Data))]
            public void AlwaysReturns_Country( IRegionalMetadata region1, IRegionalMetadata region2, IRegionalMetadata region3, IRegionalMetadata region4 )
            {
                var actual = create().GetRequiredFields( region1, region2, region3, region4 );
                Assert.Contains( AddressField.Country, actual );
            }

            public class RequiredFieldMatrixData : TheoryData<AddressField,IRegionalMetadata,IRegionalMetadata,IRegionalMetadata,IRegionalMetadata>
            {
                public RequiredFieldMatrixData()
                {
                    foreach ( var field in all )
                    {
                        var self = new AddressField[] { field };

                        // check conditions where metadata may be null
                        Add( field, country( self ), null, null, null );
                        Add( field, null, province( self ), null, null );
                        Add( field, null, null, locality( self ), null );
                        Add( field, null, null, null, sublocality( self ) );
                        Add( field, country( all ), null, null, null );
                        Add( field, null, province( all ), null, null );
                        Add( field, null, null, locality( all ), null );
                        Add( field, null, null, null, sublocality( all ) );

                        // check conditions where the metadata is defined but the collection is null
                        Add( field, country( self ), province(), locality(), sublocality() );
                        Add( field, country(), province( self ), locality(), sublocality() );
                        Add( field, country(), province(), locality( self ), sublocality() );
                        Add( field, country(), province(), locality(), sublocality( self ) );
                        Add( field, country( all ), province(), locality(), sublocality() );
                        Add( field, country(), province( all ), locality(), sublocality() );
                        Add( field, country(), province(), locality( all ), sublocality() );
                        Add( field, country(), province(), locality(), sublocality( all ) );

                        // check conditions where the required collections may be empty
                        Add( field, country( self ), province( empty ), locality( empty ), sublocality( empty ) );
                        Add( field, country( empty ), province( self ), locality( empty ), sublocality( empty ) );
                        Add( field, country( empty ), province( empty ), locality( self ), sublocality( empty ) );
                        Add( field, country( empty ), province( empty ), locality( empty ), sublocality( self ) );
                        Add( field, country( all ), province( empty ), locality( empty ), sublocality( empty ) );
                        Add( field, country( empty ), province( all ), locality( empty ), sublocality( empty ) );
                        Add( field, country( empty ), province( empty ), locality( all ), sublocality( empty ) );
                        Add( field, country( empty ), province( empty ), locality( empty ), sublocality( all ) );
                    }
                }
            }

            public static RequiredFieldMatrixData WhenFieldRequired_ReturnsFieldInCollection_Data = new RequiredFieldMatrixData();

            [Theory]
            [MemberData(nameof( WhenFieldRequired_ReturnsFieldInCollection_Data ) )]
            public void WhenFieldRequired_ReturnsFieldInCollection( AddressField field, IRegionalMetadata region1, IRegionalMetadata region2, IRegionalMetadata region3, IRegionalMetadata region4 )
            {
                var actual = create().GetRequiredFields( region1, region2, region3, region4 );
                Assert.Contains( field, actual );
            }

            public class NonRequiredFieldMatrixData : TheoryData<AddressField, IRegionalMetadata, IRegionalMetadata, IRegionalMetadata, IRegionalMetadata>
            {
                public NonRequiredFieldMatrixData()
                {
                    foreach ( var field in all )
                    {
                        var self = new AddressField[] { field };
                        var others = all.Except( self ).ToArray();

                        // check conditions where metadata may be null
                        Add( field, null, null, null, null );
                        Add( field, country( others ), null, null, null );
                        Add( field, null, province( others ), null, null );
                        Add( field, null, null, locality( others ), null );
                        Add( field, null, null, null, sublocality( others ) );

                        // check conditions where the metadata is defined but the collection is null
                        Add( field, country(), province(), locality(), sublocality() );
                        Add( field, country( others ), province(), locality(), sublocality() );
                        Add( field, country(), province( others ), locality(), sublocality() );
                        Add( field, country(), province(), locality( others ), sublocality() );
                        Add( field, country(), province(), locality(), sublocality( others ) );

                        // check conditions where the required collections may be empty
                        Add( field, country( empty ), province( empty ), locality( empty ), sublocality( empty ) );
                        Add( field, country( others ), province( empty ), locality( empty ), sublocality( empty ) );
                        Add( field, country( empty ), province( others ), locality( empty ), sublocality( empty ) );
                        Add( field, country( empty ), province( empty ), locality( others ), sublocality( empty ) );
                        Add( field, country( empty ), province( empty ), locality( empty ), sublocality( others ) );
                    }
                }
            }

            public static NonRequiredFieldMatrixData WhenFieldNotRequired_FieldNotInCollection_Data = new NonRequiredFieldMatrixData();

            [Theory]
            [MemberData( nameof( WhenFieldNotRequired_FieldNotInCollection_Data ) )]
            public void WhenFieldNotRequired_FieldNotInCollection( AddressField field, IRegionalMetadata region1, IRegionalMetadata region2, IRegionalMetadata region3, IRegionalMetadata region4 )
            {
                var actual = create().GetRequiredFields( region1, region2, region3, region4 );
                Assert.DoesNotContain( field, actual );
            }
        }
    }
}