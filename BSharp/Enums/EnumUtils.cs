using System;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Enums {
    public static class EnumUtils {
        public static T Step<T>(this T currentEnumValue, int step) where T : Enum {
            if (step < 0) {
                throw new IndexOutOfRangeException(
                    $"Wait; I don't think this will work for negative numbers like {step}..."
                );
            }

            int enumCount        = Enum.GetValues(typeof(T)).Length;
            int currentEnumIndex = (int)(object)currentEnumValue;
            int nextEnumIndex    = (currentEnumIndex + step) % enumCount;

            // ReSharper disable once PossibleInvalidCastException
            return (T)(object)nextEnumIndex;
        }

        public static T Next<T>(this T currentEnumValue) where T : Enum {
            return currentEnumValue.Step(1);
        }

        public static T Previous<T>(this T currentEnumValue) where T : Enum {
            return currentEnumValue.Step(-1);
        }

        /// <summary>
        /// A generic wrapper for <see cref="Enum.Parse(System.Type,string)"/> that throws a <b>useful message</b> if <see cref="value"/> is an empty string -
        /// because for some reason, C#'s message in that scenario is "Must specify valid information for parsing in the string."
        /// </summary>
        /// <param name="name">the string name of a <see cref="T"/> value</param>
        /// <typeparam name="T">the type of the <see cref="Enum"/></typeparam>
        /// <returns><see cref="name"/> parsed as a <see cref="T"/> value</returns>
        /// <exception cref="ArgumentException">if <see cref="name"/> is an empty string (i.e. <c>""</c>)</exception>
        public static T Parse<T>(string name, bool ignoreCase = false) where T : struct {
            try {
                return (T)Enum.Parse(typeof(T), name, ignoreCase);
            }
            catch (Exception e) {
                if (name == "") {
                    throw e.PrependMessage($"Cannot parse an empty string as an enum of type {typeof(T).Name}!");
                }
                else {
                    throw;
                }
            }
        }
    }
}