using System.ComponentModel;
using System.Linq;
using System.Text;

using FowlFever.BSharp.Strings.Settings;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// TODO: add to <see cref="PrettificationSettings"/>
/// </summary>
public enum StringAlignment {
    Left, Right, Center,
}

public static class StringAlignmentExtensions {
    public static Lines Align(
        this StringAlignment alignment,
        string               str,
        OneLine?             padString = default,
        FillerSettings?      settings  = default
    ) {
        return str.Align(
            alignment: alignment,
            padString: padString,
            settings: settings
        );
    }

    /// <summary>
    /// Applies a <see cref="StringAlignment"/> to this <see cref="string"/>.
    /// </summary>
    /// <param name="str">this <see cref="string"/></param>
    /// <param name="width">the length of the resulting <see cref="string"/></param>
    /// <param name="alignment">the type of <see cref="StringAlignment"/></param>
    /// <param name="padString">see <see cref="FillerSettings.PadString"/></param>
    /// <param name="settings">fine-grained <see cref="FillerSettings"/> <i>(📎 Individual parameters take precedence over these settings)</i></param>
    /// <returns>the <see cref="StringAlignment"/>-aligned <see cref="string"/></returns>
    public static Lines Align(
        this string str,
        [NonNegativeValue]
        int? width = default,
        StringAlignment? alignment = default,
        OneLine?         padString = default,
        FillerSettings?  settings  = default
    ) {
        settings = settings.Resolve();
        settings = settings with {
            LineLengthLimit = width ?? settings.LineLengthLimit,
            Alignment = alignment   ?? settings.Alignment,
            PadString = padString   ?? settings.PadString,
        };
        return str.Align(settings);
    }

    /// <summary>
    /// Adds repetitions of <see cref="filler"/> until <see cref="original"/> is at least <see cref="totalLength"/>.
    /// <p/>
    /// If <see cref="original"/> is longer than <see cref="totalLength"/>, nothing happens.
    /// </summary>
    /// <param name="original">this <see cref="string"/></param>
    /// <param name="totalLength">the desired <see cref="string.Length"/> of the result</param>
    /// <param name="filler">the <see cref="string"/> used to pad <paramref name="original"/>.
    /// <br/>
    /// 📎 If <see cref="string.IsNullOrEmpty"/>, <see cref="original"/> is used instead.</param>
    /// <param name="alignment">where the <see cref="original"/> string should be aligned within the result</param>
    /// <returns>a <see cref="string"/> with a <see cref="string.Length"/> >= <see cref="totalLength"/></returns>
    /// <exception cref="InvalidEnumArgumentException">if an unknown <see cref="StringAlignment"/> is passed</exception>
    public static Lines Fill(
        this string original,
        [NonNegativeValue]
        int totalLength,
        OneLine?        filler    = default,
        StringAlignment alignment = StringAlignment.Left
    ) {
        return original.Align(
            settings: new FillerSettings {
                LineLengthLimit = totalLength,
                PadString       = filler ?? OneLine.Space,
                Alignment       = alignment,
            }
        );
    }

    /// <summary>
    /// Aligns a string according to a set of <see cref="FillerSettings"/>.
    /// </summary>
    /// <param name="str">this <see cref="string"/></param>
    /// <param name="settings">a set of <see cref="FillerSettings"/></param>
    /// <returns>the aligned <see cref="string"/></returns>
    public static Lines Align(this string str, FillerSettings? settings) {
        return str.Lines().Select(it => it.Fit(settings.Resolve())).Lines();
    }

    /// <summary>
    /// Repeats a <see cref="string"/> until <see cref="desiredLength"/> is reached, with the last entry potentially being partial.
    /// </summary>
    /// <param name="toRepeat"></param>
    /// <param name="desiredLength"></param>
    /// <returns></returns>
    public static string RepeatToLength(this string toRepeat, [NonNegativeValue] int desiredLength) {
#if NET6_0_OR_GREATER
        //The following is an example using <c>string.Create</c>, but unfortunately that isn't available until C# 6.
        return string.Create(
            desiredLength,
            toRepeat,
            (span, source) => {
                for (int spanPos = 0; spanPos < desiredLength; spanPos++) {
                    var sourcePos = spanPos % source.Length;
                    span[spanPos] = source[sourcePos];
                }
            }
        );
#else
        var sb = new StringBuilder();

        for (var i = 0; i < desiredLength; i++) {
            sb.Append(toRepeat[i % toRepeat.Length]);
        }

        return sb.ToString();
#endif
    }
}