using System.Text.Json;

using FowlFever.BSharp.Clerical;

namespace FowlFever.Clerical;

/// <summary>
/// A basic implementation of <see cref="ISaveFile{TData}"/>.
/// </summary>
/// <typeparam name="TData"></typeparam>
public class SaveFile2<TData> : ISaveFile<TData>
    where TData : ISaveData {
    public ISaveFolder    SaveFolder { get; }
    public TData?         Data       { get; }
    public DateTimeOffset TimeStamp  { get; }
    public FileName       FileName   { get; }
    public FileInfo       File       => SaveFolder.DirectoryInfo.GetChildFile(FileName.NameWithExtensions);

    public SaveFile2(ISaveFolder saveFolder, TData? data, FileName fileName, DateTimeOffset timeStamp) {
        SaveFolder = saveFolder;
        Data       = data;
        FileName   = fileName;
        TimeStamp  = timeStamp;
    }

    public ISaveFile<TData> Save() {
        File.SerializeCautiously(Data);
        return this;
    }
}