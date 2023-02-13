using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Memory;

internal static partial class PrimitiveMath {
    #region NumberType

    /// <summary>
    /// The <see cref="Type"/>s that are valid for use with <see cref="Vector{T}"/>s.
    /// </summary>
    public enum NumberType : byte {
        Byte,
        SByte,
        Short,
        UShort,
        Int,
        UInt,
        Long,
        ULong,
        Float,
        Double,
        Decimal,
        /// <summary>
        /// A "native integer", which is a given computer's favorite integral type to work with.
        ///
        /// For example, some computers were built to work best with <see cref="int"/>s, while others work best with <see cref="long"/>s.
        /// <see cref="IntPtr"/> would correspond to <see cref="int"/> or <see cref="long"/> when run on those computers, respectively. 
        /// </summary>
        NInt,
        /// <summary>
        /// Same as <see cref="NInt"/>, but unsigned.
        /// </summary>
        NUInt,
    }

    public static Type RealType(this NumberType numberType) {
        return numberType switch {
            NumberType.Byte    => typeof(byte),
            NumberType.SByte   => typeof(sbyte),
            NumberType.Short   => typeof(short),
            NumberType.UShort  => typeof(ushort),
            NumberType.Int     => typeof(int),
            NumberType.UInt    => typeof(uint),
            NumberType.Long    => typeof(long),
            NumberType.ULong   => typeof(ulong),
            NumberType.Float   => typeof(float),
            NumberType.Double  => typeof(double),
            NumberType.Decimal => typeof(decimal),
            NumberType.NInt    => typeof(nint),
            NumberType.NUInt   => typeof(nuint),
            _                  => throw new ArgumentOutOfRangeException(nameof(numberType), numberType, null)
        };
    }

    public static int Bytes(this NumberType numberType) {
        return numberType switch {
            NumberType.Byte    => sizeof(byte),
            NumberType.SByte   => sizeof(sbyte),
            NumberType.Short   => sizeof(short),
            NumberType.UShort  => sizeof(ushort),
            NumberType.Int     => sizeof(int),
            NumberType.UInt    => sizeof(uint),
            NumberType.Long    => sizeof(long),
            NumberType.ULong   => sizeof(ulong),
            NumberType.Float   => sizeof(float),
            NumberType.Double  => sizeof(double),
            NumberType.Decimal => sizeof(decimal),
            NumberType.NInt    => Unsafe.SizeOf<nint>(),
            NumberType.NUInt   => Unsafe.SizeOf<nuint>(),
            _                  => throw new ArgumentOutOfRangeException(nameof(numberType), numberType, null)
        };
    }

    #endregion
}