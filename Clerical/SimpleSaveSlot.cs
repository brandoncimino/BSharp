namespace FowlFever.Clerical;

public class SimpleSaveSlot<TData> : ISaveSlot<TData>
    where TData : ISaveData {
    public FileName                      Nickname              { get; }
    public ISaveFolder                   SaveFolder            { get; }
    public string[]                      RelativePath          { get; }
    public int                           SaveFileCount         { get; }
    public string                        SaveFileSearchPattern { get; }
    public SaveManagerSettings           Settings              { get; }

    public SimpleSaveSlot(
        ISaveFolder          saveFolder,
        FileName             nickname,
        SaveManagerSettings? settings = default
    ) {
        SaveFolder   = saveFolder;
        Nickname     = nickname;
        Settings     = settings ?? new SaveManagerSettings();
    }
}