using System.Collections;
using System.Text.RegularExpressions;

using FluentValidation;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;
using FowlFever.Conjugal.Affixing;

namespace FowlFever.Clerical.Validated;

public record FileExtensionGroup(FileNamePart[] ExtensionParts) {
    public FileExtensionGroup(
        IEnumerable<FileNamePart> extensionParts
    ) : this(
        extensionParts as FileNamePart[] ?? extensionParts.ToArray()
    ) { }

    public FileExtensionGroup(
        IEnumerable<string> extensionParts
    ) : this(
        extensionParts.Select(it => new FileNamePart(it))
    ) { }

    public override string ToString() {
        return ExtensionParts.JoinString(".").Prefix(".");
    }
}