using System;
using System.IO;
using System.Linq;

using BrandonUtils.Testing;

using FowlFever.BSharp.Clerical.Saving;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace BrandonUtils.Tests.Standalone.Clerical {
    public class SaveSlotTests {
        private static readonly SaveFolder TestFolder = new SaveFolder(Path.GetTempPath(), nameof(SaveSlotTests));

        private static ISaveSlot<TestSaveData> GetUniqueSlot() {
            var slot = new SimpleSaveSlot<TestSaveData>(TestFolder, nameof(SimpleSaveSlot<TestSaveData>) + Guid.NewGuid());
            Console.WriteLine(slot);
            return slot;
        }

        [Test]
        public void NewSaveSlotIsEmpty() {
            var saveSlot = GetUniqueSlot();
            Console.WriteLine(saveSlot.SaveFileCount);
            Assert.That(saveSlot, Has.Property(nameof(saveSlot.SaveFileCount)).EqualTo(0));
        }

        [Test]
        public void SaveSlotNewFile() {
            var saveSlot = GetUniqueSlot();
            var nickname = nameof(SaveSlotNewFile) + Guid.NewGuid();
            var data     = new TestSaveData(nickname);

            Assert.That(saveSlot, Has.Property(nameof(saveSlot.SaveFileCount)).EqualTo(0));

            // save to the slot for the first time
            var now       = DateTime.Today;
            var firstSave = saveSlot.Save(data, now);

            Asserter.WithHeading("After the first save")
                    .And(
                        Asserter.Against(saveSlot)
                                .And(Has.Property(nameof(saveSlot.SaveFileCount)).EqualTo(1))
                                .And(
                                    it => it.EnumerateSaveFiles().Select(f => f.TimeStamp),
                                    Contains.Item(now)
                                )
                    )
                    .And(
                        Asserter.Against(firstSave)
                                .Exists()
                                .Nicknamed(saveSlot.Nickname)
                                .TimeStamped(now)
                                .And(Has.Property(nameof(firstSave.Data)).EqualTo(data))
                    )
                    .And(
                        Asserter.Against(saveSlot.LatestFile()?.Load())
                                .And(it => it.IsEquivalentTo(firstSave))
                    )
                    .Invoke();

            // save to the slot a _second_ time
            now += TimeSpan.FromDays(1);
            var secondSave = saveSlot.Save(data, now);
            Asserter.WithHeading("After second save")
                    .And(
                        Asserter.Against(saveSlot)
                                .And(Has.Property(nameof(saveSlot.SaveFileCount)).EqualTo(2))
                                .And(
                                    it => it.EnumerateSaveFiles().Select(f => f.TimeStamp),
                                    Contains.Item(now)
                                )
                    )
                    .And(
                        Asserter.Against(secondSave)
                                .Exists()
                                .Nicknamed(saveSlot.Nickname)
                                .TimeStamped(now)
                                .And(Has.Property(nameof(secondSave.Data)).EqualTo(data))
                                .And(Has.Property(nameof(secondSave.Data)).EqualTo(firstSave.Data))
                                .And(Has.Property(nameof(secondSave.TimeStamp)).Not.EqualTo(firstSave.TimeStamp))
                    )
                    .And(
                        Asserter.Against(saveSlot.LatestFile()?.Load())
                                .Exists()
                                .And(it => it.IsEquivalentTo(secondSave))
                    )
                    .Invoke();
        }

        [Test]
        public void SaveToSlot() {
            var saveSlot = GetUniqueSlot();
            var nickname = nameof(SaveToSlot) + Guid.NewGuid();
            var data     = new TestSaveData(nickname);

            var startTime = DateTime.Today;

            for (int i = 0; i < 5; i++) {
                var now = startTime.AddDays(i);
                data.Counter = i;

                var saved  = saveSlot.Save(data, now);
                var latest = saveSlot.LatestFile();
                latest?.Load();

                // expectations
                Asserter.Against(saveSlot)
                        .WithHeading($"Save Slot iteration #{i}")
                        .And(Has.Property(nameof(saveSlot.SaveFileCount)).EqualTo(i + 1))
                        .And(
                            Asserter.Against(latest)
                                    .WithHeading($"Latest File #{i}")
                                    .And(Is.Not.Null)
                                    .Exists()
                                    .Nicknamed(saveSlot.Nickname)
                                    .TimeStamped(now)
                                    .And(Has.Property(nameof(latest.Data)).Not.Null)
                                    .And(it => it.Data?.Counter, Is.EqualTo(i))
                        )
                        .And(
                            Asserter.Against(saved)
                                    .WithHeading($"Returned file from {nameof(saveSlot)}.{nameof(saveSlot.Save)}")
                                    .And(Is.Not.Null)
                                    .Exists()
                                    .Nicknamed(saveSlot.Nickname)
                                    .TimeStamped(now)
                                    .And(Has.Property(nameof(saved.Data)).Not.Null)
                                    .And(it => it.Data?.Counter, Is.EqualTo(i))
                                    .And(it => it.Data,          Is.EqualTo(latest.Data))
                        )
                        .Invoke();
            }
        }
    }
}