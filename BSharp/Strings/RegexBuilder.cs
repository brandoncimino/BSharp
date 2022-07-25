using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading;

using FowlFever.BSharp.Attributes;
using FowlFever.Implementors;

namespace FowlFever.BSharp.Strings;

[Experimental()]
public abstract record InitResettable<T>
    where T : notnull {
    private   T?   _value;
    protected T    InitValue => LazyInitializer.EnsureInitialized(ref _value, ref _isInitialized, ref SyncLock, Supply) ?? throw new ArgumentNullException(nameof(_value), "Lazy initializer returned null!");
    private   bool _isInitialized;
    protected bool IsInitialized {
        get => _isInitialized;
        init {
            if (value != false) {
                throw new ArgumentException($"You can only explicitly set {nameof(IsInitialized)} to false!");
            }

            _isInitialized = value;
        }
    }
    private object? SyncLock;

    [MemberNotNull(nameof(_value))] protected abstract T Supply();
}

/// <summary>
/// A base class to construct builder-style <c>record</c>s for <see cref="System.Text.RegularExpressions.Regex"/> patterns. 
/// </summary>
[Experimental()]
public abstract record RegexBuilder : InitResettable<Regex>, IHas<Regex> {
    private static readonly TimeSpan DefaultMatchTimeout = new Regex("").MatchTimeout;

    /// <summary>
    /// The <see cref="string"/> representation of the final <see cref="Regex"/>.
    /// </summary>
    public string Pattern => InitValue.ToString();

    private readonly TimeSpan _matchTimeout = DefaultMatchTimeout;
    /// <inheritdoc cref="System.Text.RegularExpressions.Regex.MatchTimeout"/>
    public TimeSpan MatchTimeout {
        get => _matchTimeout;
        init {
            _matchTimeout = value;
            IsInitialized = false;
        }
    }

    private readonly RegexOptions _options = RegexOptions.None;
    /// <inheritdoc cref="System.Text.RegularExpressions.Regex.Options"/>
    public RegexOptions Options {
        get => _options;
        init {
            _options      = value;
            IsInitialized = false;
        }
    }

    Regex IHas<Regex>.Value => Regex;

    /// <summary>
    /// The final <see cref="System.Text.RegularExpressions.Regex"/> for this <see cref="RegexBuilder"/>.
    /// </summary>
    public Regex Regex => InitValue;

    protected abstract string BuildPattern();

    protected sealed override Regex Supply() => new(BuildPattern(), Options, MatchTimeout);

    public sealed override string ToString() => Pattern;

    public static implicit operator Regex(RegexBuilder builder) => builder.Regex;
}