// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !NET5_0_OR_GREATER && !NETSTANDARD2_1
namespace System.Runtime.CompilerServices {
    /// <summary>
    /// This interface is required for types that want to be indexed into by dynamic patterns.
    /// </summary>
    /// <remarks>
    /// This is Brandon's backwards-compatible implementation of the ITuple interface taken from the decompiled .net6 source code.
    /// </remarks>
    public interface ITuple {
        /// <summary>
        /// The number of positions in this data structure.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Get the element at position <param name="index"/>.
        /// </summary>
        object? this[int index] { get; }
    }
}
#endif