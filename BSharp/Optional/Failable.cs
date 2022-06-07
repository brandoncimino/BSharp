using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Reflection;

using JetBrains.Annotations;

[assembly: InternalsVisibleTo("FowlFever.Testing")]

namespace FowlFever.BSharp.Optional {
    /**
     * <inheritdoc cref="IFailable"/>
     */
    public record Failable : IFailable {
        public Exception?                Excuse                { get; }
        public bool                      Failed                => Excuse != null;
        public IReadOnlyCollection<Type> IgnoredExceptionTypes { get; }
        public Exception?                IgnoredException      { get; }
        public string?                   Expression            { get; }

        internal Failable(
            Exception?         excuse,
            IEnumerable<Type>? ignoredExceptionTypes,
            Exception?         ignoredException,
            string?            expression
        ) {
            Excuse                = excuse;
            IgnoredException      = ignoredException;
            IgnoredExceptionTypes = ignoredExceptionTypes?.ToArray() ?? Array.Empty<Type>();
            Expression            = expression;
        }

        protected Failable(IFailable other) : this(other.Excuse, other.IgnoredExceptionTypes, other.IgnoredException, other.Expression) { }

        public static Failable Invoke(
            [InstantHandle]
            Action failableAction,
            IEnumerable<Type> ignoredExceptionTypes,
            [CallerArgumentExpression("failableAction")]
            string? expression = default
        ) {
            ignoredExceptionTypes = ignoredExceptionTypes.Must(ReflectionUtils.IsExceptionType).ToArray();

            if (failableAction == null) {
                throw new ArgumentNullException(nameof(failableAction), $"Unable to attempt a {nameof(Failable)} because the {nameof(failableAction)} was null!");
            }

            try {
                failableAction.Invoke();
                return new Failable(default, ignoredExceptionTypes, default, expression);
            }
            catch (Exception e) when (e.IsInstanceOf(ignoredExceptionTypes)) {
                // Handling an ignored exception
                return new Failable(default, ignoredExceptionTypes, e, expression);
            }
            catch (Exception e) {
                // Handling a non-ignored exception
                return new Failable(e, ignoredExceptionTypes, default, expression);
            }
        }

        public static Failable Invoke([InstantHandle] Action failableAction, [CallerArgumentExpression("failableAction")] string? expression = default, params Type[] ignoredExceptionTypes) {
            return Invoke(failableAction, ignoredExceptionTypes, expression);
        }

        public override string ToString() {
            return $"{Expression} ⇒ {(Failed ? $"{this.GetIcon()} [{Excuse}]" : this.GetIcon())}";
        }
    }
}