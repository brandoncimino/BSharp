using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Optional;

/// <summary>
/// Contains a set of <see cref="IFailable"/> results.
/// </summary>
[PublicAPI]
public class RapSheet : IEnumerable<IFailable>, IPrettifiable, IFailable {
    public enum Verdict { Passed, Failed, }

    private readonly ILookup<Verdict, IFailable> _charges;
    public           IEnumerable<IFailable>      Charges => _charges.SelectMany(it => it.AsEnumerable());

    public IEnumerable<IFailable> this[Verdict outcome] => _charges[outcome];

    public IEnumerable<IFailable> Convictions => this[Verdict.Failed];
    public IEnumerable<IFailable> Acquittals  => this[Verdict.Passed];
    public Optional<object?>      Plaintiff   { get; }

    public RapSheet(IEnumerable<IFailable> charges) {
        _charges = charges.ToLookup(
            it => it switch {
                { Failed: true }  => Verdict.Failed,
                { Failed: false } => Verdict.Passed,
            }
        );
    }

    public RapSheet(params IFailable[] charges) : this(charges.AsEnumerable()) { }

    public RapSheet(Optional<object?> plaintiff, IEnumerable<IFailable> charges) : this(charges) => Plaintiff = plaintiff;

    public RapSheet(Optional<object?> plaintiff, IFailable charge, params IFailable[] charges) : this(plaintiff, charges.AsEnumerable().Prepend(charge)) { }

    #region IEnumerable<> Implementation

    public IEnumerator<IFailable> GetEnumerator() => _charges.SelectMany(it => it).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region Formatting

    public string FailIcon { get; init; } = "💔";
    public string PassIcon { get; init; } = "🎊";

    public  Func<RapSheet, string>?  SummaryFormatter   { get; set; }
    public  Func<object?, string>?   PlaintiffFormatter { get; set; }
    public  Func<IFailable, string>? FailableFormatter  { get; set; }
    private string                   Icon               => Convictions.Any() ? FailIcon : PassIcon;

    private string FormatSummary_Default() {
        string CountString() => Convictions.IsNotEmpty()
                                    ? $"[{Convictions.Count()}/{Charges.Count()}]"
                                    : $"All {Charges.Count()}";

        Optional<string> PlaintiffString() => Plaintiff.Select(it => PlaintiffFormatter?.Invoke(it) ?? FormatPlaintiff_Default(it));

        string AgainstString() => PlaintiffString().Select(it => $"against `{PlaintiffString()}` ").OrElse("");

        string VerdictString() => Convictions.IsNotEmpty() ? "stuck" : "were dropped";

        return $"{Icon} {CountString()} charges {AgainstString()}{VerdictString()}!";
    }

    private static string FormatPlaintiff_Default(object? plaintiff) => plaintiff switch {
        IEnumerable e => e.Cast<object>().JoinString(", ", "[", "]"),
        null          => "⛔",
        _             => plaintiff.ToString()!,
    };

    private static string FormatFailable_Default(IFailable failable) => $"{failable}";

    private string GetSummary() => SummaryFormatter?.Invoke(this) ?? FormatSummary_Default();

    public override string ToString() => GetSummary();

    private string FormatFailable(IFailable failable) => FailableFormatter?.Invoke(failable) ?? FormatFailable_Default(failable);

    #endregion

    public string Prettify(PrettificationSettings? settings = default) {
        var lines = new List<string> { GetSummary() };
        lines.AddRange(Charges.Select(FormatFailable).Indent());
        return lines.JoinLines();
    }

    public Exception?                   Excuse                => ExceptionUtils.Aggregate(Convictions.Select(it => it.Excuse));
    public bool                         Failed                => Convictions.Any();
    IReadOnlyCollection<Type> IFailable.IgnoredExceptionTypes => throw new NotSupportedException(MethodBase.GetCurrentMethod()?.ToString());
    Exception IFailable.                IgnoredException      => throw new NotSupportedException(MethodBase.GetCurrentMethod()?.ToString());
    public Supplied<string>             Description           => GetSummary();
}