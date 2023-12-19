using System.Diagnostics;
using System.Numerics;

using FowlFever.Clerical;

using JetBrains.Annotations;

namespace Clerical.Tests;

internal static partial class TestHelpers {
    [StackTraceHidden]
    public static class Parse {
        public sealed record ParseResult<T>(
            string                            Input,
            Result<(bool success, T? parsed)> TryResult,
            Result<T>                         HardResult,
            ExcludeFromEquality<string>?      Description = null
        ) {
            public string TryParseDescription() => $"{Description}.TryParse(\"{Input}\")";
            public string ParseDescription()    => $"{Description}.Parse(\"{Input}\")";

            public          bool Equals(ParseResult<T>? other) => throw new NotSupportedException();
            public override int  GetHashCode()                 => throw new NotSupportedException();
        }

        [MustUseReturnValue]
        public static ParseResult<T> Stylish<T, P>(
            string         input,
            ClericalStyles styles
        )
            where T : IClericalParsable<T>, IEquatable<T>, IEqualityOperators<T, T, bool>
            where P : IClericalParser<T, ClericalStyles, string?> {
            return new ParseResult<T>(
                input,
                ResultOf(() => (P.TryParse_Internal(input, styles, out var result) is null, result))!,
                ResultOf(() => P.Parse_Internal(input, styles)),
                $"{typeof(P).TypeName()}({styles})"
            );
        }

        [MustUseReturnValue]
        public static ParseResult<T> StringParsable<T>(string input) where T : IParsable<T>, IEquatable<T>, IEqualityOperators<T, T, bool> {
            return new ParseResult<T>(
                input,
                ResultOf(() => (T.TryParse(input, null, out var tryResult), tryResult)),
                ResultOf(() => T.Parse(input, null)),
                typeof(IParsable<T>).TypeName()
            );
        }

        [MustUseReturnValue]
        public static ParseResult<T> SpanParsable<T>(string input) where T : ISpanParsable<T>, IEquatable<T>, IEqualityOperators<T, T, bool> {
            return new ParseResult<T>(
                input,
                ResultOf(() => (T.TryParse(input.AsSpan(), null, out var result), result)),
                ResultOf(() => T.Parse(input.AsSpan(), null)),
                typeof(ISpanParsable<T>).TypeName()
            );
        }

        [MustUseReturnValue]
        public static ParseResult<T> ClericalString<T>(string input) where T : IClericalParsable<T>, IEquatable<T>, IEqualityOperators<T, T, bool> {
            return new ParseResult<T>(
                input,
                ResultOf(() => (T.TryParse(input, out var result), result)),
                ResultOf(() => T.Parse(input)),
                $"{typeof(IClericalParsable<T>).TypeName()}(string)"
            );
        }

        [MustUseReturnValue]
        public static ParseResult<T> ClericalSpan<T>(string input) where T : IClericalParsable<T>, IEquatable<T>, IEqualityOperators<T, T, bool> {
            return new ParseResult<T>(
                input,
                ResultOf(() => (T.TryParse(input.AsSpan(), out var result), result)),
                ResultOf(() => T.Parse(input.AsSpan())),
                $"{typeof(IClericalParsable<T>).TypeName()}(ReadOnlySpan<char>)"
            );
        }

        public static void Assert_Parses<T>(string input, T expected) where T : IClericalParsable<T>, IEquatable<T>, IEqualityOperators<T, T, bool> {
            var results = new[] {
                StringParsable<T>(input),
                SpanParsable<T>(input),
                ClericalString<T>(input),
                ClericalSpan<T>(input)
            };

            Assert.Multiple(
                () => {
                    foreach (var parseResult in results) {
                        Assert_Success(parseResult, expected);
                    }
                }
            );
        }

        public static void Assert_NoParse<T>(string input) where T : IClericalParsable<T>, IEquatable<T>, IEqualityOperators<T, T, bool> {
            var results = new[] {
                StringParsable<T>(input),
                SpanParsable<T>(input),
                ClericalString<T>(input),
                ClericalSpan<T>(input)
            };

            Assert.Multiple(
                [StackTraceHidden]() => {
                    foreach (var parseResult in results) {
                        Assert_Failure(parseResult);
                    }
                }
            );
        }
    }

    public static void Assert_Success<T>(this Parse.ParseResult<T> actual, T expected) where T : IClericalParsable<T>, IEquatable<T>, IEqualityOperators<T, T, bool> {
        Assert.Multiple(
            [StackTraceHidden]() => {
                Assert.That(() => actual.HardResult,        Is.EqualTo(expected),         actual.ParseDescription());
                Assert.That(() => actual.TryResult.OrThrow, Is.EqualTo((true, expected)), actual.TryParseDescription());
            }
        );
    }

    public static void Assert_Failure<T>(this Parse.ParseResult<T> actual) {
        Assert.Multiple(
            [StackTraceHidden]() => {
                Assert.That(() => actual.HardResult.OrThrow,        Throws.InstanceOf<FormatException>(), actual.ParseDescription());
                Assert.That(() => actual.TryResult.OrThrow.success, Is.False,                             actual.TryParseDescription());
            }
        );
    }
}