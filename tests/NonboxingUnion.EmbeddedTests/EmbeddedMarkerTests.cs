using System.Linq;
using System.Reflection;

namespace NonboxingUnion.EmbeddedTests;

public partial class EmbeddedMarkerTests
{
    [NonBoxingUnion(typeof(int), typeof(string))]
    public partial struct IntOrString;

    [Test]
    public async Task GeneratedUnion_RoundTripsValue()
    {
        IntOrString union = 42;

        var success = union.TryGetValue(out int value);

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsEqualTo(42);
    }

    [Test]
    public async Task GeneratedUnion_ParticipatesInUnionFeature()
    {
        var isUnion = typeof(IntOrString).GetCustomAttributes(typeof(UnionAttribute), false).Length == 1;
        var implementsIUnion = typeof(IUnion).IsAssignableFrom(typeof(IntOrString));

        await Assert.That(isUnion).IsTrue();
        await Assert.That(implementsIUnion).IsTrue();
    }

    [Test]
    public async Task MarkerAttribute_IsEmbeddedIntoThisAssembly()
    {
        var markerType = typeof(IntOrString)
            .GetCustomAttributesData()
            .Single(a => a.AttributeType.FullName == "NonboxingUnion.NonBoxingUnionAttribute")
            .AttributeType;

        // The marker is generated into our own compilation rather than coming from
        // a referenced runtime assembly, so it lives in this very assembly...
        await Assert.That(markerType.Assembly).IsEqualTo(typeof(IntOrString).Assembly);

        // ...and it is internal, not part of any public surface.
        await Assert.That(markerType.IsVisible).IsFalse();
    }

    [Test]
    public async Task NoNonboxingUnionRuntimeAssembly_IsReferenced()
    {
        var referencesRuntimeAssembly = typeof(IntOrString).Assembly
            .GetReferencedAssemblies()
            .Any(a => a.Name == "NonboxingUnion");

        await Assert.That(referencesRuntimeAssembly).IsFalse();
    }
}
