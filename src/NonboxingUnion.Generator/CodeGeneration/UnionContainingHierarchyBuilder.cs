using System.Collections.Immutable;
using System.Text;

using NonboxingUnion.Generator.Parsing;

namespace NonboxingUnion.Generator.CodeGeneration;

internal sealed class UnionContainingHierarchyBuilder
{
    private int _hierarchyLevelCount;
    private readonly Dictionary<int, string> _indentationCache = new();
    private readonly StringBuilder _builder;

    public int CurrentIndentationLevel { get; private set; }

    public UnionContainingHierarchyBuilder(StringBuilder builder)
    {
        _builder = builder;
        CurrentIndentationLevel = 0;
    }

    public UnionContainingHierarchyBuilder AddContainingHierarchy(string containingNamespace, ImmutableArray<ContainingType> containingTypes)
    {
        if (!string.IsNullOrEmpty(containingNamespace))
        {
            AddContainingNamespace(containingNamespace);
        }

        foreach (var containingType in containingTypes)
        {
            AddContainingType(containingType);
        }

        return this;
    }

    private void AddContainingNamespace(string containingNamespace)
    {
        _builder.AppendLine($"namespace {containingNamespace}");
        _builder.AppendLine("{");
        _hierarchyLevelCount++;
        CurrentIndentationLevel += Constants.IndentationPerLevel;
    }

    private void AddContainingType(ContainingType containingType)
    {
        _hierarchyLevelCount++;

        var indentation = GetOrCreateIndentationLevel(CurrentIndentationLevel);
        var staticModifier = containingType.IsStatic ? "static " : string.Empty;

        _builder.AppendLine($"{indentation}{containingType.Accessibility} {staticModifier}partial {containingType.Keyword} {containingType.Name}");
        _builder.AppendLine($"{indentation}{{");

        CurrentIndentationLevel += Constants.IndentationPerLevel;
    }

    public UnionContainingHierarchyBuilder CloseContainingHierarchy()
    {
        for (var i = 1; i <= _hierarchyLevelCount; i++)
        {
            var indentation = GetOrCreateIndentationLevel(CurrentIndentationLevel - (i * Constants.IndentationPerLevel));
            _builder.AppendLine($"{indentation}}}");
        }

        return this;
    }

    private string GetOrCreateIndentationLevel(int indentationLevel)
    {
        if (_indentationCache.TryGetValue(indentationLevel, out var indentation))
        {
            return indentation;
        }

        var indentationString = new string(' ', indentationLevel);
        _indentationCache.Add(indentationLevel, indentationString);
        return indentationString;
    }
}
