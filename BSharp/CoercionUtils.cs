using System;
using System.Diagnostics.Contracts;

using FowlFever.BSharp.Chronic;

namespace FowlFever.BSharp {
    [Obsolete("Please use " + nameof(Coercively) + " instead")]
    public static class CoercionUtils {
        /// <summary>
        /// Attempts to add <paramref name="a"/> and <paramref name="b"/> together by coercing them into compatible types.
        /// <p/>
        /// If <paramref name="sign"/> is negative, subtraction is performed instead.
        /// <p/>
        /// Currently handles, as of 9/15/2020:
        /// <li>int</li>
        /// <li>long</li>
        /// <li>float</li>
        /// <li>double</li>
        /// <li>decimal</li>
        /// <li><see cref="DateTime"/> + <see cref="TimeSpan"/></li>
        /// <li><see cref="TimeSpan"/> + <see cref="TimeSpan"/></li>
        /// <li><see cref="DateTime"/> + <see cref="TimeSpan"/>.<see cref="TimeSpan.FromTicks">FromTicks( int / long / float / double / decimal )</see></li>
        /// <li><see cref="TimeSpan"/> + <see cref="TimeSpan"/>.<see cref="TimeSpan.FromTicks">FromTicks( int / long / float / double / decimal )</see></li>
        /// <li><see cref="Vector2"/> + <see cref="Vector2"/> / <see cref="Vector3"/> / <see cref="Vector4"/></li>
        /// <li><see cref="Vector3"/> + <see cref="Vector2"/> / <see cref="Vector3"/> / <see cref="Vector4"/></li>
        /// <li><see cref="Vector4"/> + <see cref="Vector2"/> / <see cref="Vector3"/> / <see cref="Vector4"/></li>
        /// </summary>
        /// <remarks>
        /// This is meant to be a strict, but more performant equivalent to:
        /// <code><![CDATA[
        /// (dynamic)a + (dynamic)b;
        /// ]]></code>
        /// as that is extremely slow (upwards of 0.3 seconds!!)
        /// <br/>
        /// This uses <see cref="Convert"/> methods in place of (cast) operators in some locations due to runtime exceptions when casting <see cref="object"/>s around as different types. See <a href="https://ericlippert.com/2009/03/03/representation-and-identity/">Representation and Identity</a>
        /// </remarks>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        [Pure]
        [Obsolete("Please use " + nameof(Coercively) + "." + nameof(Coercively.Add) + " instead")]
        public static object CoerciveAddition(object a, object b, int sign = 1) {
            //store the sign of "sign" (in case somebody passed us a weird number)
            sign = Math.Sign(sign);

            //attempt to cast "a"
            switch (a) {
                //if it's a DateTime, try to cast "b" to a TimeSpan
                case DateTime a_date: {
                    var b_span = TimeUtils.TimeSpanFromObject(b);
                    if (b_span != null) {
                        return a_date + b_span.Value.Multiply(sign);
                    }
                    else {
                        throw new InvalidCastException(
                            $"Couldn't add {nameof(a)} ({a}) and {nameof(b)} ({b}) " +
                            $"because {nameof(a)} was cast to a {nameof(DateTime)} " +
                            $"while {nameof(b)} couldn't be cast / converted to a {nameof(TimeSpan)}!"
                        );
                    }
                }
                case TimeSpan a_span: {
                    var b_span = TimeUtils.TimeSpanFromObject(b);
                    if (b_span != null) {
                        return a_span + b_span.Value.Multiply(sign);
                    }
                    else {
                        throw new InvalidCastException(
                            $"Couldn't add {nameof(a)} ({a}) and {nameof(b)} ({b}) "            +
                            $"because {nameof(a)} was cast (directly) to a {nameof(TimeSpan)} " +
                            $"while {nameof(b)} couldn't be cast / converted to a {nameof(TimeSpan)}!"
                        );
                    }
                }
                case int a_int:
                    return a_int + (Convert.ToInt32(b) * sign);
                case long a_long:
                    return a_long + (Convert.ToInt64(b) * sign);
                case float a_float:
                    return a_float + (Convert.ToSingle(b) * sign);
                case double a_double:
                    return a_double + (Convert.ToDouble(b) * sign);
                case decimal a_decimal:
                    return a_decimal + (Convert.ToDecimal(b) * sign);

                // TODO: Find a better way to mix standard and Unity classes.
                //  Maybe try something like AssertJ, making a special functional interface called, like, "coercer" or something?
                //  (I'm specifically referring to AssertJ's withComparatorForType())
                /*
                case Vector2 a_vector2:
                    try {
                        return a_vector2 + (Vector2) b * sign;
                    }
                    catch (InvalidCastException e) {
                        throw new InvalidCastException($"Unable to cast {nameof(b)} ({b}) to a {nameof(Vector2)} to add it to {nameof(a)} ({a_vector2})!", e);
                    }
                case Vector3 a_vector3:
                    try {
                        return a_vector3 + (Vector3) b * sign;
                    }
                    catch (InvalidCastException e) {
                        throw new InvalidCastException($"Unable to cast {nameof(b)} ({b}) to a {nameof(Vector3)} to add it to {nameof(a)} ({a_vector3})!", e);
                    }
                case Vector4 a_vector4:
                    try {
                        return a_vector4 + (Vector4) b * sign;
                    }
                    catch (InvalidCastException e) {
                        throw new InvalidCastException($"Unable to cast {nameof(b)} ({b}) to a {nameof(Vector4)} to add it to {nameof(a)} ({a_vector4})!", e);
                    }
                */
                default:
                    throw new InvalidCastException($"Unable to coerce {nameof(a)} ({a}) and {nameof(b)} ({b}) into types compatible with addition!");
            }
        }

        [Pure]
        [Obsolete("Please use " + nameof(Coercively) + "." + nameof(Coercively.Add) + " instead")]
        public static object CoerciveSubtraction(object a, object b) {
            return CoerciveAddition(a, b, -1);
        }
    }
}