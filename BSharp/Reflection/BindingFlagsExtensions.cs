using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp.Reflection;

public static class BindingFlagsExtensions {
    private static readonly IReadOnlyDictionary<MemberTypes, BindingFlags> BindingFlag_To_MemberType = new Dictionary<MemberTypes, BindingFlags>() {
        [MemberTypes.Constructor] = BindingFlags.CreateInstance,
        [MemberTypes.Method]      = BindingFlags.InvokeMethod,
        [MemberTypes.Field]       = BindingFlags.GetField    | BindingFlags.SetField,
        [MemberTypes.Property]    = BindingFlags.GetProperty | BindingFlags.SetProperty,
    };

    /// <summary>
    /// Returns a set of <see cref="BindingFlags"/> that will match all instances of the given <see cref="MemberTypes"/>.
    /// </summary>
    /// <param name="memberTypes">the desired <see cref="MemberTypes"/></param>
    /// <returns>a set of <see cref="BindingFlags"/> that will match all instances of the given <see cref="MemberTypes"/></returns>
    public static BindingFlags GetBindingFlags(this MemberTypes memberTypes) {
        const BindingFlags baseBinding = BindingFlags.Instance |
                                         BindingFlags.Static   |
                                         BindingFlags.Public   |
                                         BindingFlags.NonPublic;

        return memberTypes.EachFlag()
                          .Aggregate(
                              baseBinding,
                              (soFar, nextMemberType) => {
                                  if (BindingFlag_To_MemberType.TryGetValue(nextMemberType, out var nextBindingFlag)) {
                                      return soFar | nextBindingFlag;
                                  }

                                  return nextBindingFlag;
                              }
                          );
    }
}