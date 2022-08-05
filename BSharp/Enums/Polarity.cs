namespace FowlFever.BSharp.Enums;

public enum Polarity { Positive, Negative }

public static class PolarityExtensions {
    public static bool ToBool(this Polarity polarity) => polarity switch {
        Polarity.Positive => true,
        Polarity.Negative => false,
        _                 => throw BEnum.UnhandledSwitch(polarity),
    };

    public static int ToSign(this Polarity polarity) => polarity switch {
        Polarity.Positive => 1,
        Polarity.Negative => -1,
        _                 => throw BEnum.UnhandledSwitch(polarity)
    };

    public static Polarity Negate(this Polarity polarity) => polarity switch {
        Polarity.Positive => Polarity.Negative,
        Polarity.Negative => Polarity.Positive,
        _                 => throw BEnum.UnhandledSwitch(polarity)
    };
}