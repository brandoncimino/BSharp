using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using FowlFever.BSharp.Clerical.Saving;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests.Clerical {
    [SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
    public class SaveFileTests {
        private static readonly SaveFolder TestFolder = new SaveFolder(Path.GetTempPath(), nameof(SaveFileTests));

        private static class Validate {
            public static void SaveFile_Basic<T>(SaveFile<T> saveFile, string expectedName, DateTime expectedTimeStamp) where T : ISaveData {
                AssertAll.Of(
                    saveFile,
                    Has.Property(nameof(saveFile.Nickname)).EqualTo(expectedName),
                    Has.Property(nameof(saveFile.TimeStamp)).EqualTo(expectedTimeStamp),
                    Has.Property(nameof(saveFile.FileSystemInfo)).With.Property(nameof(FileSystemInfo.Name)).EqualTo($"{expectedName}_{expectedTimeStamp.Ticks}{SaveFileName.DefaultExtension}")
                );
            }
        }

        [Test]
        public void Save_NullData() {
            var bDay     = new DateTime(1993, 7, 1);
            var nickname = nameof(Save_NullData) + Guid.NewGuid();
            var saveFile = new SaveFile<TestSaveData>(TestFolder, nickname, bDay);

            AssertAll.Of(
                () => Assert.That(saveFile, Has.Property(nameof(saveFile.Data)).Null),
                () => Assert.That(saveFile, Has.Property(nameof(saveFile.File)).Not.Exist),
                () => Validate.SaveFile_Basic(saveFile, nickname, bDay)
            );

            saveFile.Save();

            AssertAll.Of(
                () => Assert.That(saveFile, Has.Property(nameof(saveFile.Data)).Null),
                () => Assert.That(saveFile, Has.Property(nameof(saveFile.File)).Exist),
                () => Validate.SaveFile_Basic(saveFile, nickname, bDay)
            );
        }

        [Test]
        public void Save_RealData() {
            var saveData = new TestSaveData(nameof(Save_RealData) + Guid.NewGuid());

            var saveFile = new SaveFile<TestSaveData>(TestFolder, nameof(Save_RealData) + "_File", DateTime.Now, default, saveData);

            Asserter.Against(saveFile)
                    .And(Has.Property(nameof(saveFile.Data)).Not.Null)
                    .And(Has.Property(nameof(saveFile.FileSystemInfo)).Not.Exist)
                    .Invoke();

            saveFile.Save();

            Asserter.Against(saveFile)
                    .And(Has.Property(nameof(saveFile.Data)).Not.Null)
                    .And(Has.Property(nameof(saveFile.FileSystemInfo)).Exist)
                    .Invoke();
        }

        [Test]
        public void NewSaveFileFromExistingFile() {
            //create a file
            var saveData      = new TestSaveData(nameof(NewSaveFileFromExistingFile)                       + Guid.NewGuid());
            var firstSaveFile = new SaveFile<TestSaveData>(TestFolder, nameof(NewSaveFileFromExistingFile) + "_File", DateTime.Now, default, saveData);

            firstSaveFile.Save();

            var secondSaveFile = new SaveFile<TestSaveData>(TestFolder, firstSaveFile.File);

            Asserter.Against(secondSaveFile)
                    .Exists()
                    .And(Has.Property(nameof(secondSaveFile.Data)).Null)
                    .And(Has.Property(nameof(secondSaveFile.Nickname)).EqualTo(firstSaveFile.Nickname))
                    .And(Has.Property(nameof(secondSaveFile.TimeStamp)).EqualTo(firstSaveFile.TimeStamp))
                    .Invoke();

            secondSaveFile.Load();
            Asserter.Against(secondSaveFile)
                    .Exists()
                    .And(Has.Property(nameof(secondSaveFile.Data)).Not.Null)
                    .And(() => Assert.True(firstSaveFile.Data?.Equals(secondSaveFile.Data), "firstSaveFile.Data.Equals(secondSaveFile.Data)"))
                    .And(Has.Property(nameof(secondSaveFile.Data)).EqualTo(firstSaveFile.Data))
                    .Invoke();
        }
    }
}