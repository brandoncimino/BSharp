using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace FowlFever.Testing {
    [PublicAPI]
    public abstract class MultipleAsserter<TSelf, TActual> : IMultipleAsserter where TSelf : MultipleAsserter<TSelf, TActual>, new() {
        private const string HeadingIcon = "🧪";

        public PrettificationSettings? PrettificationSettings { get; protected set; }

        private Lazy<TActual>? _actual;

        /// <summary>
        /// The actual value being asserted against (if there is one)
        /// </summary>
        // private Lazy<TActual> Actual => _actual ?? throw new InvalidOperationException("Actual is empty!");
        internal OneTimeOnly<TActual> Actual { get; init; } = new OneTimeOnly<TActual>();

        public Func<string>? Heading { get; set; }

        public int Indent { get; protected set; }

        #region Subtest Types

        internal abstract record Subtest(
            Func<string>       Nickname,
            IResolveConstraint Constraint,
            Func<string>?      Message = default
        ) {
            protected internal abstract IAssertable Test(MultipleAsserter<TSelf, TActual> asserter);
        }

        internal record Action_AgainstAnything(
            Action              Action,
            IResolveConstraint? Constraint,
            Func<string>?       Nickname
        ) : Subtest(
            Nickname   ?? Assertable.GetNicknameSupplier(Action, default),
            Constraint ?? Throws.Nothing
        ) {
            protected internal override IAssertable Test(MultipleAsserter<TSelf, TActual> asserter) {
                return new Assertable(
                    () => asserter.ResolveAction(Action, Constraint, Message),
                    Nickname
                );
            }
        }

        internal record Action_AgainstActual(
            Action<TActual?>    Action,
            IResolveConstraint? Constraint,
            Func<string>?       Nickname
        ) : Subtest(
            Nickname   ?? Assertable.GetNicknameSupplier(Action, default),
            Constraint ?? Throws.Nothing
        ) {
            protected internal override IAssertable Test(MultipleAsserter<TSelf, TActual> asserter) {
                return new Assertable(
                    () => { asserter.ResolveAction(() => Action(asserter.Actual.Value), Constraint, Message); },
                    Nickname
                );
            }
        }

        internal record Constraint_AgainstActual(
            IResolveConstraint Constraint,
            Func<string>?      Nickname
        ) : Subtest(
            Nickname ?? Assertable.GetNicknameSupplier(default, Constraint),
            Constraint
        ) {
            protected internal override IAssertable Test(MultipleAsserter<TSelf, TActual> asserter) {
                return new Assertable(
                    () => asserter.ResolveActual(asserter.Actual.Value, Constraint, Nickname),
                    Nickname
                );
            }
        }

        internal record Constraint_AgainstAnything(
            object?            Target,
            IResolveConstraint Constraint,
            Func<string>?      Nickname
        ) : Subtest(
            Nickname ?? Assertable.GetNicknameSupplier(Target, Constraint),
            Constraint
        ) {
            protected internal override IAssertable Test(MultipleAsserter<TSelf, TActual> asserter) {
                return new Assertable(
                    () => asserter.ResolveActual(Target, Constraint, Message),
                    Nickname
                );
            }
        }

        internal record Constraint_AgainstDelegate(
            Func<object?>      Target,
            IResolveConstraint Constraint,
            Func<string>?      Nickname
        ) : Subtest(
            Nickname ?? Assertable.GetNicknameSupplier(Target, Constraint),
            Constraint
        ) {
            protected internal override IAssertable Test(MultipleAsserter<TSelf, TActual> asserter) {
                return new Assertable(
                    () => asserter.ResolveFunc(Target, Constraint, Message),
                    Nickname
                );
            }
        }

        internal record Constraint_AgainstTransformation(
            Delegate           Transformation,
            IResolveConstraint Constraint,
            Func<string>?      Nickname
        ) : Subtest(
            Nickname ?? Assertable.GetNicknameSupplier(Transformation, Constraint),
            Constraint
        ) {
            protected internal override IAssertable Test(MultipleAsserter<TSelf, TActual> asserter) {
                return new Assertable(
                    () => asserter.ResolveFunc(() => Transformation.DynamicInvoke(asserter.Actual.Value), Constraint, Message),
                    // () => asserter.ResolveActual(Transformation.DynamicInvoke(asserter.Actual.Value), Constraint, Message),
                    Nickname
                );
            }
        }

        #endregion

        internal IList<IMultipleAsserter> Asserters { get; } = new List<IMultipleAsserter>();

        internal IList<Subtest> Subtests { get; } = new List<Subtest>();

        protected abstract void OnFailure(string results);

        protected virtual void OnSuccess(string results) {
            Console.WriteLine(results);
        }

        public abstract void ResolveFunc<T>(
            Func<T>            actual,
            IResolveConstraint constraint,
            Func<string>?      message
        );

        public abstract void ResolveAction(
            Action             action,
            IResolveConstraint constraint,
            Func<string>?      message
        );

        public abstract void ResolveActual<T>(
            T                  actual,
            IResolveConstraint constraint,
            Func<string>?      message
        );

        private Optional<Exception> ShortCircuitException;

        #region Constructors

        protected MultipleAsserter() { }

        protected MultipleAsserter(TActual actual) : this(() => actual) { }

        protected MultipleAsserter(Func<TActual> actualValueDelegate) {
            Actual.Set(actualValueDelegate);
        }

        #endregion

        private TSelf Self => (this as TSelf)!;

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
        public TSelf Against(TActual actual) {
            Actual.Set(actual);
            return Self;
        }

        [MustUseReturnValue]
        public TSelf Against(Func<TActual> actualValueDelegate) {
            Actual.Set(actualValueDelegate);
            return Self;
        }

        #region "And" Constraints

        #region Actions_AgainstAnything

        private TSelf _Add_Action_AgainstAnything(Action action, IResolveConstraint? constraint, Func<string>? nickname) {
            Subtests.Add(new Action_AgainstAnything(action, constraint, nickname));
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And(Action action, Func<string>? nickname = default) => _Add_Action_AgainstAnything(action, default, nickname);

        [MustUseReturnValue]
        public TSelf And(Action action, IResolveConstraint constraint, Func<string>? nickname = default) => _Add_Action_AgainstAnything(action, constraint, nickname);

        [MustUseReturnValue]
        public TSelf And(Action action, string nickname) => _Add_Action_AgainstAnything(action, default, AsFunc(nickname));

        [MustUseReturnValue]
        public TSelf And(Action action, IResolveConstraint constraint, string nickname) => _Add_Action_AgainstAnything(action, constraint, AsFunc(nickname));

        [MustUseReturnValue]
        public TSelf And(IEnumerable<Action> actions) {
            actions.ForEach(it => _ = _Add_Action_AgainstAnything(it, default, default));
            return Self;
        }

        #endregion

        #region Actions_AgainstActual

        private TSelf _Add_Action_AgainstActual(
            Action<TActual?>    action,
            IResolveConstraint? constraint,
            Func<string>?       nickname
        ) {
            Subtests.Add(new Action_AgainstActual(action, constraint, nickname));
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And(
            Action<TActual?>   action,
            IResolveConstraint constraint,
            Func<string>?      nickname = default
        ) {
            _Add_Action_AgainstActual(action, constraint, nickname);
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And(
            Action<TActual?> action,
            Func<string>?    nickname = default
        ) =>
            _Add_Action_AgainstActual(
                action,
                default,
                nickname
            );

        [MustUseReturnValue]
        public TSelf Satisfies(
            Action<TActual?> action,
            Func<string>?    nickname = default
        ) => And(action, nickname);

        [MustUseReturnValue]
        public TSelf And(
            Action<TActual?> action,
            string?          nickname
        ) =>
            _Add_Action_AgainstActual(action, default, AsFunc(nickname));

        [MustUseReturnValue]
        public TSelf Satisfies(
            Action<TActual?>   action,
            IResolveConstraint constraint,
            Func<string>?      nickname = default
        ) =>
            And(
                action,
                constraint,
                nickname
            );

        #endregion

        #region Constraints_AgainstActual

        [MustUseReturnValue]
        public TSelf And(IResolveConstraint? constraint, Func<string>? nickname = default) {
            if (constraint != null) {
                Subtests.Add(new Constraint_AgainstActual(constraint, nickname));
            }

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
            Subtests.Add(new Constraint_AgainstAnything(target, constraint, nickname));
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

        private TSelf _Add_Constraint_AgainstDelegate(Func<object> supplier, IResolveConstraint constraint, Func<string>? nickname) {
            Subtests.Add(new Constraint_AgainstDelegate(supplier, constraint, nickname));
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And(Func<object> supplier, IResolveConstraint constraint, Func<string>? nickname = default) => _Add_Constraint_AgainstDelegate(supplier, constraint, nickname);

        [MustUseReturnValue]
        public TSelf And(Func<object> supplier, IResolveConstraint constraint, string? nickname) => _Add_Constraint_AgainstDelegate(supplier, constraint, AsFunc(nickname));

        #endregion

        #region Constraints_AgainstTransformation

        private TSelf _Add_Constraint_AgainstTransformation<TNew>(Func<TActual, TNew> actualTransformation, IResolveConstraint constraint, Func<string>? nickname) {
            Subtests.Add(new Constraint_AgainstTransformation(actualTransformation, constraint, nickname));
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And<TNew>(Func<TActual, TNew> tf, IResolveConstraint constraint, Func<string>? nickname = default) {
            return _Add_Constraint_AgainstTransformation(tf, constraint, nickname);
        }

        [MustUseReturnValue]
        public TSelf And<TNew>(Func<TActual, TNew> tf, IResolveConstraint constraint, string? nickname) {
            return _Add_Constraint_AgainstTransformation(tf, constraint, AsFunc(nickname));
        }

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

        [MustUseReturnValue]
        public TSelf AndAgainst<TNew>(
            Func<TActual?, TNew?>                  transformation,
            Func<Asserter<TNew?>, Asserter<TNew?>> ass
        ) {
            var a2 = Asserter.Against(this!, transformation);
            _Add_Asserter(a2);
            return Self;
        }

        private Asserter<TNew> _Get_ChildAsserter<TNew>(Func<TActual?, TNew> transformation) {
            TNew TfDelegate() => transformation(Actual.Get($"Could not create a child {typeof(Asserter<>).Name}"));
            return Asserter.Against(TfDelegate)
                           .WithPrettificationSettings(PrettificationSettings);
        }

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
            Heading = heading.IsNotBlank() ? () => heading! : default;
            return Self;
        }

        #endregion

        #region WithPrettificationSettings

        public TSelf WithPrettificationSettings(PrettificationSettings? settings) {
            PrettificationSettings = settings;
            return Self;
        }

        #endregion

        #region With Indent

        [MustUseReturnValue]
        protected TSelf WithIndent(int indent) {
            Indent = indent;
            return Self;
        }

        #endregion

        #endregion

        #region Executing Test Assertions

        internal IAssertable Test_Asserter(IMultipleAsserter asserter) {
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

        private Func<InvalidOperationException> ActualIsEmptyException(string message) {
            return () => new InvalidOperationException($"{message}: this {GetType().Prettify(PrettificationSettings)} doesn't have {nameof(Actual)} value!");
        }

        #endregion

        private IEnumerable<IAssertable> TestEverything() {
            return Subtests.Select(
                               it => {
                                   using (new TestExecutionContext.IsolatedContext()) {
                                       return it.Test(this);
                                   }
                               }
                           )
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

            var prettySettings = PrettificationSettings.Default with {
                PreferredLineStyle = LineStyle.Single, LineLengthLimit = 30, TypeLabelStyle = TypeNameStyle.Short
            };

            var countString = failures.IsNotEmpty() ? $"[{failures.Count}/{testResults.Count()}]" : $"All {testResults.Count()}";

            var againstString = Actual.ToOptional().Select(it => $" against [{it}]").OrElse("");

            var summary = failures.IsNotEmpty()
                              ? $"💔 {countString} assertions{againstString} failed:"
                              : $"🎊 {countString} assertions{againstString} passed!";

            return new List<string>()
                   .Append(summary)
                   .Concat(testResults.SelectMany(it => it.FormatAssertable(((IMultipleAsserter)this).Indent + 1)));
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