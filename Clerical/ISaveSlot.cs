using FowlFever.BSharp;

using JetBrains.Annotations;

namespace FowlFever.Clerical;

/// <summary>
/// <b>Represents</b>:
/// <br/>
/// <b>🕹 To a user:</b> A choice of what to load.
/// <br/>
/// <b>🤖 To the code:</b> A logically grouped, isolated set of...who knows
/// </summary>
public interface ISaveSlot {
    public PathPart             Nickname   { get; }
    public ValueArray<PathPart> SaveFolder { get; }

    public ValueArray<PathPart> RelativePath { get; }

    [NonNegativeValue] public int SaveFileCount { get; }

    public string SaveFileSearchPattern { get; }

    public SaveManagerSettings Settings { get; }
}