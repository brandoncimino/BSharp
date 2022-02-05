using System.Collections;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

using FluentValidation;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;
using FowlFever.Conjugal.Affixing;

namespace FowlFever.Clerical.Validated;

public class FileExtensionGroup {
    public IEnumerable<FileNamePart> ExtensionParts { get; }

    public FileExtensionGroup(IEnumerable<FileNamePart> extensionParts) {
        ExtensionParts = extensionParts;
    }

    public FileExtensionGroup(IEnumerable<string> extensionParts)
        : this(extensionParts.Select(it => new FileNamePart(it))) { }

    public override string ToString() {
        return ExtensionParts.JoinString(".").Prefix(".");
    }
}