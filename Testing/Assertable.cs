using System;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace FowlFever.Testing {
    public interface IAssertable : IFailable {
        public Func<string> Nickname { get; }
    }

    /// <summary>
    /// A special implementation of <see cref="IFailable"/> that handles the special case of <see cref="NUnit.Framework.SuccessException"/>.
    ///
    /// TODO: Replace this with a builder-style class; maybe one o' them fancy new records I keep hearing about
    /// </summary>
    public class Assertable : Failable, IAssertable {
        public Func<string> Nickname { get; }

        private Assertable(
            IFailable    failable,
            Func<string> nickname
        ) : base(
            failable
        ) {
            Nickname = nickname;
        }

        internal Assertable(
            Action       action,
            Func<string> nickname
        ) : this(
            Invoke(action, typeof(SuccessException)),
            nickname
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
            Func<string>?                                     nickname,
            Action                                            assertion,
            IResolveConstraint                                constraint,
            Func<string>?                                     message,
            Action<Action, IResolveConstraint, Func<string>?> actionResolver
        ) : this(
            () => actionResolver.Invoke(assertion, constraint, message),
            nickname ?? GetNicknameSupplier(assertion, constraint)
        ) { }

        public override string ToString() {
            return this.FormatAssertable().JoinLines();
        }

        internal static Func<string> GetNicknameSupplier(object? actual, IResolveConstraint? constraint, PrettificationSettings? settings = default) {
            return () => GetNickname(actual, constraint, settings);
        }

        private static string GetNickname(object? actual, IResolveConstraint? constraint, PrettificationSettings? settings) {
            var dName = actual?.Prettify(settings);
            var cName = constraint?.Prettify(settings);
            return dName.JoinNonBlank(cName, " 🗜 ");
        }
    }
}