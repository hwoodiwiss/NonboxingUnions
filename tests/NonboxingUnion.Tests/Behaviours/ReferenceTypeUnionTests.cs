namespace NonboxingUnion.Tests.Behaviours;

public partial class ReferenceTypeUnionTests
{
    public sealed class Dog
    {
        public override string ToString() => "woof";
    }

    public sealed class Cat
    {
        public override string ToString() => "meow";
    }

    [NonBoxingUnion(typeof(Dog), typeof(Cat))]
    public partial struct Pet;

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
    public async Task Value_ReturnsStoredReference()
    {
        var cat = new Cat();
        Pet union = cat;

        await Assert.That(union.Value).IsSameReferenceAs(cat);
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
    public async Task Default_HasNoValue()
    {
        Pet union = default;

        await Assert.That(union.HasValue).IsFalse();
        await Assert.That(union.Value).IsNull();
    }
}
