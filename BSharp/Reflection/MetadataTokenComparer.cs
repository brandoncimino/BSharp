using System.Collections.Generic;
using System.Reflection;

namespace FowlFever.BSharp.Reflection;

/// <summary>
/// Compares <see cref="MemberInfo"/>s by their <see cref="MemberInfo.MetadataToken"/>.
///
/// TODO: Would an extension method of "MetadataCompare" be useful? or is that silly when we could just write <c>a.MetadataToken.Equals(b.MetadataToken)</c>?
/// </summary>
public class MetadataTokenComparer : Comparer<MemberInfo>, IEqualityComparer<MemberInfo> {
    public bool Equals(MemberInfo x, MemberInfo y) {
        return x.MetadataToken == y.MetadataToken;
    }

    public int GetHashCode(MemberInfo obj) {
        return obj.MetadataToken.GetHashCode();
    }

    public override int Compare(MemberInfo x, MemberInfo y) {
        return x.MetadataToken.CompareTo(y.MetadataToken);
    }

    public static readonly MetadataTokenComparer Instance = new();
}