using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Attributes;
using FowlFever.BSharp.Collections;

namespace FowlFever.BSharp.Reflection;

/// <summary>
/// Represents a <see cref="PropertyInfo"/> and the <see cref="VariableInfo"/> that <see cref="Back"/>s it.
/// </summary>
/// <remarks>
/// Sources of <see cref="BackedProperty"/> relationships include, as of 5/27/2022:
/// <ul>
/// <li><a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Properties</a></li>
/// <li><see cref="AttributeTargets.Property"/>s annotated with <see cref="BackedByAttribute"/></li>
/// <li><see cref="AttributeTargets.Property"/>s <b>OR</b> <see cref="AttributeTargets.Field"/>s annotated with <see cref="BackerForAttribute"/></li>
/// </ul>
///
/// TODO: Derive <see cref="BackedProperty"/>s from <see cref="AccessedThroughPropertyAttribute"/>
/// </remarks>
/// <seealso cref="AutoProperty"/>
public class BackedProperty : IEquatable<BackedProperty> {
    /// <summary>
    /// The <see cref="PropertyInfo"/> that exposes <see cref="Back"/>.
    /// </summary>
    public PropertyInfo Front { get; }
    /// <summary>
    /// The source of <see cref="Front"/>'s value.
    /// </summary>
    /// <remarks>
    /// A backer can be:
    /// <ul>
    /// <li>A compiler-generated <see cref="AutoProperty"/> backing <see cref="FieldInfo"/></li>
    /// <li>A <see cref="AttributeTargets.Field"/> or <see cref="AttributeTargets.Property"/> annotated with <see cref="BackerForAttribute"/></li>
    /// <li>A <see cref="AttributeTargets.Field"/> or <see cref="AttributeTargets.Property"/> referenced by a <see cref="BackedByAttribute"/></li>
    /// </ul>
    /// </remarks>
    public VariableInfo Back { get; }

    private static readonly IEqualityComparer<MemberInfo> MemberComparer = MetadataTokenComparer.Instance;

    #region Constructors & Factories

    internal BackedProperty(PropertyInfo front, VariableInfo back) {
        Front = front;
        Back  = back;
    }

    /// <param name="front">the desired <see cref="Front"/></param>
    /// <returns>the <see cref="BackedProperty"/> that the given <see cref="PropertyInfo"/> participates in as a <see cref="Front"/>, if found; otherwise, <c>null</c></returns>
    public static BackedProperty? FromFront(PropertyInfo front) => FrontCache.GetOrAddLazily(front, _FromFront);

    /// <param name="backer">the desired <see cref="Back"/></param>
    /// <returns>all of the <see cref="BackedProperty"/>s that the given <see cref="MemberInfo"/> participates in as a <see cref="Back"/></returns>
    public static BackedProperty[] FromBack(MemberInfo backer) => BackCache.GetOrAddLazily(backer, _FromBack);

    #endregion

    #region Equality

    public bool Equals(BackedProperty? other) {
        if (ReferenceEquals(null, other)) {
            return false;
        }

        if (ReferenceEquals(this, other)) {
            return true;
        }

        return GetHashCode() == other.GetHashCode();
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) {
            return false;
        }

        if (ReferenceEquals(this, obj)) {
            return true;
        }

        if (obj.GetType() != this.GetType()) {
            return false;
        }

        return Equals((BackedProperty)obj);
    }

    public override int GetHashCode() {
        var propHash = MemberComparer.GetHashCode(Front);
        var backHash = MemberComparer.GetHashCode(Back);
        return (propHash * backHash).GetHashCode();
    }

    public static bool operator ==(BackedProperty? left, BackedProperty? right) => Equals(left, right);
    public static bool operator !=(BackedProperty? left, BackedProperty? right) => !Equals(left, right);

    #endregion

    #region Equalities using the MemberComparer

    public bool BackerEquals(MemberInfo  potentialBacker) => MemberComparer.Equals(Back,  potentialBacker);
    public bool FrontEquals(PropertyInfo potentialFront)  => MemberComparer.Equals(Front, potentialFront);

    #endregion

    #region Caching and Retrieving

    /// <summary>
    /// Caches the results of <see cref="FromFront"/>
    /// </summary>
    private static readonly ConcurrentDictionary<MemberInfo, Lazy<BackedProperty?>> FrontCache = new();

    /// <summary>
    /// Caches the results of <see cref="FromBack"/>
    /// </summary>
    private static readonly ConcurrentDictionary<MemberInfo, Lazy<BackedProperty[]>> BackCache = new();

    /// <summary>
    /// The non-cached method that powers <see cref="FromFront"/>
    /// </summary>
    private static BackedProperty? _FromFront(PropertyInfo front) {
        BackedProperty? FromBackerCache() => CacheBackers(front).TryGetValue(front, out var bp) ? bp : null;

        return AutoProperty.AutoPropertyFrom(front) ?? FromBackerCache();
    }

    /// <summary>
    /// The non-cached method that powers <see cref="FromBack"/>
    /// </summary>
    private static BackedProperty[] _FromBack(MemberInfo backer) {
        BackedProperty[]? FromAutoProperty() {
            return AutoProperty.AutoPropertyFrom(backer)
                               ?
                               .WrapInEnumerable()
                               .Cast<BackedProperty>()
                               .ToArray();
        }

        BackedProperty[] FromBackerCache() {
            return CacheBackers(backer)
                   .Values
                   .Where(it => it.BackerEquals(backer))
                   .ToArray();
        }

        return FromAutoProperty() ?? FromBackerCache();
    }

    /// <summary>
    /// Holds all of the <see cref="BackedProperty"/>s we've ever found, indexed by their <see cref="BackedProperty.Front"/>
    /// </summary>
    private static readonly ConcurrentDictionary<PropertyInfo, BackedProperty> BackerCache = new(MetadataTokenComparer.Instance);

    /// <summary>
    /// Holds all of the <see cref="Type"/>s that have been <see cref="BackerCache"/>d
    /// </summary>
    private static readonly ConcurrentSet<Type> CachedTypes = new();

    /// <summary>
    /// Caches all of the backers for <see cref="MemberInfo.ReflectedType"/> and <see cref="MemberInfo.DeclaringType"/> into <see cref="BackerCache"/>.
    /// </summary>
    /// <param name="member">this <see cref="MemberInfo"/></param>
    /// <returns>the updated <see cref="BackerCache"/></returns>
    private static ConcurrentDictionary<PropertyInfo, BackedProperty> CacheBackers(MemberInfo member) {
        void CacheTypeBackers(Type? type) {
            void CacheFront() {
                IEnumerable<BackedProperty> FromAnnotatedFront(Annotated<PropertyInfo, BackedByAttribute> annotated) {
                    return annotated.Attributes.Select(it => new BackedProperty(annotated.Member, it.GetBacker(annotated.Member)));
                }

                type.FindAnnotated<PropertyInfo, BackedByAttribute>()
                    .SelectMany(FromAnnotatedFront)
                    .ForEach(it => BackerCache.TryAdd(it.Front, it));
            }

            void CacheBack() {
                IEnumerable<BackedProperty> FromAnnotatedBack(Annotated<VariableInfo, BackerForAttribute> annotated) {
                    return annotated.Attributes.Select(it => new BackedProperty(it.GetBackedProperty(annotated.Member), annotated.Member));
                }

                type.FindAnnotated<VariableInfo, BackerForAttribute>()
                    .SelectMany(FromAnnotatedBack)
                    .ForEach(it => BackerCache.TryAdd(it.Front, it));
            }

            if (type == null) {
                return;
            }

            // `.Add()` returns `true` if the set was actually modified
            var needsToBeCached = CachedTypes.Add(type);

            if (!needsToBeCached) {
                return;
            }

            CacheBack();
            CacheFront();
        }

        CacheTypeBackers(member.ReflectedType);
        CacheTypeBackers(member.DeclaringType);
        return BackerCache;
    }

    #endregion
}

public static class BackedPropertyExtensions {
    /// <summary>
    /// Returns the <see cref="BackedProperty.Back">"backer"</see> for a <see cref="PropertyInfo"/>.
    /// </summary>
    /// <param name="propertyInfo">this <see cref="PropertyInfo"/></param>
    /// <returns>the <see cref="VariableInfo"/> that backs this <see cref="PropertyInfo"/>, if found; otherwise, null</returns>
    [Pure]
    public static VariableInfo? GetBacker(this PropertyInfo propertyInfo) => BackedProperty.FromFront(propertyInfo)?.Back;

    /// <summary>
    /// Returns the <see cref="BackedProperty.Front"/>s that this <see cref="VariableInfo"/> is a <see cref="BackedProperty.Back"/> for.
    /// </summary>
    /// <param name="backer">this <see cref="VariableInfo"/></param>
    /// <returns><b>all</b> of the <see cref="PropertyInfo"/>s that this is a <see cref="BackedProperty.Back"/> for</returns>
    [Pure]
    public static IEnumerable<PropertyInfo> GetBackedProperties(this VariableInfo backer) => BackedProperty.FromBack(backer).Select(it => it.Front);

    /// <inheritdoc cref="GetBackedProperties(FowlFever.BSharp.Reflection.VariableInfo)"/>
    public static IEnumerable<PropertyInfo> GetBackedProperties(this PropertyInfo backer) => BackedProperty.FromBack(backer).Select(it => it.Front);

    /// <inheritdoc cref="GetBackedProperties(FowlFever.BSharp.Reflection.VariableInfo)"/>
    public static IEnumerable<PropertyInfo> GetBackedProperties(this FieldInfo backer) => BackedProperty.FromBack(backer).Select(it => it.Front);
}