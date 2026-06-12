namespace NonboxingUnion.Generator.Parsing;

internal static class DiagnosticDescriptors
{
    private const string Category = "NonboxingUnion";

    public static readonly DiagnosticDescriptor UnionMustBePartial = new(
        id: "NBU001",
        title: "Non-boxing union must be partial",
        messageFormat: "The non-boxing union '{0}' must be declared 'partial' for source generation to run",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor UnionMustDeclareCase = new(
        id: "NBU002",
        title: "Non-boxing union must declare at least one case",
        messageFormat: "The non-boxing union '{0}' must declare at least one case type in its [NonBoxingUnion] attribute",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor UnsupportedContainingType = new(
        id: "NBU003",
        title: "Non-boxing union is nested in an unsupported type",
        messageFormat: "The non-boxing union cannot be generated because it is nested in '{0}', which is not a class, struct, or interface",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
