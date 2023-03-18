namespace FowlFever.BSharp.Enums;

/// <summary>
/// Enumerates the standard <a href="https://en.wikipedia.org/wiki/Two-dimensional_space">two-dimensional axes</a> (<see cref="X"/> and <see cref="Y"/>).
/// </summary>
public enum Axis2 {
    #region Canonical

    X = 0,
    Y = 1,

    #endregion

    #region Aliases

    #region Astronomical

    Horizontal = X,
    Vertical   = Y,

    #endregion

    #region Geophysical

    /// <summary>
    /// See <a href="https://en.wikipedia.org/wiki/Vertical_and_horizontal#The_plumb_line_and_spirit_level">plumb-line and spirit-level</a>
    /// </summary>
    Spirit = X,
    /// <summary>
    /// See <a href="https://en.wikipedia.org/wiki/Vertical_and_horizontal#The_plumb_line_and_spirit_level">plumb-line and spirit-level</a>
    /// </summary>
    Plumb = Y,

    #endregion

    #endregion
}