using System;

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
    Floor,
    /// <summary>
    /// AKA <see cref="Math.Ceiling(decimal)"/>
    /// </summary>
    Ceiling,
}

public static class RoundingDirectionExtensions {
    public static Rounder Rounder(this RoundingDirection roundingDirection) => roundingDirection switch {
        RoundingDirection.Ceiling => Enums.Rounder.Ceiling,
        RoundingDirection.Floor   => Enums.Rounder.Floor,
        _                         => throw BEnum.UnhandledSwitch(roundingDirection, nameof(roundingDirection), nameof(Rounder)),
    };

    public static double  ApplyTo(this RoundingDirection roundingDirection, double  value) => roundingDirection.Rounder().Round(value);
    public static float   ApplyTo(this RoundingDirection roundingDirection, float   value) => roundingDirection.Rounder().Round(value);
    public static decimal ApplyTo(this RoundingDirection roundingDirection, decimal value) => roundingDirection.Rounder().Round(value);
}

public abstract class Rounder {
    public abstract double  Round(double       value);
    public abstract decimal Round(decimal      value);
    public virtual  float   Round(float        value) => (float)Round((double)value);
    public virtual  int     RoundToInt(double  value) => (int)Round(value);
    public virtual  int     RoundToInt(float   value) => (int)Round(value);
    public virtual  int     RoundToInt(decimal value) => (int)Round(value);

    private class FloorRounder : Rounder {
        public override double  Round(double  value) => Math.Floor(value);
        public override decimal Round(decimal value) => Math.Floor(value);
    }

    public static readonly Rounder Floor = new FloorRounder();

    private class CeilingRounder : Rounder {
        public override double  Round(double  value) => Math.Ceiling(value);
        public override decimal Round(decimal value) => Math.Ceiling(value);
    }

    public static readonly Rounder Ceiling = new CeilingRounder();

    private class HalfEvenRounder : Rounder {
        public override double  Round(double  value) => Math.Round(value, MidpointRounding.ToEven);
        public override decimal Round(decimal value) => Math.Round(value, MidpointRounding.ToEven);
    }

    public static readonly Rounder HalfEven = new HalfEvenRounder();

    private class AwayFromZeroRounder : Rounder {
        public override double  Round(double  value) => Math.Round(value, MidpointRounding.AwayFromZero);
        public override decimal Round(decimal value) => Math.Round(value, MidpointRounding.AwayFromZero);
    }

    public static readonly Rounder AwayFromZero = new AwayFromZeroRounder();
}