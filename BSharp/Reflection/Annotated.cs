using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

using FowlFever.BSharp.Attributes;
using FowlFever.Implementors;

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
    IHas<TMember>,
    IEquatable<MemberInfo?>
    where TMember : MemberInfo
    where TAttribute : Attribute {
    public TMember                    Member     { get; }
    public ImmutableArray<TAttribute> Attributes { get; }
    public bool                       IsEmpty    => Attributes.IsEmpty;
    public bool                       IsNotEmpty => !IsEmpty;

    public Annotated(TMember member, IEnumerable<TAttribute> attributes, bool validateTarget = true) {
        Member     = member;
        Attributes = attributes.ToImmutableArray();
        if (validateTarget) {
            Attributes.OfType<ITargetValidatedAttribute>()
                      .ForEach(it => it.ValidateTarget(Member));
        }
    }

    public Annotated(TMember member, bool inherit = true, bool validateTarget = true) : this(
        member,
        member.GetCustomAttributes<TAttribute>(inherit),
        validateTarget
    ) { }

    public bool           Equals(MemberInfo? other) => new MetadataTokenComparer().Equals(Member, other);
    TMember IHas<TMember>.Value                     => Member;
}