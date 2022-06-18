using System;
using System.Collections.Generic;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Enums;

/// <summary>
/// Decides whether to round using <see cref="Math.Ceiling(decimal)"/> or <see cref="Math.Floor(decimal)"/>.
/// </summary>
/// <remarks>
/// Sort of a complement to <see cref="MidpointRounding"/>?
/// </remarks>
public enum RoundingDirection {
    /// <summary>
    /// AKA <see cref="Math.Floor(decimal)"/>
    /// </summary>
    Floor = default,
    /// <summary>
    /// AKA <see cref="Math.Ceiling(decimal)"/>
    /// </summary>
    Ceiling,
}

public static class RoundingDirectionExtensions {
    public static double  Round(this RoundingDirection roundingDirection, double  value) => RounderSource.Get(roundingDirection).Round(value);
    public static float   Round(this RoundingDirection roundingDirection, float   value) => RounderSource.Get(roundingDirection).Round(value);
    public static decimal Round(this RoundingDirection roundingDirection, decimal value) => RounderSource.Get(roundingDirection).Round(value);

    public static RoundingDirection Complement(this RoundingDirection roundingDirection) => roundingDirection switch {
        RoundingDirection.Floor   => RoundingDirection.Ceiling,
        RoundingDirection.Ceiling => RoundingDirection.Floor,
        _                         => throw BEnum.UnhandledSwitch(roundingDirection, nameof(roundingDirection), nameof(Complement)),
    };
}

public readonly record struct Rounder {
    private readonly RounderSource _rounderSource = RounderSource.HalfEven;
    private          RounderSource RounderSource => _rounderSource ?? RounderSource.HalfEven;

    public static readonly Rounder HalfEven     = new(RounderSource.Get(MidpointRounding.ToEven));
    public static readonly Rounder AwayFromZero = new(RounderSource.Get(MidpointRounding.AwayFromZero));
    public static readonly Rounder Ceiling      = new(RounderSource.Get(RoundingDirection.Ceiling));
    public static readonly Rounder Floor        = new(RounderSource.Get(RoundingDirection.Floor));

    public Rounder() {
        _rounderSource = RounderSource.HalfEven;
    }

    private Rounder(RounderSource rounderSource) {
        _rounderSource = rounderSource.MustNotBeNull();
    }

    public Rounder(MidpointRounding  midpointRounding) : this(RounderSource.Get(midpointRounding).MustNotBeNull()) { }
    public Rounder(RoundingDirection roundingDirection) : this(RounderSource.Get(roundingDirection).MustNotBeNull()) { }

    public static implicit operator Rounder(MidpointRounding  midpointRounding)  => new(midpointRounding);
    public static implicit operator Rounder(RoundingDirection roundingDirection) => new(roundingDirection);

    public double  Round(double       value) => RounderSource.Round(value);
    public float   Round(float        value) => RounderSource.RoundToInt(value);
    public decimal Round(decimal      value) => RounderSource.Round(value);
    public int     RoundToInt(double  value) => RounderSource.MustNotBeNull().RoundToInt(value);
    public int     RoundToInt(float   value) => RounderSource.RoundToInt(value);
    public int     RoundToInt(decimal value) => RounderSource.RoundToInt(value);
}

internal abstract class RounderSource {
    public abstract double  Round(double       value);
    public abstract decimal Round(decimal      value);
    public virtual  float   Round(float        value) => (float)Round((double)value);
    public virtual  int     RoundToInt(double  value) => (int)Round(value);
    public virtual  int     RoundToInt(float   value) => (int)Round(value);
    public virtual  int     RoundToInt(decimal value) => (int)Round(value);

    public static RounderSource Find<T>(T flavor)
        where T : struct, Enum {
        return flavor switch {
            MidpointRounding.ToEven       => HalfEven,
            MidpointRounding.AwayFromZero => AwayFromZero,
            RoundingDirection.Ceiling     => Ceiling,
            RoundingDirection.Floor       => Floor,
            _                             => throw BEnum.UnhandledSwitch(flavor),
        };
    }

    public static RounderSource Get(object roundingStyle) {
        Must.NotBeNull(roundingStyle, nameof(roundingStyle), nameof(Get));

        if (Rounders.TryGetValue(roundingStyle, out var rounder)) {
            return rounder.MustNotBeNull();
        }

        throw new ArgumentException($"[{roundingStyle.GetType()}]{roundingStyle} was not a valid rounding style!");
    }

    public static readonly RounderSource Floor        = new FloorRounderSource();
    public static readonly RounderSource Ceiling      = new CeilingRounderSource();
    public static readonly RounderSource HalfEven     = new HalfEvenRounderSource();
    public static readonly RounderSource AwayFromZero = new AwayFromZeroRounderSource();

    private static readonly Dictionary<object, RounderSource> Rounders = new() {
        [RoundingDirection.Ceiling]     = Ceiling,
        [RoundingDirection.Floor]       = Floor,
        [MidpointRounding.ToEven]       = HalfEven,
        [MidpointRounding.AwayFromZero] = AwayFromZero,
    };

    private sealed class FloorRounderSource : RounderSource {
        public override double  Round(double  value) => Math.Floor(value);
        public override decimal Round(decimal value) => Math.Floor(value);
    }

    private sealed class CeilingRounderSource : RounderSource {
        public override double  Round(double  value) => Math.Ceiling(value);
        public override decimal Round(decimal value) => Math.Ceiling(value);
    }

    private sealed class HalfEvenRounderSource : RounderSource {
        public override double  Round(double  value) => Math.Round(value, MidpointRounding.ToEven);
        public override decimal Round(decimal value) => Math.Round(value, MidpointRounding.ToEven);
    }

    private sealed class AwayFromZeroRounderSource : RounderSource {
        public override double  Round(double  value) => Math.Round(value, MidpointRounding.AwayFromZero);
        public override decimal Round(decimal value) => Math.Round(value, MidpointRounding.AwayFromZero);
    }
}