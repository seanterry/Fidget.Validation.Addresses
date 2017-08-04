using Fidget.Commander;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Metadata.Commands
{
    /// <summary>
    /// Command for querying sublocality metadata.
    /// </summary>

    public struct SublocalityMetadataQuery : ICommand<SublocalityMetadata>
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
        /// Gets or sets the key, local name, or latin name of the containing locality.
        /// </summary>

        public string Locality { get; set; }

        /// <summary>
        /// Gets or sets the key, local name, or latin name of the sublocality to return.
        /// </summary>
        
        public string Sublocality { get; set; }

        /// <summary>
        /// When specified, requests metadata be in the specified language.
        /// Available languages for metadata are defined in the default entry for the country.
        /// </summary>

        public string Language { get; set; }

        /// <summary>
        /// Creates and returns a query to return the sublocality for the given address entry.
        /// </summary>
        /// <param name="address">Address for which to create a query.</param>

        public static SublocalityMetadataQuery For( AddressData address )
        {
            if ( address == null ) throw new ArgumentNullException( nameof( address ) );

            return new SublocalityMetadataQuery
            {
                Country = address.Country,
                Province = address.Province,
                Locality = address.Locality,
                Sublocality = address.Sublocality,
                Language = address.Language,
            };
        }

        /// <summary>
        /// Handler for the command.
        /// </summary>

        public class Handler : ICommandHandler<SublocalityMetadataQuery, SublocalityMetadata>
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

            public async Task<SublocalityMetadata> Handle( SublocalityMetadataQuery command, CancellationToken cancellationToken )
            {
                var parentQuery = new LocalityMetadataQuery
                {
                    Country = command.Country,
                    Province = command.Province,
                    Locality = command.Locality,
                    Language = command.Language,
                };

                if ( await Context.Dispatcher.Execute( parentQuery, cancellationToken ) is LocalityMetadata parent )
                if ( Context.Builder.GetChildKey( parent, command.Sublocality ) is string key )
                if ( Context.Builder.BuildIdentifier( parent, key, command.Language ) is string id )
                {
                    return await Context.Client.Query<SublocalityMetadata>( id );
                }

                return null;
            }
        }
    }
}