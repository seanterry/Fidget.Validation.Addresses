using Fidget.Commander;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Metadata.Commands
{
    /// <summary>
    /// Command for querying locality metadata.
    /// </summary>

    public struct LocalityMetadataQuery : ICommand<LocalityMetadata>
    {
        /// <summary>
        /// Gets or sets the key of the containing country.
        /// Due to names not being present on global metadata, only the country key may be searched.
        /// </summary>

        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the key, local name, or latin name of the containing province.
        /// </summary>

        public string Province { get; set; }

        /// <summary>
        /// Gets or sets the key, local name, or latin name of the locality to return.
        /// </summary>
        
        public string Locality { get; set; }

        /// <summary>
        /// When specified, requests metadata be in the specified language.
        /// Available languages for metadata are defined in the default entry for the country.
        /// </summary>

        public string Language { get; set; }

        /// <summary>
        /// Creates and returns a query to return the locality for the given address entry.
        /// </summary>
        /// <param name="address">Address for which to create a query.</param>

        public static LocalityMetadataQuery For( AddressData address )
        {
            if ( address == null ) throw new ArgumentNullException( nameof( address ) );

            return new LocalityMetadataQuery
            {
                Country = address.Country,
                Province = address.Province,
                Locality = address.Locality,
                Language = address.Language,
            };
        }

        /// <summary>
        /// Handler for the command.
        /// </summary>

        public class Handler : ICommandHandler<LocalityMetadataQuery,LocalityMetadata>
        {
            /// <summary>
            /// Query context.
            /// </summary>

            readonly IMetadataQueryContext Context;

            /// <summary>
            /// Constructs a handler for the command.
            /// </summary>
            /// <param name="context">Query context.</param>

            public Handler( IMetadataQueryContext context )
            {
                Context = context ?? throw new ArgumentNullException( nameof( context ) );
            }

            /// <summary>
            /// Executes the command.
            /// </summary>

            public async Task<LocalityMetadata> Handle( LocalityMetadataQuery command, CancellationToken cancellationToken )
            {
                var parentQuery = new ProvinceMetadataQuery
                {
                    Country = command.Country,
                    Province = command.Province,
                    Language = command.Language,
                };

                if ( await Context.Dispatcher.Execute( parentQuery, cancellationToken ) is ProvinceMetadata parent )
                if ( Context.Builder.GetChildKey( parent, command.Locality ) is string key )
                if ( Context.Builder.BuildIdentifier( parent, key, command.Language ) is string id )
                {
                    return await Context.Client.Query<LocalityMetadata>( id );
                }

                return null;
            }
        }
    }
}