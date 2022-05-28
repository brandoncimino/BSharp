using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FowlFever.BSharp.Reflection;

public static partial class ReflectionUtils {
    #region Attributes

    private const BindingFlags LooseBindingFlags = BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod;

    /// <summary>
    /// A <see cref="MemberFilter"/> for members where a certain <see cref="Attribute"/> <see cref="MemberInfo.IsDefined"/>.
    /// </summary>
    internal static readonly MemberFilter AnnotatedMemberFilter = (member, annotationType) => member.IsDefined((annotationType as Type)!);

    /// <summary>
    /// Compares <see cref="MemberInfo"/>s by their <see cref="MemberInfo.MetadataToken"/>.
    ///
    /// TODO: Would an extension method of "MetadataCompare" be useful? or is that silly when we could just write <c>a.MetadataToken.Equals(b.MetadataToken)</c>?
    /// </summary>
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
    /// 
    /// |               Method |        Mean |     Error |    StdDev |
    /// |--------------------- |------------:|----------:|----------:|
    /// |         ViaIsDefined | 2,237.31 ns | 33.473 ns | 53.092 ns |
    /// | Runtime_ViaIsDefined | 2,224.79 ns | 25.931 ns | 20.245 ns |
    /// |            ViaFilter |    78.97 ns |  0.813 ns |  0.761 ns |
    /// 
    /// ]]></code>
    ///
    /// This makes it worthwhile to construct the silly <see cref="AnnotatedMemberFilter"/>.
    /// 
    /// </remarks>
    /// <param name="type">the original <see cref="Type"/></param>
    /// <typeparam name="TMember">the <see cref="MemberTypes"/> you're searching for</typeparam>
    /// <typeparam name="TAttribute">the desired <see cref="Attribute"/></typeparam>
    /// <returns>the members with the specified <see cref="Attribute"/></returns>
    public static IEnumerable<TMember> FindMembersWithAttribute<TMember, TAttribute>(this Type type)
        where TMember : MemberInfo
        where TAttribute : Attribute {
        return type.FindMembers(
                       GetMemberInfoMemberTypes<TMember>(),
                       GetMemberInfoBindingFlags<TMember>(),
                       FilterWithAttribute,
                       typeof(TAttribute)
                   )
                   .Select(it => (TMember)it);
    }

    /// <summary>
    /// Similar to <see cref="FindMembersWithAttribute{TMember,TAttribute}"/>, but returns the results as <see cref="Annotated{TMember,TAttribute}"/> instances.
    /// </summary>
    /// <param name="type">the original <see cref="Type"/></param>
    /// <typeparam name="TMember">the <see cref="MemberTypes"/> you're searching for</typeparam>
    /// <typeparam name="TAttribute">the desired <see cref="Attribute"/></typeparam>
    /// <returns>the <see cref="Annotated{TMember,TAttribute}"/> members</returns>
    public static IEnumerable<Annotated<TMember, TAttribute>> FindAnnotated<TMember, TAttribute>(this Type type)
        where TMember : MemberInfo
        where TAttribute : Attribute {
        return type.FindMembersWithAttribute<TMember, TAttribute>()
                   .Select(it => new Annotated<TMember, TAttribute>(it));
    }

    /// <inheritdoc cref="CustomAttributeExtensions.GetCustomAttributes{T}(MemberInfo, bool)"/>
    /// <returns>An <see cref="Annotated{TMember,TAttribute}"/> instance containing the results.</returns>
    public static Annotated<TMember, TAttribute> GetAnnotated<TMember, TAttribute>(this TMember member, bool inherit = true)
        where TMember : MemberInfo
        where TAttribute : Attribute
        => new(member);

    /// <inheritdoc cref="GetAnnotated{TMember,TAttribute}"/>
    public static Annotated<PropertyInfo, T> GetAnnotated<T>(this PropertyInfo property, bool inherit = true)
        where T : Attribute => new(property);

    /// <inheritdoc cref="GetAnnotated{TMember,TAttribute}"/>
    public static Annotated<FieldInfo, T> GetAnnotated<T>(this FieldInfo field, bool inherit = true)
        where T : Attribute => new(field);

    /// <inheritdoc cref="GetAnnotated{TMember,TAttribute}"/>
    public static Annotated<MethodInfo, T> GetAnnotated<T>(this MethodInfo method, bool inherit = true)
        where T : Attribute => new(method);

    /// <inheritdoc cref="GetAnnotated{TMember,TAttribute}"/>
    public static Annotated<VariableInfo, T> GetAnnotated<T>(this VariableInfo variable, bool inherit = true)
        where T : Attribute => new(variable);

    /// <inheritdoc cref="GetAnnotated{TMember,TAttribute}"/>
    public static Annotated<ConstructorInfo, T> GetAnnotated<T>(this ConstructorInfo constructor, bool inherit = true)
        where T : Attribute => new(constructor);

    /// <inheritdoc cref="GetAnnotated{TMember,TAttribute}"/>
    public static Annotated<EventInfo, T> GetAnnotated<T>(this EventInfo eventInfo, bool inherit = true)
        where T : Attribute => new(eventInfo);

    #endregion
}