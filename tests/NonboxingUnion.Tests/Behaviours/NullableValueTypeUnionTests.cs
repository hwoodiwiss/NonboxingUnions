namespace NonboxingUnion.Tests.Behaviours;

public partial class NullableValueTypeUnionTests
{
    [NonBoxingUnion(typeof(int?), typeof(string))]
    public partial struct MaybeIntOrString;

    [Test]
    public async Task NullableValueType_CaseTypeCollapsesToUnderlyingType()
    {
        MaybeIntOrString union = 42;

        // Per the union spec, a nullable value-type case collapses to its
        // underlying type, so TryGetValue exposes 'int', not 'int?'.
        var success = union.TryGetValue(out int value);

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsEqualTo(42);
    }

    [Test]
    public async Task StringCase_RoundTrips()
    {
        MaybeIntOrString union = "hello";

        var success = union.TryGetValue(out string value);

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsEqualTo("hello");
    }

    [Test]
    public async Task PatternMatching_UnderlyingTypeMatchesNumericCase()
    {
        MaybeIntOrString union = 7;

        var result = union switch
        {
            int i => $"int {i}",
            string s => $"string {s}",
            null => "none",
        };

        await Assert.That(result).IsEqualTo("int 7");
    }
}
