namespace NonboxingUnion.Generator.Extensions;

internal static class ITypeSymbolExtensions
{
    private static readonly SymbolDisplayFormat FullyQualifiedFormat = SymbolDisplayFormat.FullyQualifiedFormat
        .WithMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.None);

    /// <summary>
    /// Returns the globally-qualified type name without nullable reference annotations.
    /// Nullable value types (<c>Nullable&lt;T&gt;</c>) are preserved as written.
    /// </summary>
    public static string GetFullyQualifiedTypeName(this ITypeSymbol typeSymbol)
        => typeSymbol.WithNullableAnnotation(NullableAnnotation.NotAnnotated)
            .ToDisplayString(FullyQualifiedFormat);

    /// <summary>
    /// Determines whether the type is a nullable value type (<c>Nullable&lt;T&gt;</c>),
    /// and if so returns the underlying value type.
    /// </summary>
    public static bool TryGetNullableUnderlyingType(this ITypeSymbol typeSymbol, out ITypeSymbol underlyingType)
    {
        if (typeSymbol is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } named
            && named.TypeArguments.Length == 1)
        {
            underlyingType = named.TypeArguments[0];
            return true;
        }

        underlyingType = typeSymbol;
        return false;
    }

    public static bool IsReferenceType(this ITypeSymbol typeSymbol)
        => typeSymbol.IsReferenceType;
}
