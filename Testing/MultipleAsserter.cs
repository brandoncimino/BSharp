using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Settings;
using FowlFever.BSharp.Sugar;

using JetBrains.Annotations;

using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace FowlFever.Testing;

[PublicAPI]
[SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
public abstract class MultipleAsserter<TSelf, TActual> : IMultipleAsserter
    where TSelf : MultipleAsserter<TSelf, TActual>, new() {
    private const string HeadingIcon = "🧪";

    public PrettificationSettings? PrettificationSettings { get; protected set; }

    private Lazy<TActual>? _actual;

    /// <summary>
    /// The actual value being asserted against (if there is one)
    /// </summary>
    // private Lazy<TActual> Actual => _actual ?? throw new InvalidOperationException("Actual is empty!");
    internal OneTimeOnly<TActual> Actual { get; init; } = new OneTimeOnly<TActual>();

    public Supplied<string?>? Heading          { get; set; }
    public string?            ActualValueAlias { get; }

    public int Indent { get; protected set; }

    private Action<IEnumerable<IFailable>>? CustomActionOnFailure;

    #region Subtest Types

    internal abstract record Subtest(
        Supplied<string?>? ActualValueDescription,
        IResolveConstraint Constraint,
        string?            Expression,
        Supplied<string?>? Message = default
    ) {
        protected                   Supplied<string?> Description => GetNicknameSupplier(ActualValueDescription, Expression, Constraint);
        protected internal abstract IFailable         Test(MultipleAsserter<TSelf, TActual> asserter);

        private static Supplied<string?> GetNicknameSupplier(
            Supplied<string?>?      actualDescription,
            string?                 expression,
            IResolveConstraint?     constraint,
            PrettificationSettings? settings = default
        ) {
            static string GetNickname(Supplied<string?>? actualDescription, string? expression, IResolveConstraint? constraint, PrettificationSettings? settings) {
                var sb = new StringBuilder();
                if (actualDescription?.Value.IsNotBlank() == true) {
                    sb.Append($"[{actualDescription}]");
                }

                sb.AppendNonBlank(expression,                     " ");
                sb.AppendNonBlank(constraint?.Prettify(settings), " 🗜 ");
                return sb.ToString();
            }

            return Lazily.Get(() => GetNickname(actualDescription, expression, constraint, settings))!;
        }
    }

    internal record Action_AgainstAnything(
        Action              Action,
        IResolveConstraint? Constraint,
        Supplied<string?>?  Description,
        string?             Expression
    ) : Subtest(
        Description,
        Constraint ?? Throws.Nothing,
        Expression
    ) {
        protected internal override IFailable Test(MultipleAsserter<TSelf, TActual> asserter) {
            return new Assertable(
                () => asserter.ResolveAction(Action, Constraint, Message.OrDefault),
                Description
            );
        }
    }

    internal record Action_AgainstActual(
        Action<TActual?>    Action,
        IResolveConstraint? Constraint,
        Supplied<string?>?  Description,
        string?             Expression
    ) : Subtest(
        Description,
        Constraint ?? Throws.Nothing,
        Expression
    ) {
        protected internal override IFailable Test(MultipleAsserter<TSelf, TActual> asserter) {
            return new Assertable(
                () => { asserter.ResolveAction(() => Action(asserter.Actual.Value), Constraint, Message.OrDefault); },
                Description
            );
        }
    }

    internal record Constraint_AgainstActual(
        IResolveConstraint Constraint,
        Supplied<string?>? Description
    ) : Subtest(
        Description,
        Constraint,
        default
    ) {
        protected internal override IFailable Test(MultipleAsserter<TSelf, TActual> asserter) {
            return new Assertable(
                () => asserter.ResolveActual(asserter.Actual.Value, Constraint, Description.OrDefault),
                Description
            );
        }
    }

    internal record Constraint_AgainstAnything(
        object?            Target,
        IResolveConstraint Constraint,
        Supplied<string?>? Description,
        string?            Expression
    ) : Subtest(
        Description,
        Constraint,
        Expression
    ) {
        protected internal override IFailable Test(MultipleAsserter<TSelf, TActual> asserter) {
            return new Assertable(
                () => asserter.ResolveActual(Target, Constraint, Message.OrDefault),
                Description
            );
        }
    }

    internal record Constraint_AgainstDelegate(
        Func<object?>      Target,
        IResolveConstraint Constraint,
        Supplied<string>?  Description,
        string?            Expression
    ) : Subtest(
        Description,
        Constraint,
        Expression
    ) {
        protected internal override IFailable Test(MultipleAsserter<TSelf, TActual> asserter) {
            return new Assertable(
                () => asserter.ResolveFunc(Target, Constraint, Message.OrDefault),
                Description
            );
        }
    }

    internal record Constraint_AgainstTransformation(
        Delegate           Transformation,
        IResolveConstraint Constraint,
        Supplied<string?>? Description,
        string?            Expression
    ) : Subtest(
        Description,
        Constraint,
        Expression
    ) {
        protected internal override IFailable Test(MultipleAsserter<TSelf, TActual> asserter) {
            return new Assertable(
                () => asserter.ResolveFunc(() => Transformation.DynamicInvoke(asserter.Actual.Value), Constraint, Message.OrDefault),
                Description
            );
        }
    }

    #endregion

    internal IList<IMultipleAsserter> Asserters { get; } = new List<IMultipleAsserter>();

    internal IList<Subtest> Subtests { get; } = new List<Subtest>();

    #region Failing & Succeeding

    protected abstract void OnFailure(string results);

    /// <summary>
    /// ⚠ DO NOT call this method directly!
    ///
    /// Call <see cref="Succeed"/> instead!
    /// </summary>
    /// <param name="results"></param>
    protected virtual void OnSuccess(string results) {
        Console.WriteLine(results);
    }

    protected virtual string FormatResults(IEnumerable<IFailable> results) {
        var rapSheet = new RapSheet(results);
        return rapSheet.Prettify();
    }

    protected void Fail(IEnumerable<IFailable> results) {
        if (CustomActionOnFailure != null) {
            CustomActionOnFailure.Invoke(results);
        }
        else {
            OnFailure(FormatResults(results));
        }
    }

    protected void Succeed(string results) {
        OnSuccess(results);
    }

    #endregion

    internal abstract void ResolveFunc<T>(
        Func<T>            actual,
        IResolveConstraint constraint,
        Func<string?>?     message
    );

    protected abstract void ResolveAction(
        Action             action,
        IResolveConstraint constraint,
        Func<string?>?     message
    );

    protected abstract void ResolveActual<T>(
        T                  actual,
        IResolveConstraint constraint,
        Func<string?>?     message
    );

    private Optional<Exception> ShortCircuitException;

    #region Constructors

    protected MultipleAsserter() { }

    protected MultipleAsserter(TActual actual, string? actualValueAlias) {
        Actual.Set(actual);
        ActualValueAlias = actualValueAlias;
    }

    protected MultipleAsserter(Func<TActual> actualValueDelegate, string? actualValueAlias) {
        Actual.Set(actualValueDelegate);
        ActualValueAlias = actualValueAlias;
    }

    #endregion

    private TSelf Self => (this as TSelf)!;

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

    [MustUseReturnValue]
    public TSelf And(
        Action             action,
        Supplied<string?>? description = default,
        [CallerArgumentExpression("action")]
        string? expression = default
    ) =>
        _Add(new Action_AgainstAnything(action, default, description, default));

    [MustUseReturnValue]
    public TSelf And(Action action, IResolveConstraint constraint, Supplied<string?>? description = default, [CallerArgumentExpression("action")] string? expression = default)
        => _Add(new Action_AgainstAnything(action, constraint, description, expression));

    [MustUseReturnValue]
    [Obsolete("Ugliness")]
    public TSelf And(IEnumerable<Action> actions) {
        actions.ForEach(it => _ = _Add(new Action_AgainstAnything(it, default, default, default)));
        return Self;
    }

    #endregion

    #region Actions_AgainstActual

    [MustUseReturnValue]
    public TSelf And(
        Action<TActual?>   action,
        IResolveConstraint constraint,
        Supplied<string?>? description = default,
        [CallerArgumentExpression("action")]
        string? expression = default
    ) {
        return _Add(new Action_AgainstActual(action, constraint, description, expression));
    }

    [MustUseReturnValue]
    public TSelf And(
        Action<TActual?>   action,
        Supplied<string?>? description = default,
        [CallerArgumentExpression("action")]
        string? expression = default
    ) {
        return _Add(new Action_AgainstActual(action, default, description, expression));
    }

    [MustUseReturnValue]
    [Obsolete]
    public TSelf Satisfies(
        Action<TActual?>   action,
        IResolveConstraint constraint,
        Supplied<string?>? nickname = default
    ) =>
        And(
            action,
            constraint,
            nickname
        );

    #endregion

    #region Constraints_AgainstActual

    [MustUseReturnValue]
    public TSelf And(IResolveConstraint constraint, Supplied<string?>? description = default, [CallerArgumentExpression("constraint")] string? expression = default) {
        return _Add(new Constraint_AgainstActual(constraint, description ?? expression));
    }

    [Obsolete("ugliness")]
    [MustUseReturnValue]
    public TSelf And(IEnumerable<IResolveConstraint?>? constraints) {
        constraints?.ForEach(it => _ = And(it));
        return Self;
    }

    #endregion

    #region Constraints_AgainstAnything

    [MustUseReturnValue]
    public TSelf And(
        object?            target,
        IResolveConstraint constraint,
        Supplied<string?>? description = default,
        [CallerArgumentExpression("target")]
        string? expression = default
    ) {
        return _Add(new Constraint_AgainstAnything(target, constraint, description, expression));
    }

    #endregion

    #region Constraints_AgainstDelegate

    private TSelf _Add(Subtest subtest) {
        Subtests.Add(subtest);
        return Self;
    }

    [MustUseReturnValue]
    public TSelf And(
        Func<object>       supplier,
        IResolveConstraint constraint,
        Supplied<string>?  description = default,
        [CallerArgumentExpression("supplier")]
        string? expression = default
    ) =>
        _Add(
            new Constraint_AgainstDelegate(
                supplier,
                constraint,
                description,
                expression
            )
        );

    #endregion

    #region Constraints_AgainstTransformation

    [MustUseReturnValue]
    public TSelf And<TNew>(
        Func<TActual, TNew> tf,
        IResolveConstraint  constraint,
        Supplied<string?>?  description = default,
        [CallerArgumentExpression("tf")]
        string? expression = default
    ) {
        return _Add(
            new Constraint_AgainstTransformation(
                tf,
                constraint,
                description,
                expression
            )
        );
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
        var a2 = Asserter.Against(this, transformation).WithPrettificationSettings(PrettificationSettings);
        return _Add_Asserter(ass(a2));
    }

    #endregion

    #endregion

    #region WithHeading

    [MustUseReturnValue]
    public TSelf WithHeading(Supplied<string?>? headingSupplier) {
        Heading = headingSupplier;
        return Self;
    }

    #endregion

    #region WithPrettificationSettings

    [MustUseReturnValue]
    public TSelf WithPrettificationSettings(PrettificationSettings? settings) {
        PrettificationSettings = settings;
        return Self;
    }

    #endregion

    #region WithIndent

    [MustUseReturnValue]
    protected TSelf WithIndent(int indent) {
        Indent = indent;
        return Self;
    }

    #endregion

    #region WithActionOnFailure

    [MustUseReturnValue]
    public TSelf WithActionOnFailure(Action<IEnumerable<IFailable>> actionOnFailure) {
        CustomActionOnFailure = actionOnFailure;
        return Self;
    }

    [MustUseReturnValue]
    public TSelf WithForgiveness(string excuse) {
        return WithActionOnFailure(str => throw new InconclusiveException($"Failure was forgiven: {excuse}\n{str}"));
    }

    #endregion

    #endregion

    #region Executing Test Assertions

    internal IFailable Test_Asserter(IMultipleAsserter asserter) {
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

    private IEnumerable<IFailable> TestEverything() {
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

    private IEnumerable<string> FormatFailures([InstantHandle] IEnumerable<IFailable> testResults) {
        testResults = testResults.ToList();
        return new RapSheet(Actual.ToOptional().Cast<object?>(), testResults).Prettify().SplitLines();
    }

    private string FormatMultipleAssertionMessage(IEnumerable<IFailable> failures) {
        return new List<string>()
               .Concat(FormatHeading())
               .Concat(FormatShortCircuitException())
               .Concat(FormatFailures(failures))
               .ToStringLines()
               .Indent(Indent)
               .JoinLines();
    }

    /// <returns>either the result of <see cref="Heading"/> or an empty <see cref="IEnumerable{T}"/> of strings</returns>
    private IEnumerable<string> FormatHeading() {
        return Heading
               .OrDefault()
               .IfNotBlank(it => $"{HeadingIcon} {it}")
               .WrapInEnumerable();
    }

    private Optional<string> FormatShortCircuitException() {
        return ShortCircuitException.Select(it => $"Something caused this {GetType().Name} to be unable to execute all of the assertions that it wanted to:\n{it.Message}\n{it.StackTrace}");
    }

    #endregion

    public void Invoke() {
        var results = TestEverything().ToList();
        if (results.Any(it => it.Failed)) {
            Fail(results);
        }
        else {
            Succeed(FormatMultipleAssertionMessage(results));
        }
    }
}