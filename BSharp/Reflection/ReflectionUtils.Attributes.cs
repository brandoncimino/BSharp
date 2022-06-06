using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp.Reflection;

public static partial class ReflectionUtils {
    #region Attributes

    private const BindingFlags LooseBindingFlags = BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod;

    /// <summary>
    /// A <see cref="MemberFilter"/> for members where a certain <see cref="Attribute"/> <see cref="MemberInfo.IsDefined"/>.
    /// </summary>
    internal static readonly MemberFilter AnnotatedMemberFilter = (member, annotationType) => member.IsDefined((annotationType as Type)!);

    /// <summary>
    /// Similar to the built-in <see cref="Type.FindMembers"/>, except that it is capable of discovering <see cref="BindingFlags.Static"/>
    /// methods declared in <see cref="Type.BaseType"/>s <i>(parent classes)</i>
    /// </summary>
    /// <param name="type">this <see cref="Type"/></param>
    /// <param name="memberType">the desired <see cref="MemberTypes"/></param>
    /// <param name="bindingAttr">a bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the search is conducted</param>
    /// <param name="filter">a <see cref="MemberFilter"/> that will be applied to each potential <see cref="MemberInfo"/></param>
    /// <param name="filterCriteria">an <see cref="object"/> passed to the <see cref="MemberFilter"/> to aid in comparisons</param>
    /// <returns>all of the matching <see cref="MemberInfo"/>s</returns>
    /// <remarks>
    /// This method intentionally matches the method signature of <see cref="Type.FindMembers"/>.
    /// For the more convenient, modern equivalent, see the generic <see cref="FindAllMembers{T,T}"/>.
    /// </remarks>
    internal static IEnumerable<MemberInfo> FindAllMembers(
        this Type    type,
        MemberTypes  memberType,
        BindingFlags bindingAttr,
        MemberFilter filter,
        object?      filterCriteria
    ) {
        var found = type.FindMembers(memberType, bindingAttr, filter, filterCriteria);

        if (!bindingAttr.HasFlag(BindingFlags.Static)) {
            return found;
        }

        var sansStatic = bindingAttr.WithoutFlag(BindingFlags.Static);
        found = type.GetAncestors()
                    .Skip(1)
                    .SelectMany(it => it.FindMembers(memberType, sansStatic, filter, filterCriteria))
                    .PrependNonNull(found)
                    .ToArray();

        return found;
    }

    /// <summary>
    /// Retrieves all of the <typeparamref name="TMember"/>s in this <see cref="Type"/> that satisfy the given <see cref="MemberFilter"/> and <paramref name="filterCriteria"/>.
    /// </summary>
    /// <param name="type">the <see cref="Type"/> that has the members</param>
    /// <param name="filter">the "bi-predicate" that determines if a <see cref="MemberInfo"/> should be returned</param>
    /// <param name="filterCriteria">the criteria object used by the <paramref name="filter"/></param>
    /// <typeparam name="TMember">a <see cref="MemberInfo"/> type</typeparam>
    /// <typeparam name="TCriteria">the type of the <paramref name="filterCriteria"/> object</typeparam>
    /// <returns>all of the matching <see cref="TMember"/>s</returns>
    /// <remarks>Slightly more extensive than <see cref="RuntimeReflectionExtensions"/> methods, because this method can recursively check for <see cref="BindingFlags.Static"/> members declared in <see cref="Type.BaseType"/>s (which are normally not accessible).</remarks>
    public static IEnumerable<TMember> FindAllMembers<TMember, TCriteria>(
        this Type                        type,
        Func<TMember?, TCriteria?, bool> filter,
        TCriteria                        filterCriteria
    )
        where TMember : MemberInfo {
        bool AsMemberFilter(MemberInfo info, object criteria) => filter((TMember)info, (TCriteria)criteria);
        return type.FindAllMembers(
                       GetMemberInfoMemberTypes<TMember>(),
                       GetMemberInfoBindingFlags<TMember>(),
                       AsMemberFilter,
                       filterCriteria
                   )
                   // 📝 NOTE: You must use Select with  an explicit cast from TMember to it, otherwise
                   //    `VariableInfo` wouldn't work properly 🤷‍
                   .Select(it => (TMember)it);
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
        return type.FindAllMembers(
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