using System;
using System.Collections.Immutable;

using FowlFever.BSharp.Attributes;
using FowlFever.BSharp.Collections.Implementors;
using FowlFever.BSharp.Reflection;

namespace FowlFever.BSharp.Exceptions;

[Experimental("Cute, but is it really valuable?")]
public static class Subtypes {
    public static SubtypeSet<PARENT> Of<PARENT>() => new();
}

public record SubtypeSet<PARENT> : IHasImmutableSet<Type> {
    IImmutableSet<Type> IHasImmutableSet<Type>.AsImmutableSet => LegalTypes;
    private ImmutableHashSet<Type>             LegalTypes     { get; init; } = ImmutableHashSet.Create<Type>();

    public SubtypeSet<PARENT> Including<A>()
        where A : PARENT => this with { LegalTypes = LegalTypes.Add(typeof(A)) };

    public PARENT Require<T>(T obj) {
        var asParent = obj.MustNotBeNull()
                          .MustBe<PARENT>();

        return asParent.MustBe(it => it?.IsInstanceOf(LegalTypes) == true);
    }
}