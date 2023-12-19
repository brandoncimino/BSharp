using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Clerical.Tests;

internal static partial class TestHelpers {
    public interface IResult {
        bool                HasValue { get; }
        IEnumerable<string> Describe();
    }

    /// <summary>
    /// An alternative to <see cref="StrongBox{T}"/> that is compared using <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record#value-equality">value equality</a>.
    /// </summary>
    public readonly record struct ValueBox<T>(T BoxedValue);

    public readonly record struct ExcludeFromEquality<T>(T Value) {
        public          bool   Equals(ExcludeFromEquality<T> other) => true;
        public override int    GetHashCode()                        => 0;
        public override string ToString()                           => Value?.ToString() ?? "";

        public static implicit operator ExcludeFromEquality<T>(T value) => new(value);
    }

    public static Result<T> Success<T>(T         value)     => new(new ValueBox<T>(value), null);
    public static Result<T> Failure<T>(Exception exception) => new(null, exception);

    [StackTraceHidden]
    public readonly record struct Result<T>(
        ValueBox<T>?                 ValueBox,
        Exception?                   Exception,
        ExcludeFromEquality<string?> Description = default
    ) : IEqualityOperators<Result<T>, Result<T>, bool>,
        IResult,
        IEquatable<T> {
        public ValueBox<T>? ValueBox  { get; } = RequireMutuallyExclusive(ValueBox, Exception).a;
        public Exception?   Exception { get; } = RequireMutuallyExclusive(ValueBox, Exception).b;

        [MemberNotNullWhen(true,  nameof(ValueBox))]
        [MemberNotNullWhen(false, nameof(Exception))]
        public bool HasValue => Exception is null;

        public T OrThrow => ValueBox is { } box ? box.BoxedValue : throw Exception!;

        public object? ValueOrException => ValueBox is { } box ? box.BoxedValue : Exception;

        public override string ToString() {
            if (HasValue) {
                return ValueBox.Value.BoxedValue?.ToString()
                       ?? $"<Actual [{typeof(T).TypeName()}] value was null!>";
            }

            var msg = $"💥 {Exception.GetType().Name}";

            if (string.IsNullOrWhiteSpace(Exception.Message) == false) {
                msg = string.Join('\n', msg, Exception.Message);
            }

            return msg;
        }

        public IEnumerable<string> Describe() {
            return [Description.Value, $" ↳ {ToString()}"];
        }

        public static implicit operator Result<T>(T         value)     => Success(value);
        public static implicit operator Result<T>(Exception exception) => Failure<T>(exception);

        public bool Equals(T? other) => HasValue && EqualityComparer<T>.Default.Equals(ValueBox.Value.BoxedValue, other);

        public bool Equals(
            Result<T>                     otherResult,
            IEqualityComparer<T>?         valueComparer,
            IEqualityComparer<Exception>? exceptionComparer
        ) {
            valueComparer     ??= EqualityComparer<T>.Default;
            exceptionComparer ??= EqualityComparer<Exception>.Default;

            return HasValue switch {
                true  => otherResult.HasValue          && valueComparer.Equals(OrThrow, otherResult.OrThrow),
                false => otherResult.HasValue == false && exceptionComparer.Equals(Exception, otherResult.Exception)
            };
        }
    }
}