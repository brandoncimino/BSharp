using System;
using System.IO;

using FowlFever.Clerical;
using FowlFever.Testing;

using NUnit.Framework;

using ISaveData = FowlFever.Clerical.ISaveData;

namespace BSharp.Tests.Clerical2;

public class SaveFileTests : BaseClericalTest {
    public record TestSaveData : ISaveData;

    [Test]
    public void NewSaveFileTest() {
        var now = DateTimeOffset.Now;
        var saveFile = new SaveFile2<TestSaveData>(
            TestFolder,
            default,
            FileName.Random(),
            now
        );

        Asserter.Against(saveFile)
                .And(Has.Property(nameof(saveFile.Data)).Null)
                .And(Has.Property(nameof(saveFile.File)).Null)
                .And(Has.Property(nameof(saveFile.TimeStamp)).EqualTo(now))
                .Invoke();
    }

    [Test]
    public void PersistSaveFileTest() {
        var saveFile = new SaveFile2<TestSaveData>(
            TestFolder,
            new TestSaveData(),
            FileName.Random(),
            DateTimeOffset.Now
        );

        Assert.That(saveFile, Has.Property(nameof(saveFile.File)).Not.Exist);
        saveFile.Save();
        Assert.That(saveFile, Has.Property(nameof(saveFile.File)).Exist);
    }
}