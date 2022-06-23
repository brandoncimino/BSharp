using System;
using System.Collections.Generic;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp.Reflection;

public static partial class ReflectionUtils {
    public enum AncestryOption { IncludeSelf, ExcludeSelf, }

    public static IEnumerable<Type> GetAncestors(this Type heir, AncestryOption option = default) {
        switch (option) {
            case AncestryOption.IncludeSelf:
                yield return heir;
                break;
            case AncestryOption.ExcludeSelf: break;
            default:                         throw BEnum.UnhandledSwitch(option);
        }

        while (heir.BaseType != null) {
            yield return heir.BaseType;
            heir = heir.BaseType;
        }
    }
}