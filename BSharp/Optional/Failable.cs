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
    public class Failable : IFailable {
        public Exception?                Excuse                { get; }
        public bool                      Failed                => Excuse != null;
        public IReadOnlyCollection<Type> IgnoredExceptionTypes { get; }
        public Exception?                IgnoredException      { get; }

        protected Failable(
            Exception?         excuse,
            IEnumerable<Type>? ignoredExceptionTypes,
            Exception?         ignoredException
        ) {
            Excuse                = excuse;
            IgnoredException      = ignoredException;
            IgnoredExceptionTypes = ignoredExceptionTypes?.ToArray() ?? Array.Empty<Type>();
        }

        protected Failable(IFailable other) : this(other.Excuse, other.IgnoredExceptionTypes, other.IgnoredException) { }

        public static Failable Invoke(
            [InstantHandle]
            Action failableAction,
            params Type[] ignoredExceptionTypes
        ) {
            return Invoke(failableAction, ignoredExceptionTypes.AsEnumerable());
        }

        public static Failable Invoke([InstantHandle] Action failableAction, IEnumerable<Type> ignoredExceptionTypes) {
            ignoredExceptionTypes = ignoredExceptionTypes.Must(ReflectionUtils.IsExceptionType).ToArray();

            if (failableAction == null) {
                throw new ArgumentNullException(nameof(failableAction), $"Unable to attempt a {nameof(Failable)} because the {nameof(failableAction)} was null!");
            }

            try {
                failableAction.Invoke();
                return new Failable(default, ignoredExceptionTypes, default);
            }
            catch (Exception e) when (e.IsInstanceOf(ignoredExceptionTypes)) {
                // Handling an ignored exception
                return new Failable(default, ignoredExceptionTypes, e);
            }
            catch (Exception e) {
                // Handling a non-ignored exception
                return new Failable(e, ignoredExceptionTypes, default);
            }
        }

        public override string ToString() {
            return $"{(Failed ? $"{this.GetIcon()} [{Excuse}]" : this.GetIcon())}";
        }
    }
}