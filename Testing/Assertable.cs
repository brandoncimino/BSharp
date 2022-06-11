using System;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Sugar;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace FowlFever.Testing {
    /// <summary>
    /// A special implementation of <see cref="IFailable"/> that handles the special case of <see cref="NUnit.Framework.SuccessException"/>.
    ///
    /// TODO: Replace this with a builder-style class; maybe one o' them fancy new records I keep hearing about
    /// </summary>
    public record Assertable : Failable {
        private Assertable(
            IFailable         failable,
            Supplied<string>? description
        ) : base(
            failable,
            description
        ) { }

        internal Assertable(
            Action            action,
            Supplied<string>? description
        ) : this(
            Invoke(action, null, null, typeof(SuccessException)),
            description
        ) { }

        /// <summary>
        /// TODO: Move this into an instance method of <see cref="MultipleAsserter{TSelf,TActual}"/>
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="assertion"></param>
        /// <param name="constraint"></param>
        /// <param name="message"></param>
        /// <param name="actionResolver"></param>
        internal Assertable(
            Supplied<string>?                                 nickname,
            Action                                            assertion,
            IResolveConstraint                                constraint,
            Func<string>?                                     message,
            Action<Action, IResolveConstraint, Func<string>?> actionResolver
        ) : this(
            () => actionResolver.Invoke(assertion, constraint, message),
            nickname ?? GetNicknameSupplier(assertion, constraint)
        ) { }

        public override string ToString() => this.FormatAssertable().JoinLines();

        internal static Supplied<string> GetNicknameSupplier(object? actual, IResolveConstraint? constraint, PrettificationSettings? settings = default) {
            static string GetNickname(object? actual, IResolveConstraint? constraint, PrettificationSettings? settings) {
                var dName = actual?.Prettify(settings);
                var cName = constraint?.Prettify(settings);
                return dName.JoinNonBlank(cName, " 🗜 ");
            }

            return Lazily.Get(() => GetNickname(actual, constraint, settings));
        }
    }
}