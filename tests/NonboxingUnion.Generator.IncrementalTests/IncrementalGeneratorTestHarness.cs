using System.Collections.Immutable;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using NonboxingUnion;

namespace NonboxingUnion.Generator.IncrementalTests;

/// <summary>
/// Helpers for driving <see cref="NonBoxingUnionGenerator"/> through a
/// <see cref="GeneratorDriver"/> with incremental step tracking enabled, so that tests
/// can assert which pipeline steps are reused (cached) versus re-run between compilations.
/// </summary>
internal static class IncrementalGeneratorTestHarness
{
    private static readonly GeneratorDriverOptions TrackingOptions = new(
        disabledOutputs: IncrementalGeneratorOutputKind.None,
        trackIncrementalGeneratorSteps: true);

    /// <summary>
    /// The metadata references every test compilation needs: the currently loaded runtime
    /// assemblies plus the assembly that defines <see cref="NonBoxingUnionAttribute"/>.
    /// </summary>
    private static readonly ImmutableArray<MetadataReference> References = BuildReferences();

    public static CSharpCompilation CreateCompilation(params string[] sources)
    {
        var syntaxTrees = sources
            .Select(static source => CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default))
            .ToArray();

        return CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: syntaxTrees,
            references: References,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    public static GeneratorDriver CreateDriver()
    {
        var generator = new NonBoxingUnionGenerator().AsSourceGenerator();
        return CSharpGeneratorDriver.Create(
            generators: [generator],
            additionalTexts: [],
            parseOptions: CSharpParseOptions.Default,
            optionsProvider: null,
            driverOptions: TrackingOptions);
    }

    /// <summary>
    /// Returns the run reasons for every output of a named, tracked pipeline step in the most
    /// recent run. One entry per tracked value flowing through the step.
    /// </summary>
    public static ImmutableArray<IncrementalStepRunReason> StepReasons(GeneratorDriver driver, string stepName)
    {
        var result = driver.GetRunResult().Results[0];
        if (!result.TrackedSteps.TryGetValue(stepName, out var steps))
        {
            return [];
        }

        return steps
            .SelectMany(static step => step.Outputs)
            .Select(static output => output.Reason)
            .ToImmutableArray();
    }

    private static ImmutableArray<MetadataReference> BuildReferences()
    {
        var builder = ImmutableArray.CreateBuilder<MetadataReference>();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
            {
                builder.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
        }

        var attributeLocation = typeof(NonBoxingUnionAttribute).Assembly.Location;
        if (!builder.Any(reference => reference.Display == attributeLocation))
        {
            builder.Add(MetadataReference.CreateFromFile(attributeLocation));
        }

        return builder.ToImmutable();
    }
}
