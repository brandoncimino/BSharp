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
            Failable     failable,
            Func<string> nickname
        ) : base(
            failable
        ) {
            Nickname = nickname;
        }

        private Assertable(
            Action       action,
            Func<string> nickname
        ) : this(
            Invoke(action, typeof(SuccessException)),
            nickname
        ) { }

        public Assertable(
            Func<string>?                                           nickname,
            TestDelegate                                            assertion,
            IResolveConstraint                                      constraint,
            Func<string>?                                           message,
            Action<TestDelegate, IResolveConstraint, Func<string>?> actionResolver
        ) : this(
            () => actionResolver.Invoke(assertion, constraint, message),
            nickname ?? GetNicknameSupplier(assertion, constraint)
        ) { }

        public Assertable(
            Func<string>?                                                          nickname,
            ActualValueDelegate<object>                                            actual,
            IResolveConstraint                                                     constraint,
            Func<string>?                                                          message,
            Action<ActualValueDelegate<object>, IResolveConstraint, Func<string>?> resolver
        ) : this(
            () => resolver.Invoke(actual, constraint, message),
            nickname ?? GetNicknameSupplier(actual, constraint)
        ) { }

        public override string ToString() {
            return this.FormatAssertable().JoinLines() ?? base.ToString();
        }

        public static IAssertable Assert<TActual>(
            Func<string>?                                                           nickname,
            ActualValueDelegate<TActual>                                            actual,
            IResolveConstraint                                                      constraint,
            Func<string>?                                                           message,
            Action<ActualValueDelegate<TActual>, IResolveConstraint, Func<string>?> resolver
        ) {
            return new Assertable(
                () => resolver.Invoke(actual, constraint, message),
                nickname ?? GetNicknameSupplier(actual, constraint)
            );
        }


        internal static Func<string> GetNicknameSupplier(Delegate? dgate, IResolveConstraint? constraint, PrettificationSettings? settings = default) {
            return () => GetNickname(dgate, constraint, settings);
        }


        private static string GetNickname(Delegate? dgate, IResolveConstraint? constraint, PrettificationSettings? settings) {
            var dName = dgate?.Prettify(settings);
            var cName = constraint?.Prettify(settings);
            var parts = new[] { dName, cName };
            return parts.NonBlank().JoinString(" 🗜 ");
        }
    }
}