using System.Collections.Immutable;

using FowlFever.BSharp.Memory;

namespace FowlFever.Clerical;

public readonly record struct DirectoryPath(ImmutableArray<PathPart> Parts, DirectorySeparator DirectorySeparator = DirectorySeparator.Universal)
#if NET7_0_OR_GREATER
    : System.Numerics.IEqualityOperators<DirectoryPath, DirectoryPath, bool>,
      System.Numerics.IAdditionOperators<DirectoryPath, DirectoryPath, DirectoryPath>,
      System.Numerics.IAdditionOperators<DirectoryPath, PathPart, DirectoryPath>,
      System.Numerics.IAdditionOperators<DirectoryPath, FileName, FilePath>,
      System.Numerics.IAdditionOperators<DirectoryPath, FilePath, FilePath>,
      System.Numerics.IDecrementOperators<DirectoryPath>
#endif
{
    public bool IsEmpty => Parts.IsDefaultOrEmpty;

    public static DirectoryPath operator +(DirectoryPath left, DirectoryPath right) {
        if (right.IsEmpty) {
            return left;
        }

        if (left.IsEmpty) {
            return right;
        }

        return left with {
            Parts = right.Parts.AddRange(right.Parts)
        };
    }

    public static DirectoryPath operator +(DirectoryPath left, PathPart right) {
        if (right.Length == 0) {
            return left;
        }

        return left with {
            Parts = left.Parts.Add(right)
        };
    }

    public static FilePath operator +(DirectoryPath left, FileName right) {
        return new FilePath(left, right);
    }

    public int Length {
        get {
            var sum = 0;

            foreach (var p in Parts) {
                sum += p.Length;
            }

            var separatorCount = Parts.Length - 1;
            if (separatorCount > 0) {
                sum += separatorCount;
            }

            return sum;
        }
    }

    public override string ToString() {
        return string.Create(
            Length,
            this,
            (span, path) => {
                var pos = 0;
                for (int i = 0; i < path.Parts.Length; i++) {
                    if (i > 0) {
                        span.Write(path.DirectorySeparator.ToChar(), ref pos);
                    }

                    span.Write(path.Parts[i].AsSpan(), ref pos);
                }
            }
        );
    }

    public DirectoryPath Parent {
        get {
            if (Parts.Length <= 1) {
                return default;
            }

            return this with {
                Parts = Parts[..^1]
            };
        }
    }

    public static DirectoryPath operator --(DirectoryPath value) {
        return value.Parent;
    }

    public static FilePath operator +(DirectoryPath left, FilePath right) {
        return right with {
            Directory = left + right.Directory
        };
    }
}