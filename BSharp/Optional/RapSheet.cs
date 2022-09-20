using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Settings;

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

    public OneLine FailIcon { get; init; } = "💔".OneLine();
    public OneLine PassIcon { get; init; } = "🎊".OneLine();

    public  Func<RapSheet, OneLine>? HeadlineFormatter  { get; set; }
    public  Func<object?, string>?   PlaintiffFormatter { get; set; }
    public  Func<IFailable, string>? FailableFormatter  { get; set; }
    private OneLine                  Icon               => Convictions.Any() ? FailIcon : PassIcon;

    private OneLine FormatHeadline_Default() {
        string CountString() => Convictions.IsNotEmpty()
                                    ? $"[{Convictions.Count()}/{Charges.Count()}]"
                                    : $"All {Charges.Count()}";

        Optional<string> PlaintiffString() => Plaintiff.Select(it => PlaintiffFormatter?.Invoke(it) ?? FormatPlaintiff_Default(it));

        string AgainstString() => PlaintiffString().Select(it => $"against `{PlaintiffString()}` ").OrElse("");

        string VerdictString() => Convictions.IsNotEmpty() ? "stuck" : "were dropped";

        return $"{Icon} {CountString()} charges {AgainstString()}{VerdictString()}!".OneLine();
    }

    private static string FormatPlaintiff_Default(object? plaintiff) => plaintiff switch {
        IEnumerable e => e.Cast<object>().JoinString(", ", "[", "]"),
        null          => "⛔",
        _             => plaintiff.ToString()!,
    };

    private static string FormatFailable_Default(IFailable failable) => $"{failable}";

    /// <summary>
    /// The one-line summary of this <see cref="RapSheet"/>
    /// </summary>
    /// <returns></returns>
    public OneLine GetHeadline() => HeadlineFormatter?.Invoke(this) ?? FormatHeadline_Default();

    public override string ToString() => GetHeadline();

    private string FormatFailable(IFailable failable) => FailableFormatter?.Invoke(failable) ?? FormatFailable_Default(failable);

    public string Prettify(PrettificationSettings? settings = default) {
        var lines = new List<string> { GetHeadline() };
        lines.AddRange(Charges.Select(FormatFailable).Indent());
        return lines.JoinLines();
    }

    #endregion

    public Exception?        Excuse      => Exceptional.Aggregate(Convictions.Select(it => it.Excuse));
    public bool              Failed      => Convictions.Any();
    public Supplied<string?> Description => GetHeadline().ToString();

    public IRenderable GetRenderable() {
        var tree = new Tree(GetHeadlineRenderable());
        foreach (var charge in Charges) {
            tree.AddNode(charge.GetRenderable());
        }

        return tree;
    }
}