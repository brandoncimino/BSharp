namespace FowlFever.BSharp.Enums;

/// <summary>
/// Enumerates the standard <a href="https://en.wikipedia.org/wiki/Three-dimensional_space">three-dimensional axes</a> (<see cref="X"/>, <see cref="Y"/> and <see cref="Z"/>).
/// </summary>
/// <remarks>When working with <see cref="Axis3"/>, you cannot assume the real-world orientation of the axes. For example, in Unity, the vertical axis is <see cref="Y"/>, while in Unreal, it is <see cref="Z"/>.</remarks>
public enum Axis3 {
    #region Canonical

    X = 0, Y = 1, Z = 2,

    #endregion
}