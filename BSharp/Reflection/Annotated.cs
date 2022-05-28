using System;
using System.Collections.Generic;
using System.Reflection;

namespace FowlFever.BSharp.Reflection;

/// <summary>
/// Joins together <see cref="Attribute"/>s with their target <see cref="MemberInfo"/>.
/// </summary>
/// <typeparam name="TMember">the annotated <see cref="MemberInfo"/> type</typeparam>
/// <typeparam name="TAttribute">the <see cref="Attribute"/> type</typeparam>
public class Annotated<TMember, TAttribute>
    where TMember : MemberInfo
    where TAttribute : Attribute {
    public TMember                 Member     { get; }
    public IEnumerable<TAttribute> Attributes { get; }

    public Annotated(TMember member, IEnumerable<TAttribute>? attributes = null, bool inherit = true) {
        Member     = member;
        Attributes = attributes ?? Member.GetCustomAttributes<TAttribute>(inherit);
    }
}