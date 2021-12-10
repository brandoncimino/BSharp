using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone;
using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Optional;
using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BrandonUtils.Testing {
    [PublicAPI]
    public abstract class MultipleAsserter<TSelf, TActual> : IMultipleAsserter where TSelf : MultipleAsserter<TSelf, TActual>, new() {
        private const string HeadingIcon = "🧪";

        public PrettificationSettings PrettificationSettings { get; protected set; }

        private Optional<ActualValueDelegate<TActual>> _actual;
        /// <summary>
        /// The actual value being asserted against (if there is one)
        /// </summary>
        private Optional<ActualValueDelegate<TActual>> Actual {
            get => _actual;
            set {
                if (_actual.HasValue) {
                    throw new InvalidOperationException($"The {nameof(Actual)} value has already been set!");
                }

                _actual = value.Select(AndAlsoCache);
            }
        }

        /// <summary>
        /// Contains the result of the latest time <see cref="Actual"/> was <see cref="ActualValueDelegate{TActual}.Invoke"/>d.
        /// </summary>
        private Optional<TActual> CachedActual;


        /// <param name="originalActualValueDelegate">the original <see cref="ActualValueDelegate{TActual}"/></param>
        /// <returns>a new <see cref="ActualValueDelegate{TActual}"/> that executes the <paramref name="originalActualValueDelegate"/> while <b>also</b> storing the result in <see cref="CachedActual"/></returns>
        private ActualValueDelegate<TActual> AndAlsoCache(ActualValueDelegate<TActual> originalActualValueDelegate) {
            return () => {
                var v = originalActualValueDelegate.Invoke();
                CachedActual = v;
                return v;
            };
        }

        public Func<string> Heading { get; set; }

        public int Indent { get; set; }

        internal IList<(Action action, Func<string> nickname)>                                                     Actions_AgainstAnything     { get; } = new List<(Action, Func<string>)>();
        internal IList<(Action<TActual> action, Func<string> nickname)>                                            Actions_AgainstActual       { get; } = new List<(Action<TActual>, Func<string>)>();
        internal IList<(IResolveConstraint constraint, Func<string> nickname)>                                     Constraints_AgainstActual   { get; } = new List<(IResolveConstraint, Func<string>)>();
        internal IList<(object target, IResolveConstraint constraint, Func<string> nickname)>                      Constraints_AgainstAnything { get; } = new List<(object, IResolveConstraint, Func<string>)>();
        internal IList<(ActualValueDelegate<object> target, IResolveConstraint constraint, Func<string> nickname)> Constraints_AgainstDelegate { get; } = new List<(ActualValueDelegate<object>, IResolveConstraint, Func<string>)>();

        internal IList<IMultipleAsserter> Asserters { get; } = new List<IMultipleAsserter>();

        internal IList<(Delegate transformation, IResolveConstraint constraint, Func<string> nickname)> Constraints_AgainstTransformation { get; } = new List<(Delegate, IResolveConstraint, Func<string>)>();

        protected abstract void OnFailure(string results);


        protected virtual void OnSuccess(string results) {
            Console.WriteLine(results);
        }

        public abstract void ResolveFunc<T>(
            ActualValueDelegate<T> actual,
            IResolveConstraint     constraint,
            Func<string>?          message
        );

        public abstract void ResolveAction(
            TestDelegate       action,
            IResolveConstraint constraint,
            Func<string>?      message
        );

        private Optional<Exception> ShortCircuitException;

        protected MultipleAsserter() { }

        protected MultipleAsserter(TActual actual) : this(() => actual) { }

        protected MultipleAsserter(ActualValueDelegate<TActual> actualValueDelegate) {
            Actual = actualValueDelegate;
        }

        private TSelf Self => this as TSelf;

        //TODO: Make an extension method of this called "AsFunc" or something
        [ContractAnnotation("null => null")]
        [ContractAnnotation("notnull => notnull")]
        private Func<T>? AsFunc<T>(T? obj) {
            return obj switch {
                null       => null,
                string str => str.IsBlank() ? default(Func<T>) : () => obj,
                _          => () => obj
            };
        }

        #region Builder

        [MustUseReturnValue]
        public TSelf Against(TActual? actual) {
            Actual = new Optional<ActualValueDelegate<TActual>>(() => actual);
            return Self;
        }

        [MustUseReturnValue]
        public TSelf Against(ActualValueDelegate<TActual> actualValueDelegate) {
            Actual = actualValueDelegate;
            return Self;
        }

        #region "And" Constraints

        #region Actions_AgainstAnything

        private TSelf _Add_Action_AgainstAnything(Action action, Func<string>? nickname) {
            Actions_AgainstAnything.Add((action, nickname));
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And(Action action, Func<string>? nickname = default) => _Add_Action_AgainstAnything(action, nickname);

        [MustUseReturnValue]
        public TSelf And(Action action, string nickname) => _Add_Action_AgainstAnything(action, AsFunc(nickname));

        [MustUseReturnValue]
        public TSelf And(IEnumerable<Action> actions) {
            actions?.ForEach(it => _ = _Add_Action_AgainstAnything(it, default));
            return Self;
        }

        #endregion

        #region Actions_AgainstActual

        private TSelf _Add_Action_AgainstActual(Action<TActual> action, Func<string>? nickname) {
            Actions_AgainstActual.Add((action, nickname));
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And(
            Action<TActual> action,
            Func<string>?   nickname = default
        ) =>
            _Add_Action_AgainstActual(
                action,
                nickname
            );

        [MustUseReturnValue]
        public TSelf And(
            Action<TActual> action,
            string?         nickname
        ) =>
            _Add_Action_AgainstActual(action, AsFunc(nickname));

        #endregion

        #region Constraints_AgainstActual

        [MustUseReturnValue]
        public TSelf And(IResolveConstraint? constraint, Func<string>? nickname = default) {
            Constraints_AgainstActual.AddNonNull((constraint, message: nickname));
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And(IResolveConstraint? constraint, string nickname) => And(constraint, () => nickname);

        [MustUseReturnValue]
        public TSelf And(IEnumerable<IResolveConstraint?>? constraints) {
            constraints?.ForEach(it => _ = And(it));
            return Self;
        }

        #endregion

        #region Constraints_AgainstAnything

        private TSelf _Add_Constraint_AgainstAnything(object? target, IResolveConstraint constraint, Func<string>? nickname) {
            Constraints_AgainstAnything.Add((target, constraint, nickname));
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And(
            object?            target,
            IResolveConstraint constraint,
            Func<string>?      nickname = default
        ) =>
            _Add_Constraint_AgainstAnything(target, constraint, nickname);

        [MustUseReturnValue]
        public TSelf And(IEnumerable<(object target, IResolveConstraint constraint)>? constraints) {
            constraints?.ForEach(it => _ = _Add_Constraint_AgainstAnything(it.target, it.constraint, default));
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And(
            object?            target,
            IResolveConstraint constraint,
            string?            nickname
        ) =>
            _Add_Constraint_AgainstAnything(target, constraint, AsFunc(nickname));

        #endregion

        #region Constraints_AgainstDelegate

        private TSelf _Add_Constraint_AgainstDelegate(ActualValueDelegate<object> supplier, IResolveConstraint constraint, Func<string>? nickname) {
            Constraints_AgainstDelegate.AddNonNull((supplier, constraint, nickname));
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And(ActualValueDelegate<object> supplier, IResolveConstraint constraint, Func<string>? nickname = default) => _Add_Constraint_AgainstDelegate(supplier, constraint, nickname);

        [MustUseReturnValue]
        public TSelf And(ActualValueDelegate<object> supplier, IResolveConstraint constraint, string? nickname) => _Add_Constraint_AgainstDelegate(supplier, constraint, AsFunc(nickname));

        #endregion

        #region Constraints_AgainstTransformation

        private TSelf _Add_Constraint_AgainstTransformation<TNew>(Func<TActual, TNew> actualTransformation, IResolveConstraint constraint, Func<string>? nickname) {
            nickname ??= Assertable.GetNicknameSupplier(actualTransformation, constraint);
            Constraints_AgainstTransformation.Add((actualTransformation, constraint, nickname));
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And<TNew>(Func<TActual, TNew> tf, IResolveConstraint constraint, Func<string>? nickname = default) => _Add_Constraint_AgainstTransformation(tf, constraint, nickname);

        [MustUseReturnValue]
        public TSelf And<TNew>(Func<TActual, TNew> tf, IResolveConstraint constraint, string? nickname) => _Add_Constraint_AgainstTransformation(tf, constraint, AsFunc(nickname));

        [MustUseReturnValue]
        public TSelf And<TNew>(IEnumerable<(Func<TActual, TNew>, IResolveConstraint)>? constraints) {
            constraints?.ForEach(it => _ = _Add_Constraint_AgainstTransformation(it.Item1, it.Item2, default));
            return Self;
        }

        #endregion

        #region Asserters

        private TSelf _Add_Asserter(IMultipleAsserter asserter) {
            Asserters.Add(asserter);
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And(IMultipleAsserter asserter) => _Add_Asserter(asserter);

        #endregion

        #endregion

        #region WithHeading

        [MustUseReturnValue]
        public TSelf WithHeading(Func<string> headingSupplier) {
            Heading = headingSupplier;
            return Self;
        }

        [MustUseReturnValue]
        public TSelf WithHeading(string? heading) {
            Heading = heading.IsNotBlank() ? () => heading : (Func<string>)default;
            return Self;
        }

        #endregion

        #region WithPrettificationSettings

        public TSelf WithPrettificationSettings(PrettificationSettings settings) {
            PrettificationSettings = settings;
            return Self;
        }

        #endregion

        #region With Indent

        [MustUseReturnValue]
        public TSelf WithIndent(int indent) {
            Indent = indent;
            return Self;
        }

        #endregion

        #endregion

        #region Executing Test Assertions

        private IAssertable Test_Action_AgainstAnything((Action action, Func<string> nickname) ass) => new Assertable(
            ass.nickname ?? Assertable.GetNicknameSupplier(ass.action, default),
            new TestDelegate(ass.action),
            Throws.Nothing,
            default,
            ResolveAction
        );

        private IAssertable Test_Action_AgainstActual((Action<TActual> action, Func<string> nickname) ass) {
            var actual = Actual.OrElseThrow(ActualIsEmptyException($"Could not execute the {ass.GetType().Prettify(PrettificationSettings)} {ass.action.Method.Name}"));
            return new Assertable(
                ass.nickname ?? Assertable.GetNicknameSupplier(ass.action, default),
                () => ass.action.Invoke(actual.Invoke()),
                Throws.Nothing,
                default,
                ResolveAction
            );
        }

        private IAssertable Test_Constraint_AgainstActual((IResolveConstraint constraint, Func<string> message) againstActual) {
            var actualValueDelegate = Actual.OrElseThrow(ActualIsEmptyException(againstActual.Item1));
            return Assertable.Assert(
                default,
                actualValueDelegate,
                againstActual.constraint,
                againstActual.message,
                ResolveFunc
            );
        }

        private IAssertable Test_Constraint_AgainstAnything((object, IResolveConstraint, Func<string>) constraint_againstAnything) {
            var (target, resolveConstraint, message) = constraint_againstAnything;
            return new Assertable(default, () => target, resolveConstraint, message, ResolveFunc);
        }

        private IAssertable Test_Constraint_AgainstDelegate((ActualValueDelegate<object> tDelegate, IResolveConstraint constraint, Func<string> nickname) constraint_againstDelegate) {
            var (tDelegate, constraint, nickname) = constraint_againstDelegate;
            return new Assertable(nickname, tDelegate, constraint, default, ResolveFunc);
        }

        private IAssertable Test_Constraint_AgainstTransformation((Delegate transformation, IResolveConstraint constraint, Func<string> nickname) constraint_againstTransformation) {
            var (transformation, constraint, nickname) = constraint_againstTransformation;
            return new Assertable(
                nickname ?? Assertable.GetNicknameSupplier(transformation, constraint),
                () => transformation.DynamicInvoke(Actual.OrElseThrow(ActualIsEmptyException(constraint)).Invoke()),
                constraint,
                default,
                ResolveFunc
            );
        }

        private IAssertable Test_Asserter(IMultipleAsserter asserter) {
            return new Assertable(
                asserter.Heading,
                asserter.Invoke,
                Throws.Nothing,
                default,
                ResolveAction
            );
        }

        #endregion

        #region Validations / Exceptions

        private ActualValueDelegate<TActual> MustGetActualValue(string message) {
            return Actual.OrElseThrow(ActualIsEmptyException(message));
        }

        private Func<InvalidOperationException> ActualIsEmptyException(IResolveConstraint constraint) {
            return ActualIsEmptyException($"Could not convert the {constraint.GetType().Name} to an {nameof(Action)}");
        }

        private Func<InvalidOperationException> ActualIsEmptyException(string message) {
            return () => new InvalidOperationException($"{message}: this {GetType().Prettify(PrettificationSettings)} doesn't have {nameof(Actual)} value!");
        }

        #endregion

        private IEnumerable<IAssertable> TestEverything() {
            //TODO: Create dedicated classes for each of this constraint/action types; and/or a clean interface for them
            return Actions_AgainstAnything.Select(Test_Action_AgainstAnything)
                                          .Concat(Actions_AgainstActual.Select(Test_Action_AgainstActual))
                                          .Concat(Constraints_AgainstActual.Select(Test_Constraint_AgainstActual))
                                          .Concat(Constraints_AgainstAnything.Select(Test_Constraint_AgainstAnything))
                                          .Concat(Constraints_AgainstTransformation.Select(Test_Constraint_AgainstTransformation))
                                          .Concat(Constraints_AgainstDelegate.Select(Test_Constraint_AgainstDelegate))
                                          .Concat(Asserters.Select(Test_Asserter));
        }

        [ContractAnnotation("=> stop")]
        public void ShortCircuit(Exception shortCircuitException) {
            ShortCircuitException = shortCircuitException;
            Invoke();
        }

        #region formatting

        private IEnumerable<string> FormatFailures([InstantHandle] IEnumerable<IAssertable> testResults) {
            testResults = testResults.ToList();
            var failures = testResults.Where(it => it.Failed).ToList();

            var prettySettings = new PrettificationSettings() {
                PreferredLineStyle = { Value = LineStyle.Single },
                LineLengthLimit    = { Value = 30 },
                TypeLabelStyle     = { Value = TypeNameStyle.Short }
            };

            var countString = failures.IsNotEmpty() ? $"[{failures.Count}/{testResults.Count()}]" : $"All {testResults.Count()}";

            var againstString = Actual.Select(it => CachedActual.Prettify(prettySettings))
                                      .Select(it => it.Truncate(prettySettings.LineLengthLimit))
                                      .Select(it => $" against [{it}]")
                                      .OrElse("");

            var summary = failures.IsNotEmpty()
                              ? $"💔 {countString} assertions{againstString} failed:"
                              : $"🎊 {countString} assertions{againstString} passed!";

            return new List<string>()
                   .Append(summary)
                   .Concat(testResults.SelectMany(it => it.FormatAssertable(Indent + 1)));
        }


        private string FormatMultipleAssertionMessage(IEnumerable<IAssertable> failures) {
            return new List<string>()
                   .Concat(FormatHeading())
                   .Concat(FormatShortCircuitException())
                   .Concat(FormatFailures(failures))
                   .ToStringLines()
                   .Indent(Indent)
                   .JoinLines();
        }

        /// <summary>
        /// Returns either the result of <see cref="Heading"/> or an empty <see cref="IEnumerable{T}"/> of strings.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> FormatHeading() {
            return (Heading?.Invoke()).WrapInEnumerable();
        }

        private Optional<string> FormatShortCircuitException() {
            return ShortCircuitException.Select(it => $"Something caused this {GetType().Name} to be unable to execute all of the assertions that it wanted to:\n{it.Message}\n{it.StackTrace}");
        }

        #endregion

        public void Invoke() {
            var results = TestEverything().ToList();
            if (results.Any(it => it.Failed)) {
                OnFailure(FormatMultipleAssertionMessage(results));
            }
            else {
                OnSuccess(FormatMultipleAssertionMessage(results));
            }
        }
    }
}