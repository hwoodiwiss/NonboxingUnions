using System.Runtime.CompilerServices;

namespace NonboxingUnion.Tests.Behaviours;

public partial class ValueTypeUnionTests
{
    [NonBoxingUnion(typeof(int), typeof(bool))]
    public partial struct IntOrBool;

    [Test]
    public async Task ImplicitConversion_FromInt_RoundTripsThroughTryGetValue()
    {
        IntOrBool union = 42;

        var success = union.TryGetValue(out int value);

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsEqualTo(42);
    }

    [Test]
    public async Task ImplicitConversion_FromBool_RoundTripsThroughTryGetValue()
    {
        IntOrBool union = true;

        var success = union.TryGetValue(out bool value);

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsTrue();
    }

    [Test]
    public async Task TryGetValue_ForWrongCase_ReturnsFalse()
    {
        IntOrBool union = 42;

        var success = union.TryGetValue(out bool _);

        await Assert.That(success).IsFalse();
    }

    [Test]
    public async Task HasValue_WhenConstructedFromCase_IsTrue()
    {
        IntOrBool union = 7;

        await Assert.That(union.HasValue).IsTrue();
    }

    [Test]
    public async Task Default_HasNoValue()
    {
        IntOrBool union = default;

        await Assert.That(union.HasValue).IsFalse();
        await Assert.That(union.Value).IsNull();
    }

    [Test]
    public async Task Value_ReturnsBoxedUnderlyingValue()
    {
        IntOrBool union = 99;

        await Assert.That(union.Value).IsEqualTo(99);
    }

    [Test]
    public async Task PatternMatching_IsExhaustiveAndNonBoxing()
    {
        IntOrBool intUnion = 5;
        IntOrBool boolUnion = false;

        await Assert.That(Describe(intUnion)).IsEqualTo("int 5");
        await Assert.That(Describe(boolUnion)).IsEqualTo("bool False");
        await Assert.That(Describe(default)).IsEqualTo("none");

        static string Describe(IntOrBool union) => union switch
        {
            int i => $"int {i}",
            bool b => $"bool {b}",
            null => "none",
        };
    }

    [Test]
    public async Task IsUnion_IsRecognisedAsUnionType()
    {
        var isUnion = typeof(IntOrBool).GetCustomAttributes(typeof(UnionAttribute), false).Length == 1;
        var implementsIUnion = typeof(IUnion).IsAssignableFrom(typeof(IntOrBool));

        await Assert.That(isUnion).IsTrue();
        await Assert.That(implementsIUnion).IsTrue();
    }
}
