using System.IO;

using FowlFever.Clerical;

using ISaveFolder = FowlFever.Clerical.ISaveFolder;

namespace BSharp.Tests.Clerical2;

public abstract class BaseClericalTest {
    protected static readonly FileName TestFolderName = new FileName(nameof(BaseClericalTest));
    protected static readonly ISaveFolder TestFolder = new SaveFolder(Path.GetTempPath(),TestFolderName);
}