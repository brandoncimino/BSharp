namespace FowlFever.Clerical;

/// <summary>
/// Specifies what we should do when we try to create a <see cref="FileInfo"/> that already exists.
/// </summary>
public enum DuplicateFileResolution {
    Error = default,
    Overwrite,
    Backup,
}