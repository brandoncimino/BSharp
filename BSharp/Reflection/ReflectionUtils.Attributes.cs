using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FowlFever.BSharp.Reflection;

public static partial class ReflectionUtils {
    private static bool IsAnnotated(MemberInfo member, object annotationType) {
        return member.IsDefined((annotationType as Type)!);
    }

    private static readonly Lazy<MethodInfo> IsAnnotatedMember = new Lazy<MethodInfo>(() => typeof(ReflectionUtils).GetMethod(nameof(IsAnnotated)));

    /// <summary>
    /// A <see cref="MemberFilter"/> for members where a certain <see cref="Attribute"/> <see cref="MemberInfo.IsDefined"/>.
    /// </summary>
    /// <remarks>
    /// This roundabout way of constructing a <see cref="MemberFilter"/> is, apparently, much more efficient: see <a href="https://www.youtube.com/watch?v=er9nD-usM1A">Achieving compile-time performance with Reflection in C#</a>
    /// </remarks>
    internal static readonly MemberFilter AnnotatedMemberFilter = (MemberFilter)Delegate.CreateDelegate(typeof(MemberFilter), IsAnnotatedMember.Value);

    internal class MetadataTokenComparer : Comparer<MemberInfo>, IEqualityComparer<MemberInfo> {
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

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// The difference in performance of <see cref="Type.GetMethods()"/> vs. <see cref="RuntimeReflectionExtensions.GetRuntimeMethods"/> isn't very significant,
    /// but the performance of <see cref="Type.FindMembers"/> is <b>considerably</b> better:
    /// 
    /// <code><![CDATA[
    /// |               Method |        Mean |     Error |    StdDev |
    /// |--------------------- |------------:|----------:|----------:|
    /// |         ViaIsDefined | 2,237.31 ns | 33.473 ns | 53.092 ns |
    /// | Runtime_ViaIsDefined | 2,224.79 ns | 25.931 ns | 20.245 ns |
    /// |            ViaFilter |    78.97 ns |  0.813 ns |  0.761 ns |
    /// ]]></code>
    /// 
    /// </remarks>
    /// <param name="type">the original <see cref="Type"/></param>
    /// <param name="bindingAttr">determines how the search is performed</param>
    /// <typeparam name="TMember">the <see cref="MemberTypes"/> you're searching for</typeparam>
    /// <typeparam name="TAttribute">the desired <see cref="Attribute"/></typeparam>
    /// <returns></returns>
    public static IEnumerable<TMember> FindMembersWithAttribute<TMember, TAttribute>(this Type type, BindingFlags bindingAttr = default)
        where TMember : MemberInfo
        where TAttribute : Attribute {
        return type.FindMembers(MemberTypesMap[typeof(TMember)], bindingAttr, FilterWithAttribute, typeof(TAttribute)).Select(it => (TMember)it);
    }

    /// <summary>
    /// Similar to <see cref="FindMembersWithAttribute{TMember,TAttribute}"/>, but returns the results as <see cref="Annotated{TMember,TAttribute}"/> instances.
    /// </summary>
    /// <param name="type">the original <see cref="Type"/></param>
    /// <param name="bindingAttr">determines how the search is performed</param>
    /// <typeparam name="TMember">the <see cref="MemberTypes"/> you're searching for</typeparam>
    /// <typeparam name="TAttribute">the desired <see cref="Attribute"/></typeparam>
    /// <returns></returns>
    public static IEnumerable<Annotated<TMember, TAttribute>> FindAnnotated<TMember, TAttribute>(this Type type, BindingFlags bindingAttr = default)
        where TMember : MemberInfo
        where TAttribute : Attribute {
        return type.FindMembersWithAttribute<TMember, TAttribute>(bindingAttr)
                   .Select(it => new Annotated<TMember, TAttribute>(it));
    }

    /// <inheritdoc cref="CustomAttributeExtensions.GetCustomAttributes{T}(MemberInfo, bool)"/>
    public static Annotated<TMember, TAttribute> GetAnnotated<TMember, TAttribute>(this TMember member, bool inherit = true)
        where TMember : MemberInfo
        where TAttribute : Attribute
        => new(member);
}