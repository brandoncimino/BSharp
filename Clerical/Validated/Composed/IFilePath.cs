using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;

namespace FowlFever.Clerical.Validated.Composed;

public interface IFilePath : IFileSystemPath, IFileName {
    public DirectoryPath Directory { get; }
    /// <summary>
    /// The <see cref="IFileName"/> (<b>including <see cref="Atomic.IFileExtension"/>s!</b>) of this <see cref="IFilePath"/>.
    /// </summary>
    /// <remarks>
    /// The only difference between <see cref="FileName"/> and <see cref="IFileName.ToFileName"/> is that <see cref="FileName"/> is a property, which allows
    /// it to be modified by <c>with</c> expressions.
    /// </remarks>
    public FileName FileName { get; }
    public DirectorySeparator Separator { get; }

    public new static ReadOnlySpan<char> Ratify(ReadOnlySpan<char> filePath) {
        Must.Have(filePath[^1].IsDirectorySeparator(), false, $"cannot end in a directory separator (that would imply a {nameof(DirectoryPath)})");
        return filePath;
    }
}