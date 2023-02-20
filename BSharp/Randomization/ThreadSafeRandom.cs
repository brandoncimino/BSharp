using System;

namespace FowlFever.BSharp.Randomization;

/// <summary>
/// A simple <see cref="Random"/> implementation that locks itself whenever it is invoked, in order to promote thread-safety.
/// </summary>
/// <remarks>
/// This is <b>only</b> intended to be used in .NET versions prior to 6. In .NET 6+, use <a href="https://learn.microsoft.com/en-us/dotnet/api/System.Random.Shared">Random.Shared</a> instead. 
/// </remarks>
#if NET6_0_OR_GREATER
[Obsolete("In .NET 6+, use Random.Shared instead.", UrlFormat = "https://learn.microsoft.com/en-us/dotnet/api/System.Random.Shared")]
#endif
internal sealed class ThreadSafeRandom : Random {
    public ThreadSafeRandom() { }
    public ThreadSafeRandom(int seed) : base(seed) { }

    public override int Next() {
        lock (this) {
            return base.Next();
        }
    }

    public override int Next(int maxValue) {
        lock (this) {
            return base.Next();
        }
    }

    public override int Next(int minValue, int maxValue) {
        lock (this) {
            return base.Next(minValue, maxValue);
        }
    }

    public override void NextBytes(byte[] buffer) {
        lock (this) {
            base.NextBytes(buffer);
        }
    }

    public override void NextBytes(Span<byte> buffer) {
        lock (this) {
            base.NextBytes(buffer);
        }
    }

    public override double NextDouble() {
        lock (this) {
            return base.NextDouble();
        }
    }

#if NET6_0_OR_GREATER
    public override long NextInt64() {
        lock (this) {
            return base.NextInt64();
        }
    }

    public override long NextInt64(long maxValue) {
        lock (this) {
            return base.NextInt64(maxValue);
        }
    }

    public override long NextInt64(long minValue, long maxValue) {
        lock (this) {
            return base.NextInt64(minValue, maxValue);
        }
    }

    public override float NextSingle() {
        lock (this) {
            return base.NextSingle();
        }
    }
#endif

    protected override double Sample() {
        lock (this) {
            return base.Sample();
        }
    }
}