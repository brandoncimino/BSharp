using System;
using System.Threading;

using FowlFever.BSharp;
using FowlFever.BSharp.Refreshing;

using NUnit.Framework;

namespace BrandonUtils.Tests.Standalone {
    public class FreshnessTests {
        public CountedFunc<DateTime> GetTimekeeper() {
            return new CountedFunc<DateTime>(() => DateTime.Now);
        }

        [Test]
        public void EverySecond() {
            var timeKeeper = new CountedFunc<DateTime>(() => DateTime.Now);
            var perSecond = new Refreshing<DateTime, DateTime>(
                () => timeKeeper.Invoke(),
                () => DateTime.Now,
                (prev, current) => current >= (prev + TimeSpan.FromSeconds(1))
            );

            Assert.That(timeKeeper.InvocationCount, Is.EqualTo(0));
            Assert.That(perSecond.Freshness,        Is.EqualTo(Freshness.Stale));

            Console.WriteLine(perSecond.Value);
            Assert.That(perSecond.Freshness,        Is.EqualTo(Freshness.Fresh));
            Assert.That(timeKeeper.InvocationCount, Is.EqualTo(1));

            int hertz   = 10;
            int seconds = 5;
            for (int s = 0; s < seconds; s++) {
                for (int i = 0; i < hertz; i++) {
                    Thread.Sleep(TimeSpan.FromSeconds(1f / hertz));
                    var v = perSecond.Value;
                    v = perSecond.Value;
                    v = perSecond.Value;
                    v = perSecond.Value;
                    v = perSecond.Value;

                    Assert.That(perSecond.Freshness, Is.EqualTo(Freshness.Fresh));
                }

                Console.WriteLine(perSecond.Value);
                Assert.That(timeKeeper.InvocationCount, Is.EqualTo(s + 2));
            }
        }

        [Test]
        public void MultipleSecondsButOnlyOneInvocation() {
            var invoker = new CountedFunc<string>(() => "swag: " + DateTime.Now);
            var perSecond = new Refreshing<string, DateTime>(
                invoker.Invoke,
                () => DateTime.Now,
                (prev, cur) => cur - prev >= TimeSpan.FromSeconds(1)
            );

            Assert.That(perSecond, Has.Property(nameof(perSecond.Freshness)).EqualTo(Freshness.Stale));
            Assert.That(invoker,   Has.Property(nameof(invoker.InvocationCount)).EqualTo(0));
        }

        [Test]
        public void PristineUntilFirstRetrieval() {
            const string str     = "yolo";
            var          invoker = new CountedFunc<string>(() => str);
            var refreshing = new Refreshing<string, long>(
                invoker.Invoke,
                () => DateTime.Now.Ticks,
                (prev, cur) => cur - prev >= TimeSpan.TicksPerSecond
            );

            Assert.That(invoker,    Has.Property(nameof(invoker.InvocationCount)).EqualTo(0));
            Assert.That(refreshing, Has.Property(nameof(refreshing.Freshness)).EqualTo(Freshness.Stale));

            10.Repeat(
                () => {
                    Assert.That(refreshing.Value, Is.EqualTo(str));
                    Assert.That(invoker,          Has.Property(nameof(invoker.InvocationCount)).EqualTo(1));
                    Assert.That(refreshing,       Has.Property(nameof(refreshing.Freshness)).EqualTo(Freshness.Fresh));
                }
            );
        }
    }
}