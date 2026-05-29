using NonboxingUnion.Generator.CodeGeneration;

namespace NonboxingUnion.Tests.Generator;

public class DiscriminatorTypeResolverTests
{
    [Test]
    [Arguments(1, "byte")]
    [Arguments(2, "byte")]
    [Arguments(254, "byte")]
    [Arguments(255, "byte")]
    [Arguments(256, "ushort")]
    [Arguments(65534, "ushort")]
    [Arguments(65535, "ushort")]
    [Arguments(65536, "uint")]
    public async Task GetBackingTypeKeyword_ReturnsSmallestUnsignedType(int variantCount, string expected)
    {
        var result = DiscriminatorTypeResolver.GetBackingTypeKeyword(variantCount);

        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task GetBackingTypeKeyword_ForZeroVariants_Throws()
    {
        await Assert.That(() => DiscriminatorTypeResolver.GetBackingTypeKeyword(0))
            .Throws<ArgumentOutOfRangeException>();
    }
}
