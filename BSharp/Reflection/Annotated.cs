using System;
using System.Collections.Generic;
using System.Reflection;

namespace FowlFever.BSharp.Reflection;

/// <summary>
/// Joins together <see cref="Attribute"/>s with their target <see cref="MemberInfo"/>.
/// </summary>
/// <typeparam name="TMember">the annotated <see cref="MemberInfo"/> type</typeparam>
/// <typeparam name="TAttribute">the <see cref="Attribute"/> type</typeparam>
/// <remarks>The addition of <c>sealed</c>, while not normally something I would do, increases the efficiency of <c>record</c>s considerably, apparently
/// <i>(TODO: citation needed)</i>
/// </remarks>
public sealed record Annotated<TMember, TAttribute> :
    IEquatable<MemberInfo?>
    where TMember : MemberInfo
    where TAttribute : Attribute {
    public TMember                 Member     { get; }
    public IEnumerable<TAttribute> Attributes { get; }

    public Annotated(TMember member, IEnumerable<TAttribute>? attributes = null, bool inherit = true) {
        Member     = member;
        Attributes = attributes ?? Member.GetCustomAttributes<TAttribute>(inherit);
    }

    public bool Equals(MemberInfo? other) => new MetadataTokenComparer().Equals(Member, other);
}