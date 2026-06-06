using System.Collections.Immutable;

namespace NonboxingUnion.Generator.Parsing;

/// <summary>
/// The outcome of inspecting a single <c>[NonBoxingUnion]</c> target. Carries
/// either the extracted <see cref="UnionToGenerate"/> model or the diagnostics
/// that explain why nothing can be generated. Both are value-equatable so the
/// incremental pipeline can cache them.
/// </summary>
internal sealed record UnionParseResult
{
    public static readonly UnionParseResult None =
        new(null, ImmutableArray<DiagnosticInfo>.Empty);

    public UnionParseResult(UnionToGenerate? union, ImmutableArray<DiagnosticInfo> diagnostics)
    {
        Union = union;
        Diagnostics = diagnostics;
    }

    public UnionToGenerate? Union { get; }

    public ImmutableArray<DiagnosticInfo> Diagnostics { get; }

    public bool Equals(UnionParseResult? other)
    {
        if (other is null)
        {
            return false;
        }

        return Equals(Union, other.Union)
            && Diagnostics.SequenceEqual(other.Diagnostics);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = (hash * 31) + (Union?.GetHashCode() ?? 0);
            foreach (var diagnostic in Diagnostics)
            {
                hash = (hash * 31) + diagnostic.GetHashCode();
            }

            return hash;
        }
    }
}
