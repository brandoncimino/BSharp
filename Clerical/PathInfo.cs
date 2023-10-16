using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Primitives;

namespace FowlFever.Clerical;

public readonly struct PathInfo {
    [MaybeNull] private readonly string _source;
    private readonly             int    _baseNameLength;
    private readonly             int    _extensionLength;

    public FileExtension Extension {
        get {
            if (_source is null || _extensionLength == 0) {
                return default;
            }

            var extStartsAt = _source.Length - _extensionLength;
            return FileExtension.Parser.CreateUnsafe(new StringSegment(_source, extStartsAt, _source.Length - extStartsAt));
        }
    }

    public PathPart BaseName {
        get {
            if (_source is null || _baseNameLength == 0) {
                return default;
            }

            var fileNameStartsAt = _source.Length - _extensionLength - _baseNameLength;
            return PathPart.CreateUnsafe(new StringSegment(_source, fileNameStartsAt, _baseNameLength));
        }
    }

    public FileName FileName => new(BaseName, Extension);

    private PathInfo(
        string source,
        int    baseNameLength,
        int    extensionLength
    ) {
        _source          = source;
        _baseNameLength  = baseNameLength;
        _extensionLength = extensionLength;
    }

    public static PathInfo Parse(string path) {
        var span          = path.AsSpan();
        var lastSeparator = span.LastIndexOfAny("\\/.");
        return lastSeparator switch {
            < 0 => default,
            '.' => WithNameAndExtension(path, span, lastSeparator),
            _   => new PathInfo(path, span.Length - lastSeparator, 0)
        };
    }

    private static PathInfo WithNameAndExtension(string path, ReadOnlySpan<char> span, int lastSeparator) {
        var extLength        = span.Length - lastSeparator;
        var withoutExtension = span[lastSeparator..];
        var lastSlash        = withoutExtension.LastIndexOfAny("\\/");

        var nameLength = lastSlash switch {
            < 0 => withoutExtension.Length,
            _   => withoutExtension.Length - lastSlash
        };

        return new PathInfo(path, nameLength, extLength);
    }
}