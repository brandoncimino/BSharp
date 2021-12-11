using System;

using BrandonUtils.Testing;

using FowlFever.BSharp.Chronic;

using Newtonsoft.Json;

using NUnit.Framework;

namespace BrandonUtils.Tests.Standalone.Chronic {
    public class TimePeriodTests {
        [Test]
        public void SerializeTimePeriod() {
            var period     = new TimePeriod(DateTime.Now, DateTime.Now.Add(TimeSpan.FromMinutes(1)));
            var simulacrum = TestUtils.SerialCompare(period);
            Console.WriteLine($"Final simulacrum: {simulacrum}");
            Console.WriteLine($"json: {JsonConvert.SerializeObject(simulacrum)}");
            Assert.True(period == simulacrum, "period == simulacrum");
        }
    }
}