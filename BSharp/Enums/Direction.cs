namespace FowlFever.BSharp.Enums;

/// <summary>
/// Enumerates the <a href="https://en.wikipedia.org/wiki/Body_relative_direction">body-relative directions</a>, which also correspond to the faces of a cube.
/// </summary>
public enum Direction {
    #region Egocentric

    Forward,
    Backward,
    Left,
    Right,
    Up,
    Down,

    #endregion

    #region Natural

    Dorsal  = Backward,
    Ventral = Forward,

    #endregion

    #region Nautical

    Port      = Left,
    Starboard = Right,
    // On a boat the adjective for the bow is "fore", which would be "forward" 
    Aft = Backward,

    #endregion
}