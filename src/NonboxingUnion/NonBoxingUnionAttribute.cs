namespace NonboxingUnion;

/// <summary>
/// Marks a partial struct as a non-boxing discriminated union for which the
/// source generator will emit the discriminator, per-variant storage, and the
/// union access members (<c>Value</c>, <c>HasValue</c>, <c>TryGetValue</c>) as
/// well as a constructor for each case type.
/// </summary>
/// <remarks>
/// The case types of the union are supplied as the constructor arguments, for
/// example <c>[NonBoxingUnion(typeof(Dog), typeof(Cat))]</c>. The generated type
/// is also annotated with <c>[System.Runtime.CompilerServices.Union]</c> and
/// implements <c>System.Runtime.CompilerServices.IUnion</c> so that it
/// participates in the C# union language feature.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NonBoxingUnionAttribute"/> class.
    /// </summary>
    /// <param name="types">The case types (variants) that make up the union.</param>
    public NonBoxingUnionAttribute(params Type[] types)
    {
        Types = types;
    }

    /// <summary>
    /// Gets the case types (variants) that make up the union.
    /// </summary>
    public Type[] Types { get; }
}
