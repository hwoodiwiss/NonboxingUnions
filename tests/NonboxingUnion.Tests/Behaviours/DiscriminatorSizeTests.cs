using System.Reflection;

namespace NonboxingUnion.Tests.Behaviours;

public partial class DiscriminatorSizeTests
{
    [NonBoxingUnion(typeof(int), typeof(bool), typeof(string))]
    public partial struct ThreeVariants;

    [Test]
    public async Task SmallUnion_UsesByteBackedDiscriminator()
    {
        var discriminatorType = typeof(ThreeVariants)
            .GetNestedType("Discriminator", BindingFlags.NonPublic | BindingFlags.Public);

        await Assert.That(discriminatorType).IsNotNull();
        await Assert.That(discriminatorType!.IsEnum).IsTrue();
        await Assert.That(Enum.GetUnderlyingType(discriminatorType)).IsEqualTo(typeof(byte));
    }
}
