using System.Linq;

using FowlFever.BSharp;
using FowlFever.BSharp.Strings.Spectral;

using NUnit.Framework;

namespace BSharp.Tests.Strings.Spectre;

public class AutoColumnsTests {
    [Test]
    [TestCase(4, 100, 4, new[] { 25, 25, 25, 25 })]
    [TestCase(4, 100, 3, new[] { 25, 25, 50 })]
    [TestCase(4, 100, 5, new[] { 20, 20, 20, 20, 20 })]
    public static void AutoColumns_GetWidth(int autoColCount, int consoleWidth, int usedColCount, int[] expectedWidths) {
        AutoColumns.ConsoleWidth    = () => consoleWidth;
        AutoColumns.AutoColumnCount = autoColCount;
        var widths = usedColCount.Repeat(i => AutoColumns.GetWidth(i, usedColCount)).ToArray();
        Assert.That(widths, Is.EqualTo(expectedWidths));
    }
}