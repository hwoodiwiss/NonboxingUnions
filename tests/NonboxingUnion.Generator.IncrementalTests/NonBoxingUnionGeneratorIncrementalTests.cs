using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace NonboxingUnion.Generator.IncrementalTests;

/// <summary>
/// Verifies that <see cref="NonBoxingUnionGenerator"/> is genuinely incremental: the
/// (expensive) semantic step that builds the union model must be reused across compilations
/// when nothing relevant changed, and must re-run only when the union itself changes.
///
/// These tests catch the classic "looks incremental but isn't" bug, where the model returned
/// from the transform does not implement value equality (e.g. it captures an
/// <c>ISymbol</c>/<c>SyntaxNode</c>), so every edit produces a model that compares unequal to
/// the cached one and the whole pipeline re-runs as if from scratch.
/// </summary>
public class NonBoxingUnionGeneratorIncrementalTests
{
    private const string PetUnionSource =
        """
        using NonboxingUnion;

        namespace Sample
        {
            public sealed class Dog { }
            public sealed class Cat { }

            [NonBoxingUnion(typeof(Dog), typeof(Cat))]
            public partial struct Pet { }
        }
        """;

    [Test]
    public async Task FirstRun_ReportsNewStep()
    {
        var compilation = IncrementalGeneratorTestHarness.CreateCompilation(PetUnionSource);
        var driver = IncrementalGeneratorTestHarness.CreateDriver().RunGenerators(compilation);

        var reasons = IncrementalGeneratorTestHarness.StepReasons(driver, NonBoxingUnionGenerator.GetUnionStep);

        await Assert.That(reasons).IsNotEmpty();
        await Assert.That(reasons.All(static r => r == IncrementalStepRunReason.New)).IsTrue();
    }

    [Test]
    public async Task AddingUnrelatedSyntaxTree_ReusesCachedUnionModel()
    {
        var compilation1 = IncrementalGeneratorTestHarness.CreateCompilation(PetUnionSource);

        var driver = IncrementalGeneratorTestHarness.CreateDriver().RunGenerators(compilation1);

        // Add a file that has nothing to do with any union. A correctly incremental generator
        // must NOT rebuild the union model for this edit.
        var compilation2 = compilation1.AddSyntaxTrees(
            CSharpSyntaxTree.ParseText("namespace Other { public class Unrelated { } }", CSharpParseOptions.Default));
        driver = driver.RunGenerators(compilation2);

        var reasons = IncrementalGeneratorTestHarness.StepReasons(driver, NonBoxingUnionGenerator.GetUnionStep);

        await Assert.That(reasons).IsNotEmpty();
        await Assert.That(reasons.All(static r =>
                r is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged))
            .IsTrue();
    }

    [Test]
    public async Task EditingUnrelatedTypeBody_ReusesCachedUnionModel()
    {
        var unrelated = "namespace Other { public class Unrelated { public int X; } }";
        var compilation1 = IncrementalGeneratorTestHarness.CreateCompilation(PetUnionSource, unrelated);

        var driver = IncrementalGeneratorTestHarness.CreateDriver().RunGenerators(compilation1);

        // Mutate the unrelated tree's contents. The union is untouched, so its model must be reused.
        var unrelatedTree = compilation1.SyntaxTrees[1];
        var editedTree = CSharpSyntaxTree.ParseText(
            "namespace Other { public class Unrelated { public int X; public string Y = \"changed\"; } }",
            CSharpParseOptions.Default);
        var compilation2 = compilation1.ReplaceSyntaxTree(unrelatedTree, editedTree);
        driver = driver.RunGenerators(compilation2);

        var reasons = IncrementalGeneratorTestHarness.StepReasons(driver, NonBoxingUnionGenerator.GetUnionStep);

        await Assert.That(reasons).IsNotEmpty();
        await Assert.That(reasons.All(static r =>
                r is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged))
            .IsTrue();
    }

    [Test]
    public async Task ChangingUnionCaseTypes_RebuildsUnionModel()
    {
        var compilation1 = IncrementalGeneratorTestHarness.CreateCompilation(PetUnionSource);

        var driver = IncrementalGeneratorTestHarness.CreateDriver().RunGenerators(compilation1);

        // A real change to the union: add a third case type. The model must be recomputed.
        var modifiedUnion =
            """
            using NonboxingUnion;

            namespace Sample
            {
                public sealed class Dog { }
                public sealed class Cat { }
                public sealed class Fish { }

                [NonBoxingUnion(typeof(Dog), typeof(Cat), typeof(Fish))]
                public partial struct Pet { }
            }
            """;

        var compilation2 = compilation1.ReplaceSyntaxTree(
            compilation1.SyntaxTrees[0],
            CSharpSyntaxTree.ParseText(modifiedUnion, CSharpParseOptions.Default));
        driver = driver.RunGenerators(compilation2);

        var reasons = IncrementalGeneratorTestHarness.StepReasons(driver, NonBoxingUnionGenerator.GetUnionStep);

        await Assert.That(reasons).Contains(IncrementalStepRunReason.Modified);
    }

    [Test]
    public async Task AddingSecondUnion_DoesNotRebuildExistingUnion()
    {
        var compilation1 = IncrementalGeneratorTestHarness.CreateCompilation(PetUnionSource);

        var driver = IncrementalGeneratorTestHarness.CreateDriver().RunGenerators(compilation1);

        var secondUnion =
            """
            using NonboxingUnion;

            namespace Sample
            {
                [NonBoxingUnion(typeof(int), typeof(string))]
                public partial struct Number { }
            }
            """;

        var compilation2 = compilation1.AddSyntaxTrees(
            CSharpSyntaxTree.ParseText(secondUnion, CSharpParseOptions.Default));
        driver = driver.RunGenerators(compilation2);

        var reasons = IncrementalGeneratorTestHarness.StepReasons(driver, NonBoxingUnionGenerator.GetUnionStep);

        // The newly added union is New; the pre-existing Pet union must be reused, not rebuilt.
        await Assert.That(reasons).Contains(IncrementalStepRunReason.New);
        await Assert.That(reasons.Any(static r =>
                r is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged))
            .IsTrue();
        await Assert.That(reasons).DoesNotContain(IncrementalStepRunReason.Modified);
    }
}
