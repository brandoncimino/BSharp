using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Collections {
    [PublicAPI]
    public interface IPrimaryKeyed<out T> {
        T PrimaryKey { get; }
    }
}