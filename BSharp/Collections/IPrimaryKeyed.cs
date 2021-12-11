using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections {
    [PublicAPI]
    public interface IPrimaryKeyed<out T> {
        T PrimaryKey { get; }
    }
}