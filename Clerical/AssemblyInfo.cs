using System.Runtime.CompilerServices;

// This lets everything inside of another assembly access `internal` members.
// It should be placed next to the `.csproj` files.
[assembly: InternalsVisibleTo("BSharp.Tests")]
[assembly: InternalsVisibleTo("Clerical.Tests")]