using JetBrains.Annotations;

namespace FowlFever.Clerical;

/// <summary>
/// <b>Represents</b>:
/// <br/>
/// <b>🕹 To a user:</b> A choice of what to load.
/// <br/>
/// <b>🤖 To the code:</b> A logically grouped, isolated set of <see cref="ISaveFile{TData}"/>s.
/// </summary>
/// <typeparam name="TData">the <see cref="ISaveData"/> that this <see cref="ISaveSlot{TData}"/> manages</typeparam>
public interface ISaveSlot<TData> where TData : ISaveData {
    public FileName      Nickname   { get; }
    public ISaveFolder SaveFolder { get; }

    public string[] RelativePath { get; }

    [NonNegativeValue]
    public int SaveFileCount { get; }

    public string SaveFileSearchPattern { get; }

    public SaveManagerSettings Settings { get; }
}