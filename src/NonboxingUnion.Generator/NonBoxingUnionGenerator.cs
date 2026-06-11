using System.Collections.Immutable;
using System.Text;

using NonboxingUnion.Generator.CodeGeneration;
using NonboxingUnion.Generator.CodeGeneration.Extensions;
using NonboxingUnion.Generator.Extensions;
using NonboxingUnion.Generator.Parsing;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace NonboxingUnion.Generator;

[Generator]
public class NonBoxingUnionGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Tracking name for the step that turns a marked struct into the equatable
    /// <see cref="UnionToGenerate"/> model. Exposed so incremental-caching tests can
    /// assert this step is reused (rather than re-run) for unrelated edits.
    /// </summary>
    public const string GetUnionStep = "GetUnion";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<UnionParseResult> parseResults = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                CodeGeneration.Constants.NonBoxingUnionAttributeName,
                predicate: static (node, _) => node is StructDeclarationSyntax,
                transform: static (ctx, _) => Parse(ctx));

        // Diagnostics describing invalid unions are emitted from a dedicated output so the
        // extraction stage stays free of Diagnostic/Location references (which would break
        // incremental caching).
        context.RegisterSourceOutput(
            parseResults.SelectMany(static (result, _) => result.Diagnostics),
            static (spc, diagnostic) => spc.ReportDiagnostic(diagnostic.ToDiagnostic()));

        // Register the output per-union (rather than over a Collect()ed array) so that
        // editing or adding one union does not invalidate the generated output of the
        // others. Each union already produces an independently named source file.
        IncrementalValuesProvider<UnionToGenerate> unions = parseResults
            .Select(static (result, _) => result.Union)
            .Where(static union => union is not null)
            .Select(static (union, _) => union!)
            .WithTrackingName(GetUnionStep);

        context.RegisterSourceOutput(unions,
            static (spc, union) => Execute(union, spc));
    }

    private static UnionParseResult Parse(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetSymbol is not INamedTypeSymbol unionSymbol
            || context.TargetNode is not StructDeclarationSyntax structDeclaration)
        {
            return UnionParseResult.None;
        }

        if (structDeclaration.Modifiers.All(static m => !m.IsKind(SyntaxKind.PartialKeyword)))
        {
            return Diagnostic(
                DiagnosticDescriptors.UnionMustBePartial,
                LocationInfo.CreateFrom(structDeclaration.Identifier),
                unionSymbol.Name);
        }

        var attribute = context.Attributes.FirstOrDefault();
        if (attribute is null)
        {
            return UnionParseResult.None;
        }

        // Variants = type parameters (in declaration order) + any concrete types from the attribute.
        // This lets callers write [NonBoxingUnion] on a generic struct to use only the type
        // parameters as variants, or mix in concrete types via typeof(...).
        var caseTypes = new List<ITypeSymbol>(unionSymbol.TypeParameters.Length);
        foreach (var typeParam in unionSymbol.TypeParameters)
        {
            caseTypes.Add(typeParam);
        }

        caseTypes.AddRange(GetCaseTypes(attribute));

        if (caseTypes.Count == 0)
        {
            return Diagnostic(
                DiagnosticDescriptors.UnionMustDeclareCase,
                GetAttributeLocation(attribute) ?? LocationInfo.CreateFrom(structDeclaration.Identifier),
                unionSymbol.Name);
        }

        var variants = BuildVariants(caseTypes);
        if (variants.IsDefaultOrEmpty)
        {
            return UnionParseResult.None;
        }

        if (!TryGetContainingTypes(unionSymbol, out var containingTypes, out var unsupportedContainingType))
        {
            return Diagnostic(
                DiagnosticDescriptors.UnsupportedContainingType,
                LocationInfo.CreateFrom(structDeclaration.Identifier),
                unsupportedContainingType);
        }

        var containingNamespace = unionSymbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : unionSymbol.ContainingNamespace.ToDisplayString();
        var typeParameters = GetTypeParameters(unionSymbol);

        var union = new UnionToGenerate(
            containingNamespace,
            unionSymbol.GetAccessibilityString(),
            unionSymbol.Name,
            containingTypes,
            variants,
            typeParameters);

        return new UnionParseResult(union, ImmutableArray<DiagnosticInfo>.Empty);
    }

    private static UnionParseResult Diagnostic(
        DiagnosticDescriptor descriptor,
        LocationInfo? location,
        string? messageArgument)
        => new(null, [new DiagnosticInfo(descriptor, location, messageArgument)]);

    private static LocationInfo? GetAttributeLocation(AttributeData attribute)
        => attribute.ApplicationSyntaxReference is { } reference
            ? LocationInfo.CreateFrom(reference.GetSyntax().GetLocation())
            : null;

    private static List<ITypeSymbol> GetCaseTypes(AttributeData attribute)
    {
        var caseTypes = new List<ITypeSymbol>();

        // The attribute takes a single params Type[] parameter.
        if (attribute.ConstructorArguments.Length != 1)
        {
            return caseTypes;
        }

        var argument = attribute.ConstructorArguments[0];
        if (argument.Kind != TypedConstantKind.Array)
        {
            return caseTypes;
        }

        foreach (var value in argument.Values)
        {
            if (value.Value is ITypeSymbol typeSymbol)
            {
                caseTypes.Add(typeSymbol);
            }
        }

        return caseTypes;
    }

    private static ImmutableArray<UnionVariant> BuildVariants(List<ITypeSymbol> caseTypes)
    {
        var builder = ImmutableArray.CreateBuilder<UnionVariant>(caseTypes.Count);
        var usedMemberNames = new HashSet<string>(StringComparer.Ordinal);
        var index = 0;

        foreach (var caseType in caseTypes)
        {
            index++;

            var isNullableValueType = caseType.TryGetNullableUnderlyingType(out var underlyingType);
            var effectiveCaseType = isNullableValueType ? underlyingType : caseType;

            // Per the union spec, a nullable value-type case collapses to its
            // underlying type for both the case type and the TryGetValue out parameter.
            var caseTypeName = effectiveCaseType.GetFullyQualifiedTypeName();
            var fullyQualifiedStorageType = caseType.GetFullyQualifiedTypeName();

            // An unconstrained type parameter (no struct/class constraint) could be either
            // a reference or value type at the call site.  Storing it as T? (MaybeNull
            // annotation in C# 8+, NOT Nullable<T>) means:
            //   - value-type T: no boxing, zero runtime overhead
            //   - reference-type T: behaves like a nullable reference
            // This prevents CS8618 in constructors that don't set the field.
            var isUnconstrainedTypeParam = effectiveCaseType is ITypeParameterSymbol
            {
                HasValueTypeConstraint: false,
                HasReferenceTypeConstraint: false
            };

            // Nullable storage is needed whenever the field may be left null for inactive
            // cases: reference types and unconstrained type parameters.
            var hasNullableStorage = effectiveCaseType.IsReferenceType || isUnconstrainedTypeParam;

            var storageTypeName = hasNullableStorage
                ? $"{fullyQualifiedStorageType}?"
                : fullyQualifiedStorageType;

            var constructorParameterType = fullyQualifiedStorageType;

            var memberName = MakeMemberName(effectiveCaseType, usedMemberNames);
            var fieldName = $"_{char.ToLowerInvariant(memberName[0])}{memberName.Substring(1)}";

            builder.Add(new UnionVariant(
                caseTypeName,
                constructorParameterType,
                storageTypeName,
                fieldName,
                memberName,
                effectiveCaseType.IsReferenceType,
                isNullableValueType,
                hasNullableStorage));
        }

        return builder.MoveToImmutable();
    }

    private static string MakeMemberName(ITypeSymbol caseType, HashSet<string> usedMemberNames)
    {
        var baseName = SanitizeIdentifier(caseType.Name);
        if (string.IsNullOrEmpty(baseName))
        {
            baseName = "Case";
        }

        var candidate = baseName;
        var suffix = 1;
        while (!usedMemberNames.Add(candidate))
        {
            suffix++;
            candidate = $"{baseName}{suffix}";
        }

        return candidate;
    }

    private static string SanitizeIdentifier(string name)
    {
        var builder = new StringBuilder(name.Length);
        foreach (var ch in name)
        {
            if (char.IsLetterOrDigit(ch) || ch == '_')
            {
                builder.Append(ch);
            }
        }

        if (builder.Length == 0)
        {
            return string.Empty;
        }

        if (char.IsDigit(builder[0]))
        {
            builder.Insert(0, '_');
        }

        builder[0] = char.ToUpperInvariant(builder[0]);
        return builder.ToString();
    }

    private static bool TryGetContainingTypes(
        INamedTypeSymbol unionSymbol,
        out ImmutableArray<ContainingType> containingTypes,
        out string? unsupportedContainingType)
    {
        var stack = new Stack<ContainingType>();
        var current = unionSymbol.ContainingType;
        while (current is not null)
        {
            if (!current.TryGetTypeKindKeyword(out var keyword))
            {
                containingTypes = ImmutableArray<ContainingType>.Empty;
                unsupportedContainingType = current.Name;
                return false;
            }

            stack.Push(new ContainingType(
                current.GetAccessibilityString(),
                current.IsStatic,
                keyword,
                current.Name));
            current = current.ContainingType;
        }

        containingTypes = [.. stack];
        unsupportedContainingType = null;
        return true;
    }

    private static ImmutableArray<TypeParameter> GetTypeParameters(INamedTypeSymbol unionSymbol)
    {
        if (unionSymbol.TypeParameters.IsEmpty)
        {
            return ImmutableArray<TypeParameter>.Empty;
        }

        var builder = ImmutableArray.CreateBuilder<TypeParameter>(unionSymbol.TypeParameters.Length);
        foreach (var typeParam in unionSymbol.TypeParameters)
        {
            builder.Add(new TypeParameter(typeParam.Name, BuildConstraintClause(typeParam)));
        }

        return builder.MoveToImmutable();
    }

    /// <summary>
    /// Builds the <c>where T : ...</c> clause string for a type parameter, or an
    /// empty string when the parameter is unconstrained.
    /// The order of constraints follows the C# grammar: special constraints
    /// (struct/class/notnull/unmanaged) first, then type constraints, then new().
    /// </summary>
    private static string BuildConstraintClause(ITypeParameterSymbol typeParam)
    {
        var constraints = new List<string>();

        // Primary constraint: struct, class, class?, notnull, or unmanaged.
        if (typeParam.HasValueTypeConstraint)
        {
            // 'unmanaged' implies 'struct'; Roslyn sets HasValueTypeConstraint for both.
            constraints.Add(typeParam.HasUnmanagedTypeConstraint ? "unmanaged" : "struct");
        }
        else if (typeParam.HasReferenceTypeConstraint)
        {
            constraints.Add("class");
        }
        else if (typeParam.HasNotNullConstraint)
        {
            constraints.Add("notnull");
        }

        // Secondary constraints: base class and interface constraints.
        foreach (var constraintType in typeParam.ConstraintTypes)
        {
            constraints.Add(constraintType.GetFullyQualifiedTypeName());
        }

        // Constructor constraint must be last.
        if (typeParam.HasConstructorConstraint)
        {
            constraints.Add("new()");
        }

        return constraints.Count == 0
            ? string.Empty
            : $"where {typeParam.Name} : {string.Join(", ", constraints)}";
    }

    private static void Execute(UnionToGenerate union, SourceProductionContext context)
    {
        var sourceText = GenerateUnion(union);
        var hintName = GetHintName(union);
        context.AddSource(hintName, SourceText.From(sourceText, Encoding.UTF8));
    }

    private static string GenerateUnion(UnionToGenerate union)
    {
        var sb = new StringBuilder();
        sb.Append(CodeSnippets.NonBoxingUnionFileHeader);

        var hierarchyBuilder = new UnionContainingHierarchyBuilder(sb);
        hierarchyBuilder.AddContainingHierarchy(union.Namespace, union.ContainingTypes);

        var unionBuilder = new UnionBuilder(sb, hierarchyBuilder.CurrentIndentationLevel);
        unionBuilder
            .AddTypeDeclaration(union)
            .AddDiscriminator(union)
            .AddConstructors(union)
            .AddValueProperty(union)
            .AddHasValueProperty()
            .AddTryGetValueMethods(union)
            .AddEqualityMembers(union)
            .EndType();

        hierarchyBuilder.CloseContainingHierarchy();

        return sb.ToString();
    }

    private static string GetHintName(UnionToGenerate union)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(union.Namespace))
        {
            sb.Append(union.Namespace).Append('.');
        }

        foreach (var containingType in union.ContainingTypes)
        {
            sb.Append(containingType.Name).Append('.');
        }

        sb.Append(union.Name).Append(".g.cs");
        return sb.ToString();
    }
}
