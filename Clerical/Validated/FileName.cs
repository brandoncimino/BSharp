using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Collections;

namespace FowlFever.Clerical.Validated;

/// <summary>
/// Represents a <see cref="FileSystemInfo"/>-safe name.
///
/// TODO: Add validation when things like <see cref="BaseName"/> or <see cref="Extensions"/> are set
/// TODO: Always strip periods from <see cref="Extensions"/>
/// </summary>
public class FileName : PathPart {
    public FileNamePart    BaseName   { get; }
    public FileExtension[] Extensions { get; }

    public FileName(string value) : this(BPath.GetFileNameWithoutExtensions(value), BPath.GetExtensions(value)) { }

    public FileName(string baseName, params string[] extensions) : base($"{baseName}{extensions.JoinString()}") {
        BaseName   = new FileNamePart(baseName);
        Extensions = extensions.Select(it => new FileExtension(it)).ToArray();
    }

    public static FileName Random() => Clerk.GetRandomFileName();
}