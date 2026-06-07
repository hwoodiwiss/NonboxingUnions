namespace NonboxingUnion.Tests.Behaviours;

/// <summary>
/// Exercises unions whose variants are generic type parameters of the union struct itself.
/// Three configurations are covered:
///   - Unconstrained type parameters (the typical Result&lt;T, TError&gt; pattern).
///   - A struct-constrained type parameter (stored as a value type, no boxing).
///   - A class-constrained type parameter (stored as a nullable reference, no boxing).
/// </summary>
public partial class GenericUnionTests
{
    // -------------------------------------------------------------------------
    // Result<T, TError> — both type parameters unconstrained
    // -------------------------------------------------------------------------

    [NonBoxingUnion]
    public partial struct Result<T, TError>;

    [Test]
    public async Task Result_SuccessVariant_RoundTripsThroughTryGetValue()
    {
        Result<int, string> result = 42;

        var success = result.TryGetValue(out int value);

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsEqualTo(42);
    }

    [Test]
    public async Task Result_ErrorVariant_RoundTripsThroughTryGetValue()
    {
        Result<int, string> result = "something went wrong";

        var success = result.TryGetValue(out string error);

        await Assert.That(success).IsTrue();
        await Assert.That(error).IsEqualTo("something went wrong");
    }

    [Test]
    public async Task Result_TryGetValue_ReturnsFalseForWrongVariant()
    {
        Result<int, string> result = 42;

        var success = result.TryGetValue(out string _);

        await Assert.That(success).IsFalse();
    }

    [Test]
    public async Task Result_HasValue_IsTrueAfterConstruction()
    {
        Result<int, string> result = 42;

        await Assert.That(result.HasValue).IsTrue();
    }

    [Test]
    public async Task Result_Default_HasNoValue()
    {
        Result<int, string> result = default;

        await Assert.That(result.HasValue).IsFalse();
        await Assert.That(result.Value).IsNull();
    }

    [Test]
    public async Task Result_PatternMatching_DispatchesToCorrectCase()
    {
        Result<int, string> ok = 7;
        Result<int, string> err = "oops";

        await Assert.That(Describe(ok)).IsEqualTo("ok: 7");
        await Assert.That(Describe(err)).IsEqualTo("err: oops");
        await Assert.That(Describe(default)).IsEqualTo("none");

        static string Describe(Result<int, string> r) => r switch
        {
            int v => $"ok: {v}",
            string e => $"err: {e}",
            null => "none",
        };
    }

    // -------------------------------------------------------------------------
    // ValueOrError<T, TError> — struct-constrained T, class-constrained TError
    // The struct constraint means T is stored as a plain value type (no Nullable<T>
    // wrapper), exactly like a concrete value-type variant.
    // The class constraint means TError is stored as TError? (nullable ref), exactly
    // like a concrete reference-type variant.
    // -------------------------------------------------------------------------

    [NonBoxingUnion]
    public partial struct ValueOrError<T, TError>
        where T : struct
        where TError : class;

    [Test]
    public async Task ValueOrError_StructVariant_RoundTripsThroughTryGetValue()
    {
        ValueOrError<int, Exception> result = 99;

        var success = result.TryGetValue(out int value);

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsEqualTo(99);
    }

    [Test]
    public async Task ValueOrError_ClassVariant_ReturnsSameReference()
    {
        var ex = new InvalidOperationException("boom");
        ValueOrError<int, Exception> result = ex;

        var success = result.TryGetValue(out Exception error);

        await Assert.That(success).IsTrue();
        await Assert.That(error).IsSameReferenceAs(ex);
    }

    [Test]
    public async Task ValueOrError_TryGetValue_ReturnsFalseForInactiveVariant()
    {
        ValueOrError<int, Exception> result = 1;

        var success = result.TryGetValue(out Exception _);

        await Assert.That(success).IsFalse();
    }

    [Test]
    public async Task ValueOrError_Default_HasNoValue()
    {
        ValueOrError<int, Exception> result = default;

        await Assert.That(result.HasValue).IsFalse();
        await Assert.That(result.Value).IsNull();
    }
}
