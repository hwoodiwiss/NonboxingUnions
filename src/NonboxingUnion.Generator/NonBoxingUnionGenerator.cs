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
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if EMBED_MARKER_ATTRIBUTE
        // In the embedded build the marker attribute is synthesised directly into
        // the consumer's compilation (as an internal, compilation-scoped type) so
        // that no runtime assembly reference is required. It must be emitted from
        // post-initialization output for ForAttributeWithMetadataName to see it.
        context.RegisterPostInitializationOutput(static ctx =>
        {
            ctx.AddEmbeddedAttributeDefinition();
            ctx.AddSource(
                "NonBoxingUnionAttribute.g.cs",
                SourceText.From(
                    CodeSnippets.EmbeddedMarkerAttribute(AssemblyMetadata.Name, AssemblyMetadata.Version),
                    Encoding.UTF8));
        });
#endif

        IncrementalValuesProvider<UnionToGenerate?> unions = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                CodeGeneration.Constants.NonBoxingUnionAttributeName,
                predicate: static (node, _) => node is StructDeclarationSyntax,
                transform: static (ctx, _) => GetUnionToGenerate(ctx))
            .Where(static union => union is not null);

        context.RegisterSourceOutput(unions.Collect(),
            static (spc, source) => Execute(source, spc));
    }

    private static UnionToGenerate? GetUnionToGenerate(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetSymbol is not INamedTypeSymbol unionSymbol)
        {
            return null;
        }

        var structDeclaration = (StructDeclarationSyntax)context.TargetNode;
        if (structDeclaration.Modifiers.All(static m => !m.IsKind(SyntaxKind.PartialKeyword)))
        {
            return null;
        }

        var attribute = context.Attributes.FirstOrDefault();
        if (attribute is null)
        {
            return null;
        }

        var caseTypes = GetCaseTypes(attribute);
        if (caseTypes.Count == 0)
        {
            return null;
        }

        var variants = BuildVariants(caseTypes);
        if (variants.IsDefaultOrEmpty)
        {
            return null;
        }

        var containingTypes = GetContainingTypes(unionSymbol);
        var containingNamespace = unionSymbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : unionSymbol.ContainingNamespace.ToDisplayString();

        return new UnionToGenerate(
            containingNamespace,
            unionSymbol.GetAccessibilityString(),
            unionSymbol.Name,
            containingTypes,
            variants);
    }

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

            // Reference-type fields are left null for inactive cases, so they must
            // be declared nullable to satisfy nullable reference analysis.
            var storageTypeName = effectiveCaseType.IsReferenceType
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
                isNullableValueType));
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

    private static ImmutableArray<ContainingType> GetContainingTypes(INamedTypeSymbol unionSymbol)
    {
        var stack = new Stack<ContainingType>();
        var current = unionSymbol.ContainingType;
        while (current is not null)
        {
            stack.Push(new ContainingType(
                current.GetAccessibilityString(),
                current.IsStatic,
                current.GetTypeKindKeyword(),
                current.Name));
            current = current.ContainingType;
        }

        return [.. stack];
    }

    private static void Execute(ImmutableArray<UnionToGenerate?> unions, SourceProductionContext context)
    {
        if (unions.IsDefaultOrEmpty)
        {
            return;
        }

        foreach (var union in unions)
        {
            if (union is null)
            {
                continue;
            }

            var sourceText = GenerateUnion(union);
            var hintName = GetHintName(union);
            context.AddSource(hintName, SourceText.From(sourceText, Encoding.UTF8));
        }
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
