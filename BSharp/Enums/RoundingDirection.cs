using System;

namespace FowlFever.BSharp.Enums {
    /// <summary>
    /// Decides whether to round using <see cref="Math.Ceiling(decimal)"/> or <see cref="Math.Floor(decimal)"/>.
    /// </summary>
    /// <remarks>
    /// Sort of a complement to <see cref="MidpointRounding"/>?
    /// </remarks>
    public enum RoundingDirection {
        /// <summary>
        /// AKA <see cref="Math.Ceiling(decimal)"/>
        /// </summary>
        Ceiling,
        /// <summary>
        /// AKA <see cref="Math.Floor(decimal)"/>
        /// </summary>
        Floor
    }
}