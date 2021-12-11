using System.IO;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp.Clerical {
    public enum DirectorySeparator {
        /// <summary>
        /// Aka "Unix".
        /// </summary>
        Universal,
        Windows,
        PlatformDependent
    }

    public static class DirectorySeparatorExtensions {
        public static char ToChar(this DirectorySeparator separator) {
            return separator switch {
                DirectorySeparator.Universal         => '/',
                DirectorySeparator.Windows           => '\\',
                DirectorySeparator.PlatformDependent => Path.DirectorySeparatorChar,
                _                                    => throw BEnum.InvalidEnumArgumentException(nameof(separator), separator)
            };
        }


        public static string ToCharString(this DirectorySeparator separator) {
            return separator.ToChar().ToString();
        }
    }
}