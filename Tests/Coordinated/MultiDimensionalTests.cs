using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Coordinated;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests.Coordinated;

public class MultiDimensionalTests {
    static MultiDimensional<(int x, int y, int z)> CreateChunked(int x, int y, int z) {
        var total  = x * y * z;
        var source = new List<(int, int, int)>();
        for (int xi = 0; xi < x; xi++) {
            for (int yi = 0; yi < y; yi++) {
                for (int zi = 0; zi < z; zi++) {
                    source.Add((xi, yi, zi));
                }
            }
        }

        var chunked = new MultiDimensional<(int, int, int)>(source, new[] { x, y, z });
        return chunked;
    }

    [Test]
    [TestCase(10, 40, 30, 4, 0, 2,  Should.ThrowNothing)]
    [TestCase(10, 40, 30, 0, 0, 41, Should.ThrowException)]
    public void ChunkedTest(
        int    lx,
        int    ly,
        int    lz,
        int    x,
        int    y,
        int    z,
        Should should
    ) {
        var chunked = CreateChunked(lx, ly, lz);
        Assert.That(() => chunked[x, y, z], should.Constrain());
    }

    [Test]
    [TestCase(2, 1)]
    [TestCase(2, 3)]
    public void Chunked_RejectsWrongDimensions(int actualDimensions, int requestedDimensions) {
        var md = new MultiDimensional<int>(new int[] { }, new int[actualDimensions]);
        Assert.That(() => md[Enumerable.Repeat(0, requestedDimensions).ToArray()], Throws.Exception);
    }
}