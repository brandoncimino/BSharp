using System;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp.Strings;

public readonly ref struct GraphemeClusterSpan {
    public ReadOnlySpan<char> Cluster { get; }

    public GraphemeClusterSpan(ReadOnlySpan<char> cluster) : this(cluster, GraphemeCluster.ShouldValidate.Yes) { }

    internal GraphemeClusterSpan(ReadOnlySpan<char> cluster, GraphemeCluster.ShouldValidate shouldValidate) {
        Cluster = shouldValidate switch {
            GraphemeCluster.ShouldValidate.Yes => GraphemeCluster.Validate(cluster),
            GraphemeCluster.ShouldValidate.No  => cluster,
            _                                  => throw BEnum.UnhandledSwitch(shouldValidate),
        };
    }

    public GraphemeCluster ToGraphemeCluster() => new(Cluster, GraphemeCluster.ShouldValidate.No);
}