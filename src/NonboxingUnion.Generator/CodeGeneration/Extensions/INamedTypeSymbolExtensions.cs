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

    public static string GetTypeKindKeyword(this INamedTypeSymbol namedTypeSymbol)
    {
        return namedTypeSymbol.TypeKind switch
        {
            TypeKind.Class => "class",
            TypeKind.Struct => "struct",
            TypeKind.Interface => "interface",
            _ => throw new InvalidOperationException("A non-boxing union must be contained within a class, struct or interface."),
        };
    }
}
