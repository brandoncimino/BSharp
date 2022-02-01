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

    public static FileName From(string fileName) {
        return new FileName(fileName);
    }

    public static FileName From(FileSystemInfo fileSystemInfo) {
        return new FileName(fileSystemInfo.Name);
    }
}

public static class FileNameExtensions {
    public static FileName GetFileName(this FileSystemInfo fileSystemInfo) {
        return new FileName(fileSystemInfo.Name);
    }
}