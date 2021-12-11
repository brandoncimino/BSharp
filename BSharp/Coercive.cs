using System;

using FowlFever.BSharp.Chronic;

namespace FowlFever.BSharp {
    public static class Coercive {
        /// <summary>
        /// The different <a href="https://en.wikipedia.org/wiki/Operation_(mathematics)">operations</a> supported by <see cref="Coercively"/>.
        /// </summary>
        public enum Operation {
            Plus,
            Minus,
            Times,
            DividedBy
        }

        /// <summary>
        /// Holds fat <see cref="Func{TResult}"/>s for doing math <see cref="Coercively"/>.
        /// </summary>
        /// <remarks>
        /// If this were Java, then the values in <see cref="Operator"/> would instead be a part of the <see cref="Operation"/> enum.
        /// </remarks>
        internal sealed class Operator {
            /// <summary>
            /// The corresponding <see cref="Operation"/>
            /// </summary>
            public Operation Operation { get; internal set; }
            /// <summary>
            /// The noun version of this <see cref="Operator"/>, e.g. "Addition".
            /// </summary>
            public string Noun { get; internal set; }
            /// <summary>
            /// The verb version of this <see cref="Operator"/>, e.g. "Add".
            /// </summary>
            public string Verb { get; internal set; }
            /// <summary>
            /// The symbol used for this <see cref="Operator"/>, e.g. "+".
            /// </summary>
            public string Symbol { get; internal set; }
            /// <summary>
            /// The name of the <see cref="Symbol"/>, i.e. the way we would read it out loud; e.g. "1 + 4" -> "one plus four"
            /// </summary>
            public string SymbolName { get; internal set; }

            #region Funcs

            public Func<int, int, object>           Int      { get; internal set; }
            public Func<float, float, object>       Float    { get; internal set; }
            public Func<double, double, object>     Double   { get; internal set; }
            public Func<long, long, object>         Long     { get; internal set; }
            public Func<decimal, decimal, object>   Decimal  { get; internal set; }
            public Func<short, short, object>       Short    { get; internal set; }
            public Func<string, string, object>     String   { get; internal set; }
            public Func<DateTime, TimeSpan, object> DateTime { get; internal set; }
            public Func<TimeSpan, object, object>   TimeSpan { get; internal set; }
            public Func<byte, byte, object>         Byte     { get; internal set; }
            public Func<uint, uint, object>         UInt     { get; internal set; }
            public Func<sbyte, sbyte, object>       SByte    { get; internal set; }
            public Func<ulong, ulong, object>       ULong    { get; internal set; }
            public Func<ushort, ushort, object>     UShort   { get; internal set; }

            #endregion

            public object Apply(object a, object b) {
                return a switch {
                    // Integral types
                    byte bt     => Byte?.Invoke(bt, Convert.ToByte(b)),
                    sbyte sb    => SByte?.Invoke(sb, Convert.ToSByte(b)),
                    short sh    => Short?.Invoke(sh, Convert.ToInt16(b)),
                    ushort ush  => UShort?.Invoke(ush, Convert.ToUInt16(b)),
                    int i       => Int?.Invoke(i, Convert.ToInt32(b)),
                    uint ui     => UInt?.Invoke(ui, Convert.ToUInt32(b)),
                    long l      => Long?.Invoke(l, Convert.ToInt64(b)),
                    ulong ul    => ULong?.Invoke(ul, Convert.ToUInt64(b)),
                    float f     => Float?.Invoke(f, Convert.ToSingle(b)),
                    double d    => Double?.Invoke(d, Convert.ToDouble(b)),
                    decimal dec => Decimal?.Invoke(dec, Convert.ToDecimal(b)),
                    string str  => String?.Invoke(str, Convert.ToString(b)),
                    DateTime dt => DateTime?.Invoke(dt, TimeUtils.TimeSpanOf(b)),
                    TimeSpan ts => TimeSpan?.Invoke(ts, b),
                    _           => throw new ArgumentException($"I don't know how to coercively operate on {a.GetType()} and {b.GetType()}!")
                } ?? throw DoesNotApply(a, b);
            }

            private InvalidOperationException DoesNotApply(object a, object b) {
                return new InvalidOperationException($"Could not {Verb} [{a.GetType()}]{a} and [{b.GetType()}]{b}!");
            }
        }
    }
}