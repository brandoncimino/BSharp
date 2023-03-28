using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

namespace FowlFever.BSharp;

/// <summary>
/// Extension methods that operate on <see cref="Action"/>s and <see cref="Func{TResult}"/>s.
///
/// TODO: Rename to "Lambdas" and move to <see cref="Functionally"/> package.
/// </summary>
[PublicAPI]
public static partial class LambdaExtensions {
    #region Action with Tuple args => fn(a, b).Invoke(ab)

    public static void Invoke<A, B>(this                Action<A, B>                action, (A a, B b)                          args) => action.Invoke(args.a, args.b);
    public static void Invoke<A, B, C>(this             Action<A, B, C>             action, (A a, B b, C c)                     args) => action.Invoke(args.a, args.b, args.c);
    public static void Invoke<A, B, C, D>(this          Action<A, B, C, D>          action, (A a, B b, C c, D d)                args) => action.Invoke(args.a, args.b, args.c, args.d);
    public static void Invoke<A, B, C, D, E>(this       Action<A, B, C, D, E>       action, (A a, B b, C c, D d, E e)           args) => action.Invoke(args.a, args.b, args.c, args.d, args.e);
    public static void Invoke<A, B, C, D, E, F>(this    Action<A, B, C, D, E, F>    action, (A a, B b, C c, D d, E e, F f)      args) => action.Invoke(args.a, args.b, args.c, args.d, args.e, args.f);
    public static void Invoke<A, B, C, D, E, F, G>(this Action<A, B, C, D, E, F, G> action, (A a, B b, C c, D d, E e, F f, G g) args) => action.Invoke(args.a, args.b, args.c, args.d, args.e, args.f, args.g);

    #region Inverse (tuple params, invoke with individuals)

    public static void Invoke<A, B>(this    Action<(A, B)>    action, A a, B b)      => action.Invoke((a, b));
    public static void Invoke<A, B, C>(this Action<(A, B, C)> action, A a, B b, C c) => action.Invoke((a, b, c));

    public static void Invoke<A, B, C, D>(
        this Action<(A, B, C, D)> action,
        A                         a,
        B                         b,
        C                         c,
        D                         d
    ) => action.Invoke((a, b, c, d));

    public static void Invoke<A, B, C, D, E>(
        this Action<(A, B, C, D, E)> action,
        A                            a,
        B                            b,
        C                            c,
        D                            d,
        E                            e
    ) => action.Invoke((a, b, c, d, e));

    public static void Invoke<A, B, C, D, E, F>(
        this Action<(A, B, C, D, E, F)> action,
        A                               a,
        B                               b,
        C                               c,
        D                               d,
        E                               e,
        F                               f
    ) => action.Invoke((a, b, c, d, e, f));

    public static void Invoke<A, B, C, D, E, F, G>(
        this Action<(A, B, C, D, E, F, G)> action,
        A                                  a,
        B                                  b,
        C                                  c,
        D                                  d,
        E                                  e,
        F                                  f,
        G                                  g
    ) => action.Invoke((a, b, c, d, e, f, g));

    #endregion

    #endregion

    #region Func with Tuple args => fn(ab).Invoke(a, b);

    public static TResult Invoke<A, B, TResult>(this                Func<A, B, TResult>                func, (A a, B b)                          args) => func.Invoke(args.a, args.b);
    public static TResult Invoke<A, B, C, TResult>(this             Func<A, B, C, TResult>             func, (A a, B b, C c)                     args) => func.Invoke(args.a, args.b, args.c);
    public static TResult Invoke<A, B, C, D, TResult>(this          Func<A, B, C, D, TResult>          func, (A a, B b, C c, D d)                args) => func.Invoke(args.a, args.b, args.c, args.d);
    public static TResult Invoke<A, B, C, D, E, TResult>(this       Func<A, B, C, D, E, TResult>       func, (A a, B b, C c, D d, E e)           args) => func.Invoke(args.a, args.b, args.c, args.d, args.e);
    public static TResult Invoke<A, B, C, D, E, F, TResult>(this    Func<A, B, C, D, E, F, TResult>    func, (A a, B b, C c, D d, E e, F f)      args) => func.Invoke(args.a, args.b, args.c, args.d, args.e, args.f);
    public static TResult Invoke<A, B, C, D, E, F, G, TResult>(this Func<A, B, C, D, E, F, G, TResult> func, (A a, B b, C c, D d, E e, F f, G g) args) => func.Invoke(args.a, args.b, args.c, args.d, args.e, args.f, args.g);

    #endregion

    #region Inverse: fn(ab).Invoke(a, b)

    public static TResult Invoke<A, B, TResult>(this    Func<(A, B), TResult>    action, A a, B b)      => action.Invoke((a, b));
    public static TResult Invoke<A, B, C, TResult>(this Func<(A, B, C), TResult> action, A a, B b, C c) => action.Invoke((a, b, c));

    public static TResult Invoke<A, B, C, D, TResult>(
        this Func<
            (A, B, C, D),
            TResult
        > action,
        A a,
        B b,
        C c,
        D d
    ) => action.Invoke((a, b, c, d));

    public static TResult Invoke<A, B, C, D, E, TResult>(
        this Func<
            (A, B, C, D, E),
            TResult
        > action,
        A a,
        B b,
        C c,
        D d,
        E e
    ) => action.Invoke((a, b, c, d, e));

    public static TResult Invoke<A, B, C, D, E, F, TResult>(
        this Func<
            (A, B, C, D, E, F),
            TResult
        > action,
        A a,
        B b,
        C c,
        D d,
        E e,
        F f
    ) => action.Invoke((a, b, c, d, e, f));

    public static TResult Invoke<A, B, C, D, E, F, G, TResult>(
        this Func<
            (A, B, C, D, E, F, G),
            TResult
        > action,
        A a,
        B b,
        C c,
        D d,
        E e,
        F f,
        G g
    ) => action.Invoke((a, b, c, d, e, f, g));

    #endregion

    #region Laziness

    /// <summary>
    /// Creates a <see cref="Lazy{T}"/> that will execute an <see cref="Action"/>.
    /// </summary>
    /// <remarks>
    /// The output's <see cref="Lazy{T}.Value"/> is meaningless.
    /// It is used because <see cref="Void"/> cannot be used as a type parameter.
    /// <p/>
    /// Note that <see cref="byte"/>, <see cref="bool"/>, and <see cref="ValueTuple"/> all have a <see cref="Unsafe.SizeOf{T}"/> of 1, so there's no difference between them. 
    /// </remarks>
    /// <param name="oneTimeAction">an <see cref="Action"/> that will only be executed once</param>
    /// <returns>a new <see cref="Lazy{T}"/></returns>
    public static Lazy<byte> Lazily(this Action oneTimeAction) {
        return new Lazy<byte>(
            () => {
                oneTimeAction.Invoke();
                return default;
            }
        );
    }

    /// <inheritdoc cref="Lazily"/>
    public static Lazy<byte> Lazily<T>(this Action<T> oneTimeAction, T input) {
        return new Lazy<byte>(
            () => {
                oneTimeAction.Invoke(input);
                return default;
            }
        );
    }

    /// <summary>
    /// Shorthand for new <see cref="Lazy{T}"/>(<paramref name="valueFactory"/>).
    /// </summary>
    /// <param name="valueFactory">a <see cref="Func{TResult}"/> that should only be executed once</param>
    /// <typeparam name="T">the output type of <paramref name="valueFactory"/></typeparam>
    /// <returns>a new <see cref="Lazy{T}"/></returns>
    public static Lazy<T> Lazily<T>(this Func<T> valueFactory) {
        return new Lazy<T>(valueFactory);
    }

    /// <summary>
    /// Shorthand for new <see cref="Lazy{T}"/>(() => <paramref name="valueFactory"/>, <paramref name="input"/>)
    /// </summary>
    /// <param name="valueFactory">a <see cref="Func{TResult}"/> that should only be executed once</param>
    /// <param name="input">the input value to <paramref name="valueFactory"/></param>
    /// <typeparam name="TIn">the type <paramref name="input"/></typeparam>
    /// <typeparam name="TOut">the output type of <paramref name="valueFactory"/></typeparam>
    /// <returns>a new <see cref="Lazy{T}"/></returns>
    public static Lazy<TOut> Lazily<TIn, TOut>(this Func<TIn, TOut> valueFactory, TIn input) {
        return new Lazy<TOut>(() => valueFactory(input));
    }

    #endregion

    #region From "Assertion" (Action<T>)

    public static Func<T, T> ToCheckpoint<T>(this Action<T> action) {
        return it => {
            action(it);
            return it;
        };
    }

    public static Func<T, bool> ToPredicate<T>(this Action<T> action) {
        return it => {
            try {
                action(it);
                return true;
            }
            catch {
                return false;
            }
        };
    }

    #endregion

    #region From "Checkpoint" (Func<T,T>)

    public static Action<T> ToAssertion<T>(this Func<T, T> checkpoint) => obj => checkpoint.Invoke(obj);

    public static Func<T, bool> ToPredicate<T>(this Func<T, T> checkpoint) {
        return it => {
            try {
                checkpoint(it);
                return true;
            }
            catch {
                return false;
            }
        };
    }

    #endregion

    #region From "Predicate" (Func<T, bool>)

    public static Action<T> ToAssertion<T>(this Func<T, bool> predicate) {
        return it => {
            var result = predicate(it);
            if (!result) {
                throw new ArgumentException($"{predicate.Prettify()} returned {result}!");
            }
        };
    }

    public static Func<T, T> ToCheckpoint<T>(this Func<T, bool> predicate) {
        return it => {
            predicate.ToCheckpoint().Invoke(it);
            return it;
        };
    }

    #endregion

    #region Lambda expressions

    /// <summary>
    /// Attempts to extract the underling <see cref="MemberInfo"/> from an <see cref="Expression{TDelegate}"/>
    /// </summary>
    /// <param name="lambdaExpression">this <see cref="Expression{TDelegate}"/></param>
    /// <typeparam name="T">the type of the expression's lambda (aka <see cref="Delegate"/>)</typeparam>
    /// <returns>the latter-most <see cref="MemberInfo"/> referenced by the <see cref="Expression{TDelegate}"/>, if possible; otherwise, <c>null</c></returns>
    public static MemberInfo? AsMember<T>(this Expression<T> lambdaExpression)
        where T : Delegate {
        static MemberInfo? _GetMember(Expression exp) {
            return exp switch {
                MemberExpression member                                                                       => member.Member,
                MethodCallExpression call                                                                     => call.Method,
                UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } convert => _GetMember(convert.Operand),
                _                                                                                             => default
            };
        }

        return _GetMember(lambdaExpression.Body);
    }

    #endregion

    /// <summary>
    /// Applies <paramref name="andThen"/> to the result of <paramref name="func"/>.
    /// </summary>
    /// <param name="func">the original <see cref="Func{T,TResult}"/></param>
    /// <param name="andThen">the extra <see cref="Func{T,TResult}"/></param>
    /// <typeparam name="IN">the input type</typeparam>
    /// <typeparam name="MID">the middleman type</typeparam>
    /// <typeparam name="OUT">the output type</typeparam>
    /// <returns>a <see cref="Func{T, TResult}"/> that combines <paramref name="func"/> and <paramref name="andThen"/></returns>
    public static Func<IN, OUT> Chain<IN, MID, OUT>(this Func<IN, MID> func, Func<MID, OUT> andThen) {
        return it => andThen(func(it));
    }

    public static bool OrFalse<T>(this Func<T, bool>? predicate, T arg) => predicate?.Invoke(arg) ?? false;
    public static bool OrTrue<T>(this  Func<T, bool>? predicate, T arg) => predicate?.Invoke(arg) ?? true;
}