using System.ComponentModel;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Collections;

namespace FowlFever.Clerical;

/// <summary>
/// Represents a <see cref="FileSystemInfo"/>-safe name.
/// </summary>
public record FileName {
    public string   BaseName   { get; init; }
    public string[] Extensions { get; init; } = { };
    public string   FullName   => Extensions.Prepend(BaseName).JoinString(".");

    public FileName(string fileName) {
        BaseName   = BPath.GetFileNameWithoutExtensions(fileName);
        Extensions = BPath.GetExtensions(fileName);
    }
}