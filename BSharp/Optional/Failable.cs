using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

[assembly: InternalsVisibleTo("FowlFever.Testing")]

namespace FowlFever.BSharp.Optional {
    /**
     * <inheritdoc cref="IFailable"/>
     */
    public record Failable : IFailable {
        public Exception? Excuse { get; }
        [MemberNotNullWhen(true, nameof(Excuse))]
        public bool Failed => Excuse != null;
        public IReadOnlyCollection<Type> IgnoredExceptionTypes { get; }
        public Exception?                IgnoredException      { get; }
        public Supplied<string?>?        Description           { get; }

        internal Failable(
            Exception?         excuse,
            IEnumerable<Type>? ignoredExceptionTypes,
            Exception?         ignoredException,
            Supplied<string?>? description
        ) {
            Excuse                = excuse;
            IgnoredException      = ignoredException;
            IgnoredExceptionTypes = ignoredExceptionTypes?.ToArray() ?? Array.Empty<Type>();
            Description           = description;
        }

        protected Failable(IFailable other, Supplied<string?>? description = default) : this(other.Excuse, other.IgnoredExceptionTypes, other.IgnoredException, description ?? other.Description) { }

        public static Failable Invoke(
            [InstantHandle]
            Action failableAction,
            IEnumerable<Type>  ignoredExceptionTypes,
            Supplied<string?>? description = default,
            [CallerArgumentExpression("failableAction")]
            string? expression = default
        ) {
            ignoredExceptionTypes = ignoredExceptionTypes.AllMust(ReflectionUtils.IsExceptionType).ToArray();
            var resolvedDescription = description ?? expression;

            if (failableAction == null) {
                throw new ArgumentNullException(nameof(failableAction), $"Unable to attempt a {nameof(Failable)} because the {nameof(failableAction)} was null!");
            }

            try {
                failableAction.Invoke();
                return new Failable(default, ignoredExceptionTypes, default, resolvedDescription);
            }
            catch (Exception e) when (e.IsInstanceOf(ignoredExceptionTypes)) {
                // Handling an ignored exception
                return new Failable(default, ignoredExceptionTypes, e, resolvedDescription);
            }
            catch (Exception e) {
                // Handling a non-ignored exception
                return new Failable(e, ignoredExceptionTypes, default, expression);
            }
        }

        public static Failable Invoke(
            [InstantHandle]
            Action failableAction,
            Supplied<string?>? description = default,
            [CallerArgumentExpression("failableAction")]
            string? expression = default,
            params Type[] ignoredExceptionTypes
        ) {
            return Invoke(failableAction, ignoredExceptionTypes, description, expression);
        }

        public override string ToString() {
            return $"{(Failed ? $"{this.GetIcon()} [{Excuse}]" : this.GetIcon())} {Description.GetValueOrDefault().IfBlank(GetType().Name)}";
        }
    }
}