using Microsoft.CodeAnalysis.Text;

namespace NonboxingUnion.Generator.Parsing;

/// <summary>
/// A value-equatable snapshot of a <see cref="Location"/>. Roslyn's
/// <see cref="Location"/> holds references into the syntax tree, so carrying it
/// through the incremental pipeline would defeat caching. We capture only the
/// primitive pieces needed to reconstruct it when a diagnostic is reported.
/// </summary>
internal sealed record LocationInfo(string FilePath, TextSpan TextSpan, LinePositionSpan LineSpan)
{
    public Location ToLocation() => Location.Create(FilePath, TextSpan, LineSpan);

    public static LocationInfo? CreateFrom(SyntaxNode node) => CreateFrom(node.GetLocation());

    public static LocationInfo? CreateFrom(SyntaxToken token) => CreateFrom(token.GetLocation());

    public static LocationInfo? CreateFrom(Location location)
    {
        if (location.SourceTree is null)
        {
            return null;
        }

        return new LocationInfo(
            location.SourceTree.FilePath,
            location.SourceSpan,
            location.GetLineSpan().Span);
    }
}
