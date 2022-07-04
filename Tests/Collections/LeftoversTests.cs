using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.Testing;

using NUnit.Framework;
// using System.Diagnostics.Metrics;

namespace BSharp.Tests.Collections;

[SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
public class LeftoversTests {
    [Test(Description = $"{nameof(CollectionUtils.TakeLeftovers)} should not perform any immediate (aka 'eager') enumerations")]
    [TestCase(10, 3)]
    public void TakeLeftovers_EnumeratesOnce(int sourceCount, int takeCount) {
        var (taken, leftovers) = Enumerable.Range(0, 10)
                                           .WithCounter(out var counter, i => Console.WriteLine($"Loop #{i}"))
                                           .AssertCounter(counter, 0)
                                           .TakeLeftovers(takeCount);

        Assert.That(counter.History, Has.Exactly(0).Items);

        taken.ToList().AssertCounter(counter, takeCount);
        leftovers.ToList().AssertCounter(counter, sourceCount);
    }

    [Test(Description = $"Demonstrates that {nameof(Enumerable.Skip)}(x) prevents enumeration of (x) items")]
    [TestCase(10, 2)]
    public void Example_Skip(int sourceCount, int skipCount) {
        Enumerable.Range(0, sourceCount)
                  .WithCounter(out var counter)
                  .Skip(skipCount)
                  .AssertCounter(counter, 0)
                  .ToList()
                  .AssertCounter(counter, sourceCount - skipCount);
    }

    [Test(Description = $"Demonstrates that {nameof(Enumerable.Take)}(x) only enumerates (x) items")]
    [TestCase(10, 2)]
    public void Example_Take(int sourceCount, int takeCount) {
        Enumerable.Range(0, sourceCount)
                  .WithCounter(out var counter)
                  .Take(takeCount)
                  .AssertCounter(counter, 0)
                  .ToList()
                  .AssertCounter(counter, takeCount);
    }
}