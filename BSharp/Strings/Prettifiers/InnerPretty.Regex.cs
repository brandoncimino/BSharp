using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Strings.Prettifiers;

internal static partial class InnerPretty {
    public static string PrettifyRegexMatch(Match match, PrettificationSettings settings) {
        //TODO: finish this!
        if (match.Success == false) {
            return "💔 No Match";
        }

        var pattern      = _DeMatch(match);
        var groupNames   = pattern.GetGroupNames();
        var groupNumbers = pattern.GetGroupNumbers();
        var groupDic     = new Dictionary<string, string>();
        foreach (var gn in groupNames) {
            groupDic[$"📛 {gn}"] = match.Groups[gn].Value;
        }

        foreach (var gi in groupNumbers) {
            groupDic[$"#️⃣ {gi}"] = match.Groups[gi].Value;
        }

        int gc = 0;
        foreach (Group g in match.Groups) {
            Console.WriteLine($"{gc}: {g}");
            gc++;
        }

        return groupDic.Prettify();
    }

    private static readonly FieldInfo MatchRegexField = typeof(Match).GetField("_regex", BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField) ?? throw new MissingFieldException();

    private static Regex _DeMatch(Match match) {
        return MatchRegexField.GetValue(match) as Regex ?? throw new BrandonException();
    }
}