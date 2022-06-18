using FowlFever.BSharp.Exceptions;

using NUnit.Framework.Constraints;

namespace FowlFever.Testing.NUnitExtensionPoints;

public abstract class Throws : NUnit.Framework.Throws {
    public static InstanceOfTypeConstraint RejectionException => InstanceOf<RejectionException>();
}