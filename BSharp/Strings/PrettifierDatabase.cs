using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Strings.Prettifiers;
using FowlFever.BSharp.Strings.Settings;

namespace FowlFever.BSharp.Strings {
    public interface IPrettifierDatabase {
        IPrettifier? Find(Type type);

        void         Register(IPrettifier prettifier);
        IPrettifier? Deregister(Type      type);

        IPrettifier? Find(Func<IPrettifier, bool> predicate);
    }

    public class PrettifierDatabase : IPrettifierDatabase {
        /// <summary>
        /// All of the <see cref="IPrettifier"/>s in this <see cref="PrettifierDatabase"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="IDictionary{TKey,TValue}"/> is used to make sure that retrieval via indexer is efficient.
        /// </remarks>
        private readonly IDictionary<Type, IPrettifier> Prettifiers;

        public PrettifierDatabase(IEnumerable<IPrettifier> prettifiers) {
            Prettifiers = new PrimaryKeyedList<Type, IPrettifier>(prettifiers).ToDictionary();
        }

        public PrettifierDatabase(params IPrettifier[] prettifiers) : this(prettifiers.AsEnumerable()) { }

        public void Register(IPrettifier prettifier) {
            if (prettifier == null) {
                throw new ArgumentNullException(nameof(prettifier));
            }

            Prettifiers.Add(prettifier.PrimaryKey, prettifier);
        }

        public IPrettifier? Deregister(Type type) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            return Prettifiers.Grab(type);
        }

        public IPrettifier? Find(Type type) => Prettifiers.Grab(type);

        public IPrettifier? Find(Func<IPrettifier, bool> predicate) {
            return Prettifiers.Where(it => predicate(it.Value))
                              .Select(it => it.Value)
                              .FirstOrDefault();
        }

        public static PrettifierDatabase GetDefaultPrettifiers() {
            return new PrettifierDatabase(
                new Prettifier<string>(Convert.ToString),
                new Prettifier<IPrettifiable>(PrettifyPrettifiable),
                new Prettifier<Enum>(InnerPretty.PrettifyEnum),
                new Prettifier<Type>(InnerPretty.PrettifyType),
                new Prettifier<IDictionary>(InnerPretty.PrettifyDictionary3),
                new Prettifier<KeyedList<object, object>>(InnerPretty.PrettifyKeyedList),
                new Prettifier<Match>(InnerPretty.PrettifyRegexMatch),
#if NETSTANDARD2_0
                new Prettifier<(object, object)>(InnerPretty.Tuple2),
                new Prettifier<(object, object, object)>(InnerPretty.Tuple3),
                new Prettifier<(object, object, object, object)>(InnerPretty.Tuple4),
                new Prettifier<(object, object, object, object, object)>(InnerPretty.Tuple5),
                new Prettifier<(object, object, object, object, object, object)>(InnerPretty.Tuple6),
                new Prettifier<(object, object, object, object, object, object, object)>(InnerPretty.Tuple7),
                new Prettifier<(object, object, object, object, object, object, object, object)>(InnerPretty.Tuple8Plus),
#else
                new Prettifier<ITuple>(InnerPretty.PrettifyTuple),
#endif
                new Prettifier<IEnumerable>(InnerPretty.PrettifyEnumerable),
                new Prettifier<MethodInfo>(InnerPretty.PrettifyMethodInfo),
                new Prettifier<ParameterInfo>(InnerPretty.PrettifyParameterInfo),
                new Prettifier<MemberInfo>(InnerPretty.PrettifyMemberInfo),
                new Prettifier<Delegate>(InnerPretty.PrettifyDelegate),
                ObjectToStringPrettifier
            );
        }

        internal static string PrettifyPrettifiable(IPrettifiable prettifiable, PrettificationSettings? settings) => prettifiable.Prettify(settings);

        internal static string PrettifyToString<T>(T obj, PrettificationSettings? settings)
            where T : notnull => obj.ToString().OrNullPlaceholder(settings);

        #region Special High-Priority Prettifiers

        internal static readonly IPrettifier PrettifiablePrettifier   = new Prettifier<IPrettifiable>(PrettifyPrettifiable);
        internal static readonly IPrettifier ObjectToStringPrettifier = new Prettifier<object>(PrettifyToString);

        #endregion
    }
}