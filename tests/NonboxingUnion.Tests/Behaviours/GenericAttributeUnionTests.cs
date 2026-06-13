namespace NonboxingUnion.Tests.Behaviours;

public partial class GenericAttributeUnionTests
{
    public sealed class Dog
    {
        public override string ToString() => "woof";
    }

    public sealed class Cat
    {
        public override string ToString() => "meow";
    }

    // The generic attribute form: case types as type arguments rather than typeof(...).
    [NonBoxingUnion<Dog, Cat>]
    public partial struct Pet;

    [NonBoxingUnion<int, bool, string>]
    public partial struct ThreeVariants;

    [NonBoxingUnion<int>]
    public partial struct SingleVariant;

    [Test]
    public async Task ImplicitConversion_FromReferenceType_RoundTrips()
    {
        var dog = new Dog();
        Pet union = dog;

        var success = union.TryGetValue(out Dog value);

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsSameReferenceAs(dog);
    }

    [Test]
    public async Task PatternMatching_DispatchesToCorrectCase()
    {
        Pet dogUnion = new Dog();
        Pet catUnion = new Cat();

        await Assert.That(Describe(dogUnion)).IsEqualTo("woof");
        await Assert.That(Describe(catUnion)).IsEqualTo("meow");
        await Assert.That(Describe(default)).IsEqualTo("none");

        static string Describe(Pet union) => union switch
        {
            Dog d => d.ToString(),
            Cat c => c.ToString(),
            null => "none",
        };
    }

    [Test]
    public async Task ValueTypeCase_IsStoredWithoutBoxing()
    {
        ThreeVariants union = 42;

        var success = union.TryGetValue(out int value);

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsEqualTo(42);
        await Assert.That(union.TryGetValue(out bool _)).IsFalse();
    }

    [Test]
    public async Task SingleVariant_RoundTrips()
    {
        SingleVariant union = 7;

        await Assert.That(union.HasValue).IsTrue();
        await Assert.That(union.TryGetValue(out int value)).IsTrue();
        await Assert.That(value).IsEqualTo(7);
    }

    [Test]
    public async Task Default_HasNoValue()
    {
        Pet union = default;

        await Assert.That(union.HasValue).IsFalse();
        await Assert.That(union.Value).IsNull();
    }
}
