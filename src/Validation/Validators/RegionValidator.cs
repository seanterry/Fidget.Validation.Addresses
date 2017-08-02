using Fidget.Validation.Addresses.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fidget.Validation.Addresses.Validation.Validators
{
    /// <summary>
    /// Base functionality for region validators.
    /// </summary>
    
    public abstract class RegionValidator
    {
        /// <summary>
        /// Returns whether the given region is valid.
        /// </summary>
        /// <param name="value">Address value of the child region.</param>
        /// <param name="parent">Parent region metadata.</param>
        /// <param name="child">Child region metadata.</param>
        /// <remarks>
        /// A value is considered valid if it meets the following conditions:
        /// 1. Isn't specified (null or whitespace).
        /// 2. The parent does not denote having any children (meaning the value is allowed to be free-form).
        /// 3. The value is specified and the child metadata key is in the child keys of the parent.
        /// </remarks>
        
        protected static bool IsValid( string value, RegionalMetadata parent, RegionalMetadata child )
        {
            if ( !string.IsNullOrWhiteSpace( value ) )
            if ( parent?.ChildRegionKeys?.Any() == true )
            {
                return parent.ChildRegionKeys.Contains( child?.Key );
            }

            return true;
        }   
    }
}