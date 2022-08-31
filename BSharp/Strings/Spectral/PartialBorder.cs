using FowlFever.BSharp.Enums;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// Selectively includes some of the <see cref="BoxBorderPart"/>s of a <see cref="BoxBorder"/>.
/// </summary>
public class PartialBorder : BoxBorder {
    public           bool      Top    { get; init; }
    public           bool      Left   { get; init; }
    public           bool      Right  { get; init; }
    public           bool      Bottom { get; init; }
    private readonly BoxBorder _border;

    public PartialBorder(BoxBorder border) {
        _border = border;
    }

    private bool IsPartIncluded(BoxBorderPart part) {
        return part switch {
            BoxBorderPart.Bottom      => Bottom,
            BoxBorderPart.Top         => Top,
            BoxBorderPart.Left        => Left,
            BoxBorderPart.Right       => Right,
            BoxBorderPart.TopLeft     => Top    || Left,
            BoxBorderPart.TopRight    => Top    || Right,
            BoxBorderPart.BottomLeft  => Bottom || Left,
            BoxBorderPart.BottomRight => Bottom || Right,
            _                         => throw BEnum.UnhandledSwitch(part),
        };
    }

    private BoxBorderPart? ResolvePart(BoxBorderPart part) {
        if (IsPartIncluded(part) == false) {
            return null;
        }

        if (part.IsSide()) {
            return part;
        }

        var (vert, hor) = part.GetSidesFromCorner();
        return (IsPartIncluded(vert), IsPartIncluded(hor)) switch {
            (true, true)   => part,
            (true, false)  => vert,
            (false, true)  => hor,
            (false, false) => null,
        };
    }

    public override string GetPart(BoxBorderPart part) {
        return ResolvePart(part)?.SendTo(_border.GetPart) ?? " ";
    }
}

public static class BoxBorderPartExtensions {
    public static bool IsCorner(this BoxBorderPart part) => part is BoxBorderPart.TopRight or BoxBorderPart.TopLeft or BoxBorderPart.BottomLeft or BoxBorderPart.BottomRight;
    public static bool IsSide(this   BoxBorderPart part) => !part.IsCorner();

    public static (BoxBorderPart vertical, BoxBorderPart horizontal) GetSidesFromCorner(this BoxBorderPart corner) {
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return corner switch {
            BoxBorderPart.TopLeft     => (BoxBorderPart.Top, BoxBorderPart.Left),
            BoxBorderPart.TopRight    => (BoxBorderPart.Top, BoxBorderPart.Right),
            BoxBorderPart.BottomLeft  => (BoxBorderPart.Bottom, BoxBorderPart.Left),
            BoxBorderPart.BottomRight => (BoxBorderPart.Bottom, BoxBorderPart.Right),
            _                         => throw BEnum.UnhandledSwitch(corner),
        };
    }
}