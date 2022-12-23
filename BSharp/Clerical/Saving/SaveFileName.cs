using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Clerical.Saving {
    /// <summary>
    /// A builder for names of <see cref="SaveFile{TData}"/>s.
    /// </summary>
    internal class SaveFileName {
        private string _nickname;
        public string Nickname {
            get => _nickname;
            set => _nickname = Validate_Nickname(value);
        }
        public DateTime TimeStamp;

        public const string DefaultExtension = ".sav.json";
        private      string _fullExtension   = DefaultExtension;

        public string FullExtension {
            get => _fullExtension;
            set => _fullExtension = value.PrependIfMissing(".");
        }

        public string BaseName => $"{Nickname}_{TimeStamp.Ticks}";
        public string Rendered => $"{BaseName}{FullExtension}";

        private static readonly  RegexGroup NicknameGroup       = new RegexGroup(nameof(NicknameGroup),  @".+");
        private static readonly  RegexGroup TimeStampGroup      = new RegexGroup(nameof(TimeStampGroup), @"\d+");
        internal static readonly Regex      BaseFileNamePattern = new Regex($@"^{NicknameGroup}_{TimeStampGroup}$");
        internal static readonly Regex      FileNamePattern     = new Regex($@"^{NicknameGroup}_{TimeStampGroup}{BPath.ExtensionGroup}$");

        internal static string GetFileSearchPattern(string nickname, string fullExtension) {
            return $"{nickname}_*{fullExtension}";
        }

        internal string GetFileSearchPattern() => GetFileSearchPattern(Nickname, FullExtension);

        public static SaveFileName Parse(string fileName) {
            return _parseInternal(fileName, FileNamePattern);
        }

        private static SaveFileName _parseInternal(string fileName, Regex expectedPattern) {
            var match = expectedPattern.Match(fileName);

            if (match.Success) {
                return new SaveFileName() {
                    Nickname = match.Groups[NicknameGroup.Name].Value, TimeStamp = new DateTime(long.Parse(match.Groups[TimeStampGroup.Name].Value)), FullExtension = match.Groups[BPath.ExtensionGroup.Name].Value,
                };
            }
            else {
                throw new FormatException($"Unable to convert the string \"{fileName.ToString(default)}\" to a {nameof(SaveFileName)} because it didn't match the {nameof(Regex)} pattern /{expectedPattern}/");
            }
        }

        public static SaveFileName Parse(FileInfo fileInfo) {
            return Parse(fileInfo.Name);
        }

        [ContractAnnotation("null => stop")]
        private static string Validate_Nickname(string? nickname) {
            if (nickname.IsEmpty()) {
                throw new ArgumentNullException(nameof(nickname), $"Must not be null or empty, but was [{nickname}]!");
            }

            var badCharacters = nickname.ToCharArray().Intersect(Path.GetInvalidFileNameChars()).ToList();
            if (badCharacters.IsNotEmpty()) {
                throw new ArgumentException($"{nameof(nickname)} [{nickname}] contained invalid file name characters: {badCharacters.Prettify()}");
            }

            return nickname;
        }

        public override string ToString() {
            return Rendered;
        }
    }
}