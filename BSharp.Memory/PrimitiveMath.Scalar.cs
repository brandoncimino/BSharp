using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Memory;

public static partial class PrimitiveMath {
    /// <summary>
    /// Utility methods for working with <see cref="Vector{T}"/>izeable types.
    /// </summary>
    /// <remarks>
    /// Stolen as verbatim as possible from stolen from <a href="https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Runtime/Intrinsics/Scalar.cs">Scalar.cs</a> from <c>System.Private.CoreLib</c>.
    /// <p/>
    /// 📎 This class should only be exposed through <see cref="PrimitiveMath"/> for a few reasons:
    /// <ul>
    /// <li>The <typeparamref name="T"/> parameter here constrained to <see cref="ValueType"/>, but the more specific <i>(though still slightly too broad)</i> <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/unmanaged-types">unmanaged</a> could be used instead.</li>
    /// <li>This class itself has a generic <typeparamref name="T"/> parameter, which:
    /// <ul>
    /// <li>is ugly</li>
    /// <li><a href="https://www.jetbrains.com/help/resharper/StaticMemberInGenericType.html">Rider hates</a></li>
    /// <li>can't contain extension methods</li>
    /// <li>can be inferred from method parameters</li>
    /// </ul>
    /// </li>
    /// <li>The name <a href="https://en.wikipedia.org/wiki/Scalar">scalar</a> is egregiously unintuitive.</li>
    /// </ul>
    /// </remarks>
    /// <typeparam name="T">a <see cref="Type.IsPrimitive"/> numeric type suitable for <see cref="System.Numerics.Vector"/>ization</typeparam>
    private static class Scalar<T>
        where T : struct {
        private static NotSupportedException NotPrimitiveType() {
            return RejectType("not a primitive numeric type!");
        }

        private static NotSupportedException NotIntegerType() {
            return RejectType("not a primitive integer type!");
        }

        private static NotSupportedException NotFloatingPointType() {
            return RejectType("not a primitive floating-point type!");
        }

        private static NotSupportedException RejectType(string reason, [CallerMemberName] string? caller = default) {
            return caller != null
                       ? new NotSupportedException($"{caller}() type {typeof(T).Name} is invalid: {reason}")
                       : new NotSupportedException($"type {typeof(T).Name} is invalid: {reason}");
        }

        public static T AllBitsSet {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (typeof(T) == typeof(byte)) {
                    return (T)(object)byte.MaxValue;
                }
                else if (typeof(T) == typeof(double)) {
                    return (T)(object)BitConverter.Int64BitsToDouble(-1);
                }
                else if (typeof(T) == typeof(short)) {
                    return (T)(object)(short)-1;
                }
                else if (typeof(T) == typeof(int)) {
                    return (T)(object)-1;
                }
                else if (typeof(T) == typeof(long)) {
                    return (T)(object)(long)-1;
                }
                else if (typeof(T) == typeof(nint)) {
                    return (T)(object)(nint)(-1);
                }
                else if (typeof(T) == typeof(nuint)) {
                    return (T)(object)unchecked((nuint)(-1));
                }
                else if (typeof(T) == typeof(sbyte)) {
                    return (T)(object)(sbyte)-1;
                }
                else if (typeof(T) == typeof(float)) {
                    return (T)(object)BitConverter.Int32BitsToSingle(-1);
                }
                else if (typeof(T) == typeof(ushort)) {
                    return (T)(object)ushort.MaxValue;
                }
                else if (typeof(T) == typeof(uint)) {
                    return (T)(object)uint.MaxValue;
                }
                else if (typeof(T) == typeof(ulong)) {
                    return (T)(object)ulong.MaxValue;
                }
                else {
                    throw NotPrimitiveType();
                }
            }
        }

        public static T One {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (typeof(T) == typeof(byte)) {
                    return (T)(object)(byte)1;
                }
                else if (typeof(T) == typeof(double)) {
                    return (T)(object)(double)1;
                }
                else if (typeof(T) == typeof(short)) {
                    return (T)(object)(short)1;
                }
                else if (typeof(T) == typeof(int)) {
                    return (T)(object)1;
                }
                else if (typeof(T) == typeof(long)) {
                    return (T)(object)(long)1;
                }
                else if (typeof(T) == typeof(nint)) {
                    return (T)(object)(nint)1;
                }
                else if (typeof(T) == typeof(nuint)) {
                    return (T)(object)(nuint)1;
                }
                else if (typeof(T) == typeof(sbyte)) {
                    return (T)(object)(sbyte)1;
                }
                else if (typeof(T) == typeof(float)) {
                    return (T)(object)(float)1;
                }
                else if (typeof(T) == typeof(ushort)) {
                    return (T)(object)(ushort)1;
                }
                else if (typeof(T) == typeof(uint)) {
                    return (T)(object)(uint)1;
                }
                else if (typeof(T) == typeof(ulong)) {
                    return (T)(object)(ulong)1;
                }
                else {
                    throw NotPrimitiveType();
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Abs(T value) {
            // byte, ushort, uint, and ulong should have already been handled

            if (typeof(T) == typeof(double)) {
                return (T)(object)Math.Abs((double)(object)value);
            }
            else if (typeof(T) == typeof(short)) {
                return (T)(object)Math.Abs((short)(object)value);
            }
            else if (typeof(T) == typeof(int)) {
                return (T)(object)Math.Abs((int)(object)value);
            }
            else if (typeof(T) == typeof(long)) {
                return (T)(object)Math.Abs((long)(object)value);
            }
            else if (typeof(T) == typeof(nint)) {
                return (T)(object)Math.Abs((nint)(object)value);
            }
            else if (typeof(T) == typeof(sbyte)) {
                return (T)(object)Math.Abs((sbyte)(object)value);
            }
            else if (typeof(T) == typeof(float)) {
                return (T)(object)Math.Abs((float)(object)value);
            }
            else {
                throw RejectType("not a signed type!");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Add(T left, T right) {
            if (typeof(T) == typeof(byte)) {
                return (T)(object)(byte)((byte)(object)left + (byte)(object)right);
            }
            else if (typeof(T) == typeof(double)) {
                return (T)(object)(double)((double)(object)left + (double)(object)right);
            }
            else if (typeof(T) == typeof(short)) {
                return (T)(object)(short)((short)(object)left + (short)(object)right);
            }
            else if (typeof(T) == typeof(int)) {
                return (T)(object)((int)(object)left + (int)(object)right);
            }
            else if (typeof(T) == typeof(long)) {
                return (T)(object)((long)(object)left + (long)(object)right);
            }
            else if (typeof(T) == typeof(nint)) {
                return (T)(object)((nint)(object)left + (nint)(object)right);
            }
            else if (typeof(T) == typeof(nuint)) {
                return (T)(object)((nuint)(object)left + (nuint)(object)right);
            }
            else if (typeof(T) == typeof(sbyte)) {
                return (T)(object)(sbyte)((sbyte)(object)left + (sbyte)(object)right);
            }
            else if (typeof(T) == typeof(float)) {
                return (T)(object)(float)((float)(object)left + (float)(object)right);
            }
            else if (typeof(T) == typeof(ushort)) {
                return (T)(object)(ushort)((ushort)(object)left + (ushort)(object)right);
            }
            else if (typeof(T) == typeof(uint)) {
                return (T)(object)((uint)(object)left + (uint)(object)right);
            }
            else if (typeof(T) == typeof(ulong)) {
                return (T)(object)((ulong)(object)left + (ulong)(object)right);
            }
            else {
                throw NotPrimitiveType();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Ceiling(T value) {
            if (typeof(T) == typeof(double)) {
                return (T)(object)Math.Ceiling((double)(object)value);
            }
            else if (typeof(T) == typeof(float)) {
                return (T)(object)MathF.Ceiling((float)(object)value);
            }
            else {
                throw NotFloatingPointType();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Divide(T left, T right) {
            if (typeof(T) == typeof(byte)) {
                return (T)(object)(byte)((byte)(object)left / (byte)(object)right);
            }
            else if (typeof(T) == typeof(double)) {
                return (T)(object)(double)((double)(object)left / (double)(object)right);
            }
            else if (typeof(T) == typeof(short)) {
                return (T)(object)(short)((short)(object)left / (short)(object)right);
            }
            else if (typeof(T) == typeof(int)) {
                return (T)(object)((int)(object)left / (int)(object)right);
            }
            else if (typeof(T) == typeof(long)) {
                return (T)(object)((long)(object)left / (long)(object)right);
            }
            else if (typeof(T) == typeof(nint)) {
                return (T)(object)((nint)(object)left / (nint)(object)right);
            }
            else if (typeof(T) == typeof(nuint)) {
                return (T)(object)((nuint)(object)left / (nuint)(object)right);
            }
            else if (typeof(T) == typeof(sbyte)) {
                return (T)(object)(sbyte)((sbyte)(object)left / (sbyte)(object)right);
            }
            else if (typeof(T) == typeof(float)) {
                return (T)(object)(float)((float)(object)left / (float)(object)right);
            }
            else if (typeof(T) == typeof(ushort)) {
                return (T)(object)(ushort)((ushort)(object)left / (ushort)(object)right);
            }
            else if (typeof(T) == typeof(uint)) {
                return (T)(object)((uint)(object)left / (uint)(object)right);
            }
            else if (typeof(T) == typeof(ulong)) {
                return (T)(object)((ulong)(object)left / (ulong)(object)right);
            }
            else {
                throw NotPrimitiveType();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(T left, T right) {
            if (typeof(T) == typeof(byte)) {
                return (byte)(object)left == (byte)(object)right;
            }
            else if (typeof(T) == typeof(double)) {
                return (double)(object)left == (double)(object)right;
            }
            else if (typeof(T) == typeof(short)) {
                return (short)(object)left == (short)(object)right;
            }
            else if (typeof(T) == typeof(int)) {
                return (int)(object)left == (int)(object)right;
            }
            else if (typeof(T) == typeof(long)) {
                return (long)(object)left == (long)(object)right;
            }
            else if (typeof(T) == typeof(nint)) {
                return (nint)(object)left == (nint)(object)right;
            }
            else if (typeof(T) == typeof(nuint)) {
                return (nuint)(object)left == (nuint)(object)right;
            }
            else if (typeof(T) == typeof(sbyte)) {
                return (sbyte)(object)left == (sbyte)(object)right;
            }
            else if (typeof(T) == typeof(float)) {
                return (float)(object)left == (float)(object)right;
            }
            else if (typeof(T) == typeof(ushort)) {
                return (ushort)(object)left == (ushort)(object)right;
            }
            else if (typeof(T) == typeof(uint)) {
                return (uint)(object)left == (uint)(object)right;
            }
            else if (typeof(T) == typeof(ulong)) {
                return (ulong)(object)left == (ulong)(object)right;
            }
            else {
                throw NotPrimitiveType();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ExtractMostSignificantBit(T value) {
            if (typeof(T) == typeof(byte)) {
                uint bits = (byte)(object)value;
                return bits >> 7;
            }
            else if (typeof(T) == typeof(double)) {
#if NET6_0_OR_GREATER
                ulong bits = BitConverter.DoubleToUInt64Bits((double)(object)value);
                return (uint)(bits >> 63);
#else
                throw new NotSupportedException("double requires .NET 6+");
#endif
            }
            else if (typeof(T) == typeof(short)) {
                uint bits = (ushort)(short)(object)value;
                return bits >> 15;
            }
            else if (typeof(T) == typeof(int)) {
                uint bits = (uint)(int)(object)value;
                return bits >> 31;
            }
            else if (typeof(T) == typeof(long)) {
                ulong bits = (ulong)(long)(object)value;
                return (uint)(bits >> 63);
            }
            else if (typeof(T) == typeof(nint)) {
#if TARGET_64BIT
                ulong bits = (ulong)(nint)(object)value;
                return (uint)(bits >> 63);
#else
                uint bits = (uint)(nint)(object)value;
                return bits >> 31;
#endif
            }
            else if (typeof(T) == typeof(nuint)) {
#if TARGET_64BIT
                ulong bits = (ulong)(nuint)(object)value;
                return (uint)(bits >> 63);
#else
                uint bits = (uint)(nuint)(object)value;
                return bits >> 31;
#endif
            }
            else if (typeof(T) == typeof(sbyte)) {
                uint bits = (byte)(sbyte)(object)value;
                return bits >> 7;
            }
            else if (typeof(T) == typeof(float)) {
#if NET6_0_OR_GREATER
                uint bits = BitConverter.SingleToUInt32Bits((float)(object)value);
                return bits >> 31;
#else
                throw new NotImplementedException("float requires .NET 6+");
#endif
            }
            else if (typeof(T) == typeof(ushort)) {
                uint bits = (ushort)(object)value;
                return bits >> 15;
            }
            else if (typeof(T) == typeof(uint)) {
                uint bits = (uint)(object)value;
                return bits >> 31;
            }
            else if (typeof(T) == typeof(ulong)) {
                ulong bits = (ulong)(object)value;
                return (uint)(bits >> 63);
            }
            else {
                throw NotPrimitiveType();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Floor(T value) {
            if (typeof(T) == typeof(double)) {
                return (T)(object)Math.Floor((double)(object)value);
            }
            else if (typeof(T) == typeof(float)) {
                return (T)(object)MathF.Floor((float)(object)value);
            }
            else {
                throw NotFloatingPointType();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterThan(T left, T right) {
            if (typeof(T) == typeof(byte)) {
                return (byte)(object)left > (byte)(object)right;
            }
            else if (typeof(T) == typeof(double)) {
                return (double)(object)left > (double)(object)right;
            }
            else if (typeof(T) == typeof(short)) {
                return (short)(object)left > (short)(object)right;
            }
            else if (typeof(T) == typeof(int)) {
                return (int)(object)left > (int)(object)right;
            }
            else if (typeof(T) == typeof(long)) {
                return (long)(object)left > (long)(object)right;
            }
            else if (typeof(T) == typeof(nint)) {
                return (nint)(object)left > (nint)(object)right;
            }
            else if (typeof(T) == typeof(nuint)) {
                return (nuint)(object)left > (nuint)(object)right;
            }
            else if (typeof(T) == typeof(sbyte)) {
                return (sbyte)(object)left > (sbyte)(object)right;
            }
            else if (typeof(T) == typeof(float)) {
                return (float)(object)left > (float)(object)right;
            }
            else if (typeof(T) == typeof(ushort)) {
                return (ushort)(object)left > (ushort)(object)right;
            }
            else if (typeof(T) == typeof(uint)) {
                return (uint)(object)left > (uint)(object)right;
            }
            else if (typeof(T) == typeof(ulong)) {
                return (ulong)(object)left > (ulong)(object)right;
            }
            else {
                throw NotPrimitiveType();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterThanOrEqual(T left, T right) {
            if (typeof(T) == typeof(byte)) {
                return (byte)(object)left >= (byte)(object)right;
            }
            else if (typeof(T) == typeof(double)) {
                return (double)(object)left >= (double)(object)right;
            }
            else if (typeof(T) == typeof(short)) {
                return (short)(object)left >= (short)(object)right;
            }
            else if (typeof(T) == typeof(int)) {
                return (int)(object)left >= (int)(object)right;
            }
            else if (typeof(T) == typeof(long)) {
                return (long)(object)left >= (long)(object)right;
            }
            else if (typeof(T) == typeof(nint)) {
                return (nint)(object)left >= (nint)(object)right;
            }
            else if (typeof(T) == typeof(nuint)) {
                return (nuint)(object)left >= (nuint)(object)right;
            }
            else if (typeof(T) == typeof(sbyte)) {
                return (sbyte)(object)left >= (sbyte)(object)right;
            }
            else if (typeof(T) == typeof(float)) {
                return (float)(object)left >= (float)(object)right;
            }
            else if (typeof(T) == typeof(ushort)) {
                return (ushort)(object)left >= (ushort)(object)right;
            }
            else if (typeof(T) == typeof(uint)) {
                return (uint)(object)left >= (uint)(object)right;
            }
            else if (typeof(T) == typeof(ulong)) {
                return (ulong)(object)left >= (ulong)(object)right;
            }
            else {
                throw NotPrimitiveType();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessThan(T left, T right) {
            if (typeof(T) == typeof(byte)) {
                return (byte)(object)left < (byte)(object)right;
            }
            else if (typeof(T) == typeof(double)) {
                return (double)(object)left < (double)(object)right;
            }
            else if (typeof(T) == typeof(short)) {
                return (short)(object)left < (short)(object)right;
            }
            else if (typeof(T) == typeof(int)) {
                return (int)(object)left < (int)(object)right;
            }
            else if (typeof(T) == typeof(long)) {
                return (long)(object)left < (long)(object)right;
            }
            else if (typeof(T) == typeof(nint)) {
                return (nint)(object)left < (nint)(object)right;
            }
            else if (typeof(T) == typeof(nuint)) {
                return (nuint)(object)left < (nuint)(object)right;
            }
            else if (typeof(T) == typeof(sbyte)) {
                return (sbyte)(object)left < (sbyte)(object)right;
            }
            else if (typeof(T) == typeof(float)) {
                return (float)(object)left < (float)(object)right;
            }
            else if (typeof(T) == typeof(ushort)) {
                return (ushort)(object)left < (ushort)(object)right;
            }
            else if (typeof(T) == typeof(uint)) {
                return (uint)(object)left < (uint)(object)right;
            }
            else if (typeof(T) == typeof(ulong)) {
                return (ulong)(object)left < (ulong)(object)right;
            }
            else {
                throw NotPrimitiveType();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessThanOrEqual(T left, T right) {
            if (typeof(T) == typeof(byte)) {
                return (byte)(object)left <= (byte)(object)right;
            }
            else if (typeof(T) == typeof(double)) {
                return (double)(object)left <= (double)(object)right;
            }
            else if (typeof(T) == typeof(short)) {
                return (short)(object)left <= (short)(object)right;
            }
            else if (typeof(T) == typeof(int)) {
                return (int)(object)left <= (int)(object)right;
            }
            else if (typeof(T) == typeof(long)) {
                return (long)(object)left <= (long)(object)right;
            }
            else if (typeof(T) == typeof(nint)) {
                return (nint)(object)left <= (nint)(object)right;
            }
            else if (typeof(T) == typeof(nuint)) {
                return (nuint)(object)left <= (nuint)(object)right;
            }
            else if (typeof(T) == typeof(sbyte)) {
                return (sbyte)(object)left <= (sbyte)(object)right;
            }
            else if (typeof(T) == typeof(float)) {
                return (float)(object)left <= (float)(object)right;
            }
            else if (typeof(T) == typeof(ushort)) {
                return (ushort)(object)left <= (ushort)(object)right;
            }
            else if (typeof(T) == typeof(uint)) {
                return (uint)(object)left <= (uint)(object)right;
            }
            else if (typeof(T) == typeof(ulong)) {
                return (ulong)(object)left <= (ulong)(object)right;
            }
            else {
                throw NotPrimitiveType();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Multiply(T left, T right) {
            if (typeof(T) == typeof(byte)) {
                return (T)(object)(byte)((byte)(object)left * (byte)(object)right);
            }
            else if (typeof(T) == typeof(double)) {
                return (T)(object)(double)((double)(object)left * (double)(object)right);
            }
            else if (typeof(T) == typeof(short)) {
                return (T)(object)(short)((short)(object)left * (short)(object)right);
            }
            else if (typeof(T) == typeof(int)) {
                return (T)(object)((int)(object)left * (int)(object)right);
            }
            else if (typeof(T) == typeof(long)) {
                return (T)(object)((long)(object)left * (long)(object)right);
            }
            else if (typeof(T) == typeof(nint)) {
                return (T)(object)((nint)(object)left * (nint)(object)right);
            }
            else if (typeof(T) == typeof(nuint)) {
                return (T)(object)((nuint)(object)left * (nuint)(object)right);
            }
            else if (typeof(T) == typeof(sbyte)) {
                return (T)(object)(sbyte)((sbyte)(object)left * (sbyte)(object)right);
            }
            else if (typeof(T) == typeof(float)) {
                return (T)(object)(float)((float)(object)left * (float)(object)right);
            }
            else if (typeof(T) == typeof(ushort)) {
                return (T)(object)(ushort)((ushort)(object)left * (ushort)(object)right);
            }
            else if (typeof(T) == typeof(uint)) {
                return (T)(object)((uint)(object)left * (uint)(object)right);
            }
            else if (typeof(T) == typeof(ulong)) {
                return (T)(object)((ulong)(object)left * (ulong)(object)right);
            }
            else {
                throw NotPrimitiveType();
            }
        }

        public static bool ObjectEquals(T left, T right) {
            if (typeof(T) == typeof(byte)) {
                return ((byte)(object)left).Equals((byte)(object)right);
            }
            else if (typeof(T) == typeof(double)) {
                return ((double)(object)left).Equals((double)(object)right);
            }
            else if (typeof(T) == typeof(short)) {
                return ((short)(object)left).Equals((short)(object)right);
            }
            else if (typeof(T) == typeof(int)) {
                return ((int)(object)left).Equals((int)(object)right);
            }
            else if (typeof(T) == typeof(long)) {
                return ((long)(object)left).Equals((long)(object)right);
            }
            else if (typeof(T) == typeof(nint)) {
                return ((nint)(object)left).Equals((nint)(object)right);
            }
            else if (typeof(T) == typeof(nuint)) {
                return ((nuint)(object)left).Equals((nuint)(object)right);
            }
            else if (typeof(T) == typeof(sbyte)) {
                return ((sbyte)(object)left).Equals((sbyte)(object)right);
            }
            else if (typeof(T) == typeof(float)) {
                return ((float)(object)left).Equals((float)(object)right);
            }
            else if (typeof(T) == typeof(ushort)) {
                return ((ushort)(object)left).Equals((ushort)(object)right);
            }
            else if (typeof(T) == typeof(uint)) {
                return ((uint)(object)left).Equals((uint)(object)right);
            }
            else if (typeof(T) == typeof(ulong)) {
                return ((ulong)(object)left).Equals((ulong)(object)right);
            }
            else {
                throw NotPrimitiveType();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ShiftLeft(T value, int shiftCount) {
            if (typeof(T) == typeof(byte)) {
                return (T)(object)(byte)((byte)(object)value << (shiftCount & 7));
            }
            else if (typeof(T) == typeof(short)) {
                return (T)(object)(short)((short)(object)value << (shiftCount & 15));
            }
            else if (typeof(T) == typeof(int)) {
                return (T)(object)(int)((int)(object)value << shiftCount);
            }
            else if (typeof(T) == typeof(long)) {
                return (T)(object)(long)((long)(object)value << shiftCount);
            }
            else if (typeof(T) == typeof(nint)) {
                return (T)(object)(nint)((nint)(object)value << shiftCount);
            }
            else if (typeof(T) == typeof(nuint)) {
                return (T)(object)(nuint)((nuint)(object)value << shiftCount);
            }
            else if (typeof(T) == typeof(sbyte)) {
                return (T)(object)(sbyte)((sbyte)(object)value << (shiftCount & 7));
            }
            else if (typeof(T) == typeof(ushort)) {
                return (T)(object)(ushort)((ushort)(object)value << (shiftCount & 15));
            }
            else if (typeof(T) == typeof(uint)) {
                return (T)(object)(uint)((uint)(object)value << shiftCount);
            }
            else if (typeof(T) == typeof(ulong)) {
                return (T)(object)(ulong)((ulong)(object)value << shiftCount);
            }
            else {
                throw NotIntegerType();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ShiftRightArithmetic(T value, int shiftCount) {
            if (typeof(T) == typeof(short)) {
                return (T)(object)(short)((short)(object)value >> (shiftCount & 15));
            }
            else if (typeof(T) == typeof(int)) {
                return (T)(object)(int)((int)(object)value >> shiftCount);
            }
            else if (typeof(T) == typeof(long)) {
                return (T)(object)(long)((long)(object)value >> shiftCount);
            }
            else if (typeof(T) == typeof(nint)) {
                return (T)(object)(nint)((nint)(object)value >> shiftCount);
            }
            else if (typeof(T) == typeof(sbyte)) {
                return (T)(object)(sbyte)((sbyte)(object)value >> (shiftCount & 7));
            }
            else {
                throw RejectType("not a signed primitive numeric type!");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ShiftRightLogical(T value, int shiftCount) {
            if (typeof(T) == typeof(byte)) {
                return (T)(object)(byte)((byte)(object)value >> (shiftCount & 7));
            }
            else if (typeof(T) == typeof(short)) {
                return (T)(object)(short)((ushort)(short)(object)value >> (shiftCount & 15));
            }
            else if (typeof(T) == typeof(int)) {
                return (T)(object)(int)((uint)(int)(object)value >> shiftCount);
            }
            else if (typeof(T) == typeof(long)) {
                return (T)(object)(long)((ulong)(long)(object)value >> shiftCount);
            }
            else if (typeof(T) == typeof(nint)) {
                return (T)(object)(nint)((nuint)(nint)(object)value >> shiftCount);
            }
            else if (typeof(T) == typeof(nuint)) {
                return (T)(object)(nuint)((nuint)(object)value >> shiftCount);
            }
            else if (typeof(T) == typeof(sbyte)) {
                return (T)(object)(sbyte)((byte)(sbyte)(object)value >> (shiftCount & 7));
            }
            else if (typeof(T) == typeof(ushort)) {
                return (T)(object)(ushort)((ushort)(object)value >> (shiftCount & 15));
            }
            else if (typeof(T) == typeof(uint)) {
                return (T)(object)(uint)((uint)(object)value >> shiftCount);
            }
            else if (typeof(T) == typeof(ulong)) {
                return (T)(object)(ulong)((ulong)(object)value >> shiftCount);
            }
            else {
                throw NotPrimitiveType();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Sqrt(T value) {
            if (typeof(T) == typeof(byte)) {
                return (T)(object)(byte)MathF.Sqrt((byte)(object)value);
            }
            else if (typeof(T) == typeof(double)) {
                return (T)(object)(double)Math.Sqrt((double)(object)value);
            }
            else if (typeof(T) == typeof(short)) {
                return (T)(object)(short)MathF.Sqrt((short)(object)value);
            }
            else if (typeof(T) == typeof(int)) {
                return (T)(object)(int)Math.Sqrt((int)(object)value);
            }
            else if (typeof(T) == typeof(long)) {
                return (T)(object)(long)Math.Sqrt((long)(object)value);
            }
            else if (typeof(T) == typeof(nint)) {
                return (T)(object)(nint)Math.Sqrt((nint)(object)value);
            }
            else if (typeof(T) == typeof(nuint)) {
                return (T)(object)(nuint)Math.Sqrt((nuint)(object)value);
            }
            else if (typeof(T) == typeof(sbyte)) {
                return (T)(object)(sbyte)MathF.Sqrt((sbyte)(object)value);
            }
            else if (typeof(T) == typeof(float)) {
                return (T)(object)(float)MathF.Sqrt((float)(object)value);
            }
            else if (typeof(T) == typeof(ushort)) {
                return (T)(object)(ushort)MathF.Sqrt((ushort)(object)value);
            }
            else if (typeof(T) == typeof(uint)) {
                return (T)(object)(uint)Math.Sqrt((uint)(object)value);
            }
            else if (typeof(T) == typeof(ulong)) {
                return (T)(object)(ulong)Math.Sqrt((ulong)(object)value);
            }
            else {
                throw NotPrimitiveType();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Subtract(T left, T right) {
            if (typeof(T) == typeof(byte)) {
                return (T)(object)(byte)((byte)(object)left - (byte)(object)right);
            }
            else if (typeof(T) == typeof(double)) {
                return (T)(object)(double)((double)(object)left - (double)(object)right);
            }
            else if (typeof(T) == typeof(short)) {
                return (T)(object)(short)((short)(object)left - (short)(object)right);
            }
            else if (typeof(T) == typeof(int)) {
                return (T)(object)((int)(object)left - (int)(object)right);
            }
            else if (typeof(T) == typeof(long)) {
                return (T)(object)((long)(object)left - (long)(object)right);
            }
            else if (typeof(T) == typeof(nint)) {
                return (T)(object)((nint)(object)left - (nint)(object)right);
            }
            else if (typeof(T) == typeof(nuint)) {
                return (T)(object)((nuint)(object)left - (nuint)(object)right);
            }
            else if (typeof(T) == typeof(sbyte)) {
                return (T)(object)(sbyte)((sbyte)(object)left - (sbyte)(object)right);
            }
            else if (typeof(T) == typeof(float)) {
                return (T)(object)(float)((float)(object)left - (float)(object)right);
            }
            else if (typeof(T) == typeof(ushort)) {
                return (T)(object)(ushort)((ushort)(object)left - (ushort)(object)right);
            }
            else if (typeof(T) == typeof(uint)) {
                return (T)(object)((uint)(object)left - (uint)(object)right);
            }
            else if (typeof(T) == typeof(ulong)) {
                return (T)(object)((ulong)(object)left - (ulong)(object)right);
            }
            else {
                throw NotPrimitiveType();
            }
        }

        public static T Max(T left, T right) {
            return GreaterThan(right, left) ? right : left;
        }

        public static T Min(T left, T right) {
            return LessThan(right, left) ? right : left;
        }

        /// <summary>Gets <c>true</c> if <typeparamref name="T" /> is supported; otherwise, <c>false</c>.</summary>
        /// <returns><c>true</c> if <typeparamref name="T" /> is supported; otherwise, <c>false</c>.</returns>
        /// <remarks>This is copied from the .NET 7+ <a href="https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector-1.IsSupported?view=net-7.0">Vector&lt;T&gt;.IsSupported</a> property.</remarks>
        public static bool IsSupported {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (typeof(T) == typeof(byte))   ||
                   (typeof(T) == typeof(double)) ||
                   (typeof(T) == typeof(short))  ||
                   (typeof(T) == typeof(int))    ||
                   (typeof(T) == typeof(long))   ||
                   (typeof(T) == typeof(nint))   ||
                   (typeof(T) == typeof(nuint))  ||
                   (typeof(T) == typeof(sbyte))  ||
                   (typeof(T) == typeof(float))  ||
                   (typeof(T) == typeof(ushort)) ||
                   (typeof(T) == typeof(uint))   ||
                   (typeof(T) == typeof(ulong));
        }
    }
}