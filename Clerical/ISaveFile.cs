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
    /// Data about the <b>act of saving itself</b>, such as <see cref="ISaveMetaData.LastSaveTime"/>.
    /// </summary>
    public ISaveMetaData MetaData { get; }

    /// <summary>
    /// The <see cref="DateTime"/> at which the <b><see cref="Data"/> refers to</b>.
    /// </summary>
    public DateTime TimeStamp { get; }

    /// <summary>
    /// The <see cref="FileInfo"/> that this <see cref="ISaveFile{TData}"/> serializes to.
    /// </summary>
    /// <remarks>
    /// This property <b>must be immutable!</b>
    /// </remarks>
    public FileInfo File { get; }

    /// <summary>
    /// Serializes <see cref="Data"/> to <see cref="File"/>.
    /// </summary>
    /// <param name="duplicateFileResolution">determines what we should do when a file already exists</param>
    /// <param name="jsonSettings">optional <see cref="JsonSerializerOptions"/></param>
    /// <returns></returns>
    public ISaveFile<TData> Save(DuplicateFileResolution duplicateFileResolution, JsonSerializerOptions? jsonSettings = default);

    /// <inheritdoc cref="Save(FowlFever.Clerical.DuplicateFileResolution,System.Text.Json.JsonSerializerOptions?)"/>
    /// <summary>
    /// Serializes <see cref="Data"/> to <see cref="File"/>.
    /// </summary>
    public ISaveFile<TData> Save(SaveManagerSettings? saveSettings = default);

    /// <summary>
    /// Deserializes the contents of <see cref="File"/> and stores it in <see cref="Data"/>.
    /// </summary>
    /// <param name="saveSettings"></param>
    /// <returns></returns>
    public ISaveFile<TData> Load(SaveManagerSettings? saveSettings = default);
}