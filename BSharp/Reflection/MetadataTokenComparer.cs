using System.Collections.Generic;
using System.Reflection;

namespace FowlFever.BSharp.Reflection;

/// <summary>
/// Compares <see cref="MemberInfo"/>s by their <see cref="MemberInfo.MetadataToken"/>.
/// </summary>
public sealed class MetadataTokenComparer : IEqualityComparer<MemberInfo?>, IComparer<MemberInfo?> {
    public                 bool                  Equals(MemberInfo?      x, MemberInfo? y) => x?.MetadataToken == y?.MetadataToken;
    public                 int                   GetHashCode(MemberInfo? obj)              => obj?.MetadataToken.GetHashCode() ?? 0;
    public                 int                   Compare(MemberInfo?     x, MemberInfo? y) => Comparer<int?>.Default.Compare(x?.MetadataToken, y?.MetadataToken);
    public static readonly MetadataTokenComparer Instance = new();
}