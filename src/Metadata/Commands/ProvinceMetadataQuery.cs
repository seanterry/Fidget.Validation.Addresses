using Fidget.Commander;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Metadata.Commands
{
    /// <summary>
    /// Command for querying province metadata.
    /// </summary>

    public struct ProvinceMetadataQuery : ICommand<ProvinceMetadata>
    {
        /// <summary>
        /// Gets or sets the key of the containing country.
        /// Due to names not being present on global metadata, only the country key may be searched.
        /// </summary>

        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the key, local name, or latin name of the province to return.
        /// </summary>
        
        public string Province { get; set; }

        /// <summary>
        /// When specified, requests metadata be in the specified language.
        /// Available languages for metadata are defined in the default entry for the country.
        /// </summary>

        public string Language { get; set; }

        /// <summary>
        /// Handler for the command.
        /// </summary>

        public class Handler : ICommandHandler<ProvinceMetadataQuery, ProvinceMetadata>
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

            public async Task<ProvinceMetadata> Handle( ProvinceMetadataQuery command, CancellationToken cancellationToken )
            {
                var parentQuery = new CountryMetadataQuery
                {
                    Country = command.Country,
                    Language = command.Language,
                };

                if ( await Context.Dispatcher.Execute( parentQuery, cancellationToken ) is CountryMetadata country )
                if ( Context.Builder.GetChildKey( country, command.Province ) is string key )
                if ( Context.Builder.BuildIdentifier( country, key, command.Language ) is string id )
                {
                    return await Context.Client.Query<ProvinceMetadata>( id );
                }

                return null;
            }
        }
    }
}