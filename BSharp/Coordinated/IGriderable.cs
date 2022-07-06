using System.Collections.Generic;

namespace FowlFever.BSharp.Coordinated;

public interface IGriderable<T> : IEnumerable<KeyValuePair<Coord, T>> { }