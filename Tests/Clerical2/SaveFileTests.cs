// using System;
// using System.Collections.Generic;
// using System.Data;
// using System.IO;
// using System.Threading;
// using System.Threading.Tasks;
//
// using FowlFever.Clerical;
// using FowlFever.Clerical.Validated;
// using FowlFever.Testing;
//
// using Microsoft.Diagnostics.Symbols;
// using Microsoft.Diagnostics.Tracing.Parsers.JSDumpHeap;
//
// using NUnit.Framework;
//
// using StringTokenFormatter;
//
// namespace BSharp.Tests.Clerical2;
//
// public class SaveFileTests : BaseClericalTest {
//     public record TestSaveData;
//
//     [Test]
//     public void NewSaveFileTest() {
//         var now = DateTimeOffset.Now;
//         var saveFile = new VolatileSaveFile<TestSaveData>(
//             TestFolder,
//             default,
//             Clerk.GetRandomFileName(),
//             now
//         );
//
//         Asserter.Against(saveFile)
//                 .And(Has.Property(nameof(saveFile.Data)).Null)
//                 .And(Has.Property(nameof(saveFile.File)).Null)
//                 .And(Has.Property(nameof(saveFile.TimeStamp)).EqualTo(now))
//                 .Invoke();
//     }
//
//     [Test]
//     public void PersistSaveFileTest() {
//         var saveFile = new VolatileSaveFile<TestSaveData>(
//             TestFolder,
//             new TestSaveData(),
//             Clerk.GetRandomFileName(),
//             DateTimeOffset.Now
//         );
//
//         Assert.That(saveFile, Has.Property(nameof(saveFile.File)).Not.Exist);
//         saveFile.Save();
//         Assert.That(saveFile, Has.Property(nameof(saveFile.File)).Exist);
//     }
//
//     [Test]
//     public void UnloadedDataIsNull() {
//         var data = GetFileWithData();
//         var saveFile = new VolatileSaveFile<TestFileData>(
//             TestFolder,
//             Clerk.GetFileName(data.File.Name),
//             DateTimeOffset.Now
//         );
//
//         Assert.That(() => saveFile.Data, Is.Null);
//         saveFile.Sync();
//         Assert.That(() => saveFile.Data, Is.EqualTo(data.Data));
//     }
//
//     #region VolatileFile
//
//     [Test]
//     public void VolatileFileMustExist() {
//         var tempFile = GetTempFile(false);
//         Console.WriteLine($"Temp File: {tempFile}");
//         var vf       = new VolatileFile<TestSaveData>(default, tempFile);
//         Console.WriteLine(vf);
//
//         Assert.That(() => vf.GetData(), Throws.Exception);
//     }
//
//     #endregion
// }

