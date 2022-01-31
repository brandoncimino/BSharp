using System.ComponentModel;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Collections;

namespace FowlFever.Clerical;

/// <summary>
/// Represents a <see cref="FileSystemInfo"/>-safe name.
///
/// TODO: Add validation when things like <see cref="BaseName"/> or <see cref="Extensions"/> are set
/// TODO: Always strip periods from <see cref="Extensions"/>
/// </summary>
public record FileName {
    public string   BaseName   { get; init; }
    public string[] Extensions { get; init; } = { };
    public string   NameWithExtensions   => Extensions.Prepend(BaseName).JoinString(".");

    public FileName(string fileName) {
        BaseName   = BPath.GetFileNameWithoutExtensions(fileName);
        Extensions = BPath.GetExtensions(fileName);
    }

    public static FileName Random() {
        return new FileName(Path.GetRandomFileName());
    }
}