using FowlFever.BSharp;

namespace FowlFever.Clerical;

public class SimpleSaveSlot<TData> : ISaveSlot {
    public PathPart             Nickname              { get; }
    public ValueArray<PathPart> SaveFolder            { get; }
    public ValueArray<PathPart> RelativePath          { get; }
    public int                  SaveFileCount         { get; }
    public string               SaveFileSearchPattern { get; }
    public SaveManagerSettings  Settings              { get; }

    public SimpleSaveSlot(
        ValueArray<PathPart> saveFolder,
        PathPart             nickname,
        SaveManagerSettings? settings = default
    ) {
        SaveFolder = saveFolder;
        Nickname   = nickname;
        Settings   = settings ?? new SaveManagerSettings();
    }
}