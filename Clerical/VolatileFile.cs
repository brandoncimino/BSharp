using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Optional;

using Microsoft.Diagnostics.Tracing.Parsers.FrameworkEventSource;

namespace FowlFever.Clerical;

/// <summary>
/// A similar pattern to <see cref="JsonFileData{T}"/> that doesn't use <see cref="FluentValidation"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class VolatileFile<T> : IFileData<T> {
    public FileInfo File { get; }

    /// <summary>
    /// Gets the <b>currently loaded</b> <typeparamref name="T"/>, which may or may not reflect the actual <see cref="File"/> content.
    /// </summary>
    /// <seealso cref="GetData"/>
    public T? Data { get; private set; }

    public bool CanDeserialize => this is { Data: null, IsPersisted: true };
    public bool CanSerialize   => this is { Data: not null, IsPersisted: false };
    public void Deserialize()  => LoadDataFromFile(default);

    public void Serialize() => SaveDataToFile(default);

    public T GetData() {
        Sync();
        return Must.NotBeNull(Data, nameof(Data), nameof(GetData));
    }

    public VolatileFile(T? data, FileInfo fileInfo) {
        File = fileInfo;
        Data     = data;
    }

    public static VolatileFile<T> FromExistingFile(FileInfo fileInfo) {
        Must.NotBeEmpty(fileInfo, nameof(fileInfo), nameof(FromExistingFile));
        return new VolatileFile<T>(default, fileInfo);
    }

    public static VolatileFile<T> FromDataObject(T data, FileInfo fileInfo) {
        Must.NotBeNull(data, nameof(data), nameof(FromDataObject));
        Must.BeEmptyOrMissing(fileInfo, nameof(fileInfo), nameof(FromDataObject));
        return new VolatileFile<T>(data, fileInfo);
    }

    public  bool IsPersisted => File is { Exists: true, Length: > 0 };

    /// <summary>
    /// Either <see cref="LoadDataFromFile"/> <b>or</b> <see cref="SaveDataToFile"/> - whichever is necessary.
    /// </summary>
    /// <remarks>
    /// This method is idempotent, meaning that it can be called repeatedly with no consequences: subsequent calls to <see cref="Sync"/> will return without doing anything.
    /// </remarks>
    /// <param name="JsonSerializerOptions">optional <see cref="JsonSerializerOptions"/></param>
    /// <exception cref="ArgumentException"></exception>
    public void Sync() {
        Action<JsonSerializerOptions?>? syncAction = this switch {
            { Data: null, IsPersisted: true }      => LoadDataFromFile,
            { Data: null, IsPersisted: false }     => throw new ArgumentException(),
            { Data: not null, IsPersisted: true }  => default,
            { Data: not null, IsPersisted: false } => SaveDataToFile,
        };

        syncAction?.Invoke(new JsonSerializerOptions());
    }

    private void LoadDataFromFile(JsonSerializerOptions? options) {
        Must.NotBeEmpty(File, nameof(File), nameof(LoadDataFromFile));
        Must.BeNull(Data, nameof(Data), nameof(LoadDataFromFile));

        var dataFromFile = File.Deserialize<T>(options);
        Data = dataFromFile;
    }

    private void SaveDataToFile(JsonSerializerOptions? options) {
        Must.NotExist(File, nameof(File), nameof(SaveDataToFile));
        Must.NotBeNull(Data, nameof(Data), nameof(SaveDataToFile));

        File.Serialize(Data, BSharp.Clerical.DuplicateFileResolution.Error, options);
    }
}