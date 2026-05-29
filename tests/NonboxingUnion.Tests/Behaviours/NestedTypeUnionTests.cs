namespace NonboxingUnion.Tests.Behaviours;

public partial class NestedTypeUnionTests
{
    public partial class Outer
    {
        public partial class Inner
        {
            [NonBoxingUnion(typeof(int), typeof(long))]
            public partial struct DeeplyNested;
        }
    }

    [Test]
    public async Task UnionNestedInContainingTypes_GeneratesCorrectly()
    {
        Outer.Inner.DeeplyNested union = 123;

        var success = union.TryGetValue(out int value);

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsEqualTo(123);
    }

    [Test]
    public async Task NestedUnion_DispatchesAcrossCases()
    {
        Outer.Inner.DeeplyNested longUnion = 456L;

        var result = longUnion switch
        {
            int i => $"int {i}",
            long l => $"long {l}",
            null => "none",
        };

        await Assert.That(result).IsEqualTo("long 456");
    }
}
