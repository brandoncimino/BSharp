using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Enums {
    /// <summary>
    /// Contains factory methods for building <see cref="EnumSet{T}"/>s.
    /// </summary>
    [PublicAPI]
    public static class EnumSet {
        public static EnumSet<T> Of<T>(params T[] enumValues) where T : struct, Enum {
            return new EnumSet<T>(enumValues);
        }

        public static EnumSet<T> Inverted<T>(this ISet<T> original) where T : struct, Enum {
            return new EnumSet<T>(original);
        }

        public static bool IsEnumSet<T>(this ISet<T> set) where T : struct, Enum {
            return true;
        }


        public static EnumSet<T> Of<T>(IEnumerable<T> values) where T : struct, Enum {
            return new EnumSet<T>(values);
        }


        public static EnumSet<T> OfAllValues<T>() where T : struct, Enum {
            return new EnumSet<T>(BEnum.GetValues<T>());
        }


        public static EnumSet<T> ToEnumSet<T>(this IEnumerable<T> source) where T : struct, Enum {
            return new EnumSet<T>(source);
        }


        public static EnumSet<T> Of<T>(params IEnumerable<T>[] sets) where T : struct, Enum {
            return new EnumSet<T>(sets);
        }
    }
}