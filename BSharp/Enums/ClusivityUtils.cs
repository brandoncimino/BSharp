using JetBrains.Annotations;

namespace FowlFever.BSharp.Enums {
    [PublicAPI]
    public static class ClusivityUtils {
        public static string MaxSymbol(this Clusivity clusivity) {
            return clusivity == Clusivity.Exclusive ? ")" : "]";
        }

        public static string MinSymbol(this Clusivity clusivity) {
            return clusivity == Clusivity.Exclusive ? "(" : "[";
        }

        public static string FormatRange(object min, Clusivity minClusivity, object max, Clusivity maxClusivity) {
            return $"{minClusivity.MinSymbol()}{min}, {max}{maxClusivity.MaxSymbol()}";
        }
    }
}