// Global using directives

global using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

// Ensures that the `System.Numerics` namespace - used by the "Generic Math" interfaces like `System.Numerics.IEqualityOperators<,,>` -
// is available even if all of the code that uses it gets omitted by conditional-compilation.
global using System.Numerics;