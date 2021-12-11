using System;

using FowlFever.BSharp.Clerical.Saving;
using FowlFever.BSharp.Strings;
using FowlFever.Testing;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace BrandonUtils.Tests.Standalone.Clerical {
    [TestOf(nameof(SaveFileName))]
    public class SaveFileNameTests {
        public class SaveFileNameExpectation {
            public string   Nickname;
            public DateTime TimeStamp;
            public string   Extension;
            public string   RenderedName;

            public SaveFileNameExpectation(string nickname, long timeStamp_ticks, string extension, string renderedName) {
                Nickname     = nickname;
                TimeStamp    = new DateTime(timeStamp_ticks);
                Extension    = extension;
                RenderedName = renderedName;
            }
        }

        private static SaveFileNameExpectation[] GetExpectations() {
            return new[] {
                new SaveFileNameExpectation(
                    nameof(SaveFileName_Fancy),
                    637671744000000000,
                    ".butts",
                    "SaveFileName_Fancy_637671744000000000.butts"
                ),
                new SaveFileNameExpectation(
                    "f7299b3c-7788-493e-8b92-86da6ae60dd6",
                    99,
                    "yolo",
                    "f7299b3c-7788-493e-8b92-86da6ae60dd6_99.yolo"
                ),
                new SaveFileNameExpectation(
                    "#",
                    0,
                    "yolo.swag",
                    "#_0.yolo.swag"
                )
            };
        }

        [Test]
        public void SaveFileName_Fancy([ValueSource(nameof(GetExpectations))] SaveFileNameExpectation expectation) {
            var sfn = new SaveFileName() {
                Nickname      = expectation.Nickname,
                TimeStamp     = expectation.TimeStamp,
                FullExtension = expectation.Extension
            };

            Assert.That(sfn.Rendered, Is.EqualTo(expectation.RenderedName));
        }

        [Test]
        public void SaveFileName_Parse([ValueSource(nameof(GetExpectations))] SaveFileNameExpectation expectation) {
            var sfn = SaveFileName.Parse(expectation.RenderedName);
            AssertAll.Of(
                () => Assert.That(sfn.Rendered,      Is.EqualTo(expectation.RenderedName)),
                () => Assert.That(sfn.Nickname,      Is.EqualTo(expectation.Nickname)),
                () => Assert.That(sfn.TimeStamp,     Is.EqualTo(expectation.TimeStamp)),
                () => Assert.That(sfn.FullExtension, Is.EqualTo(expectation.Extension.PrefixIfMissing(".")))
            );
        }
    }
}