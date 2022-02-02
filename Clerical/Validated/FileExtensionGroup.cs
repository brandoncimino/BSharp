using System.Collections;
using System.Text.RegularExpressions;

using FluentValidation;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;
using FowlFever.Conjugal.Affixing;

namespace FowlFever.Clerical.Validated;

public class FileExtensionGroup {
    private IEnumerable<FileNamePart> ExtensionParts;

    public FileExtensionGroup(IEnumerable<string> extensionParts) {

    }

    public FileExtensionGroup(IEnumerable<FileNamePart> extensionParts) {
        ExtensionParts = extensionParts;
    }

    public override string ToString() {
        return ExtensionParts.JoinString(".").Prefix(".");
    }
}