using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using FowlFever.BSharp.Collections;

using Implementors;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// A lazily-evaluated, "buildable" version of <see cref="Spectre.Console.Text"/> / <see cref="Spectre.Console.Markup"/> / <see cref="Spectre.Console.Paragraph"/>.
/// </summary>
public record Epitaph() : IHasList<IStylized>, IHasRenderable {
    IList<IStylized> IHasList<IStylized>.AsList { get; } = ImmutableList.CreateBuilder<IStylized>();

    public Epitaph(string? content, Style? style = default) : this() {
        Add(content, style);
    }

    public string ToMarkup() {
        return string.Concat(this.Select(it => it.ToMarkup()));
    }

    public IRenderable GetRenderable() {
        return new Spectre.Console.Markup(ToMarkup());
    }

    public Epitaph Add(string? content, Style? style = default) {
        if (content.IsNotEmpty()) {
            this.AddNonNull<Epitaph, IStylized>(new Stylized { Content = content.Wrap(), Style = style });
        }

        return this;
    }

    public Epitaph Add(IHas<string?>? content, Style? style = default) => Add(content?.Value, style);

    public Epitaph Add(IStylized? content, params IStylized?[]? more) => this.AddNonNull(content)
                                                                             .AddNonNull(more);

    public Epitaph Add(IEnumerable<IStylized> contents) => this.AddNonNull(contents);

    public static implicit operator Epitaph(string? str) => new(str);
}

public interface IStylized {
    string Content { get; }
    Style? Style   { get; }

    public string ToMarkup() {
        return Style == null ? Content.EscapeMarkup() : $"[{Style.ToMarkup()}]{Content.EscapeMarkup()}[/]";
    }
}

public record Stylized(string? Content = default, Style? Style = default) : IStylized {
    private readonly string? _content = Content;
    public string Content {
        get => _content ?? "";
        init => _content = value;
    }
    public Style? Style { get; init; }
}