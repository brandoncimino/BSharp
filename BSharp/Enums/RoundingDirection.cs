using System;
using System.Collections.Generic;

using FowlFever.BSharp.Exceptions;

using Humanizer;

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
    public static double  Round(this RoundingDirection roundingDirection, double  value) => Rounder.Get(roundingDirection).Round(value);
    public static float   Round(this RoundingDirection roundingDirection, float   value) => Rounder.Get(roundingDirection).Round(value);
    public static decimal Round(this RoundingDirection roundingDirection, decimal value) => Rounder.Get(roundingDirection).Round(value);

    public static RoundingDirection Complement(this RoundingDirection roundingDirection) => roundingDirection switch {
        RoundingDirection.Floor   => RoundingDirection.Ceiling,
        RoundingDirection.Ceiling => RoundingDirection.Floor,
        _                         => throw BEnum.UnhandledSwitch(roundingDirection, nameof(roundingDirection), nameof(Complement)),
    };
}

public abstract class Rounder {
    public abstract double  Round(double       value);
    public abstract decimal Round(decimal      value);
    public virtual  float   Round(float        value) => (float)Round((double)value);
    public virtual  int     RoundToInt(double  value) => (int)Round(value);
    public virtual  int     RoundToInt(float   value) => (int)Round(value);
    public virtual  int     RoundToInt(decimal value) => (int)Round(value);

    public static Rounder Get(object roundingStyle) {
        Must.NotBeNull(roundingStyle, nameof(roundingStyle), nameof(Get));

        if (Rounders.TryGetValue(roundingStyle, out var rounder)) {
            return rounder;
        }

        throw new ArgumentException($"[{roundingStyle.GetType()}]{roundingStyle} was not a valid rounding style!");
    }

    private static readonly Dictionary<object, Rounder> Rounders = new() {
        [RoundingDirection.Ceiling]     = new CeilingRounder(),
        [RoundingDirection.Floor]       = new FloorRounder(),
        [MidpointRounding.ToEven]       = new HalfEvenRounder(),
        [MidpointRounding.AwayFromZero] = new AwayFromZeroRounder(),
    };

    private class FloorRounder : Rounder {
        public override double  Round(double  value) => Math.Floor(value);
        public override decimal Round(decimal value) => Math.Floor(value);
    }

    private class CeilingRounder : Rounder {
        public override double  Round(double  value) => Math.Ceiling(value);
        public override decimal Round(decimal value) => Math.Ceiling(value);
    }

    private class HalfEvenRounder : Rounder {
        public override double  Round(double  value) => Math.Round(value, MidpointRounding.ToEven);
        public override decimal Round(decimal value) => Math.Round(value, MidpointRounding.ToEven);
    }

    private class AwayFromZeroRounder : Rounder {
        public override double  Round(double  value) => Math.Round(value, MidpointRounding.AwayFromZero);
        public override decimal Round(decimal value) => Math.Round(value, MidpointRounding.AwayFromZero);
    }
}