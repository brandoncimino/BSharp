using System.Text.Json;

namespace FowlFever.Clerical;

/// <summary>
/// Joins together a <see cref="System.IO.FileInfo"/> with the <see cref="ISaveData"/> it contains.
/// </summary>
/// <typeparam name="TData">the type of the serialized <see cref="Data"/></typeparam>
public interface ISaveFile<out TData>
    where TData : ISaveData {
    public ISaveFolder SaveFolder { get; }

    /// <summary>
    /// The implementation-specific, "meaningful" data that this <see cref="ISaveFile{TData}"/> manages.
    /// </summary>
    public TData? Data { get; }

    /// <summary>
    /// The <see cref="DateTime"/> at which the <b><see cref="Data"/> refers to</b>.
    /// </summary>
    public DateTimeOffset TimeStamp { get; }

    public FileName FileName { get; }

    /// <summary>
    /// The <see cref="FileInfo"/> that this <see cref="ISaveFile{TData}"/> serializes to.
    /// </summary>
    /// <remarks>
    /// This property <b>must be immutable!</b>
    /// </remarks>
    public FileInfo File { get; }

    public ISaveFile<TData> Save();
}