namespace NonboxingUnion.Generator.CodeGeneration.Extensions;

internal static class INamedTypeSymbolExtensions
{
    public static string GetAccessibilityString(this INamedTypeSymbol namedTypeSymbol)
    {
        return namedTypeSymbol.DeclaredAccessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            Accessibility.ProtectedAndInternal => "private protected",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.Private => "private",
            _ => "internal",
        };
    }

    public static bool TryGetTypeKindKeyword(this INamedTypeSymbol namedTypeSymbol, out string keyword)
    {
        keyword = namedTypeSymbol.TypeKind switch
        {
            TypeKind.Class => "class",
            TypeKind.Struct => "struct",
            TypeKind.Interface => "interface",
            _ => string.Empty,
        };

        return keyword.Length != 0;
    }
}
