using System.Runtime.CompilerServices;

// This lets everything inside of another assembly access `internal` members.
// This file should live in the same directory as an `.asmdef` file.
// The `assemblyName` should refer to the `name:` field in another `.asmdef` file.
[assembly: InternalsVisibleTo("BrandonUtils.Tests.BSharp")]
[assembly: InternalsVisibleTo("BrandonUtils.Tests.Standalone")]
[assembly: InternalsVisibleTo("BrandonUtils.Tests.PlayMode")]
[assembly: InternalsVisibleTo("BrandonUtils.Tests.EditMode")]