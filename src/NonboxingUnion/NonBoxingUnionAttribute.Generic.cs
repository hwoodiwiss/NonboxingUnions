namespace NonboxingUnion;

/// <summary>
/// Marks a partial struct as a non-boxing discriminated union whose case types
/// (variants) are supplied as generic type arguments, for example
/// <c>[NonBoxingUnion&lt;Dog&gt;]</c>.
/// </summary>
/// <remarks>
/// This is the generic counterpart of <see cref="NonBoxingUnionAttribute"/>; it carries
/// the same meaning, but lets the case types be expressed as type arguments rather than
/// <c>typeof(...)</c> values. See <see cref="NonBoxingUnionAttribute"/> for details.
/// </remarks>
/// <typeparam name="T1">The first case type (variant) of the union.</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute<T1> : Attribute;

/// <summary>
/// Marks a partial struct as a non-boxing discriminated union whose case types
/// (variants) are supplied as generic type arguments, for example
/// <c>[NonBoxingUnion&lt;Dog, Cat&gt;]</c>.
/// </summary>
/// <remarks>
/// This is the generic counterpart of <see cref="NonBoxingUnionAttribute"/>; it carries
/// the same meaning, but lets the case types be expressed as type arguments rather than
/// <c>typeof(...)</c> values. See <see cref="NonBoxingUnionAttribute"/> for details.
/// </remarks>
/// <typeparam name="T1">The first case type (variant) of the union.</typeparam>
/// <typeparam name="T2">The second case type (variant) of the union.</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute<T1, T2> : Attribute;

/// <summary>
/// Marks a partial struct as a non-boxing discriminated union whose case types
/// (variants) are supplied as generic type arguments, for example
/// <c>[NonBoxingUnion&lt;T1, T2, T3&gt;]</c>.
/// </summary>
/// <remarks>
/// This is the generic counterpart of <see cref="NonBoxingUnionAttribute"/>; it carries
/// the same meaning, but lets the case types be expressed as type arguments rather than
/// <c>typeof(...)</c> values. See <see cref="NonBoxingUnionAttribute"/> for details.
/// </remarks>
/// <typeparam name="T1">The first case type (variant) of the union.</typeparam>
/// <typeparam name="T2">The second case type (variant) of the union.</typeparam>
/// <typeparam name="T3">The third case type (variant) of the union.</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute<T1, T2, T3> : Attribute;

/// <summary>
/// Marks a partial struct as a non-boxing discriminated union whose case types
/// (variants) are supplied as generic type arguments, for example
/// <c>[NonBoxingUnion&lt;T1, T2, T3, T4&gt;]</c>.
/// </summary>
/// <remarks>
/// This is the generic counterpart of <see cref="NonBoxingUnionAttribute"/>; it carries
/// the same meaning, but lets the case types be expressed as type arguments rather than
/// <c>typeof(...)</c> values. See <see cref="NonBoxingUnionAttribute"/> for details.
/// </remarks>
/// <typeparam name="T1">The first case type (variant) of the union.</typeparam>
/// <typeparam name="T2">The second case type (variant) of the union.</typeparam>
/// <typeparam name="T3">The third case type (variant) of the union.</typeparam>
/// <typeparam name="T4">The fourth case type (variant) of the union.</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute<T1, T2, T3, T4> : Attribute;

/// <summary>
/// Marks a partial struct as a non-boxing discriminated union whose case types
/// (variants) are supplied as generic type arguments, for example
/// <c>[NonBoxingUnion&lt;T1, T2, T3, T4, T5&gt;]</c>.
/// </summary>
/// <remarks>
/// This is the generic counterpart of <see cref="NonBoxingUnionAttribute"/>; it carries
/// the same meaning, but lets the case types be expressed as type arguments rather than
/// <c>typeof(...)</c> values. See <see cref="NonBoxingUnionAttribute"/> for details.
/// </remarks>
/// <typeparam name="T1">The first case type (variant) of the union.</typeparam>
/// <typeparam name="T2">The second case type (variant) of the union.</typeparam>
/// <typeparam name="T3">The third case type (variant) of the union.</typeparam>
/// <typeparam name="T4">The fourth case type (variant) of the union.</typeparam>
/// <typeparam name="T5">The fifth case type (variant) of the union.</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute<T1, T2, T3, T4, T5> : Attribute;

/// <summary>
/// Marks a partial struct as a non-boxing discriminated union whose case types
/// (variants) are supplied as generic type arguments, for example
/// <c>[NonBoxingUnion&lt;T1, T2, T3, T4, T5, T6&gt;]</c>.
/// </summary>
/// <remarks>
/// This is the generic counterpart of <see cref="NonBoxingUnionAttribute"/>; it carries
/// the same meaning, but lets the case types be expressed as type arguments rather than
/// <c>typeof(...)</c> values. See <see cref="NonBoxingUnionAttribute"/> for details.
/// </remarks>
/// <typeparam name="T1">The first case type (variant) of the union.</typeparam>
/// <typeparam name="T2">The second case type (variant) of the union.</typeparam>
/// <typeparam name="T3">The third case type (variant) of the union.</typeparam>
/// <typeparam name="T4">The fourth case type (variant) of the union.</typeparam>
/// <typeparam name="T5">The fifth case type (variant) of the union.</typeparam>
/// <typeparam name="T6">The sixth case type (variant) of the union.</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute<T1, T2, T3, T4, T5, T6> : Attribute;

/// <summary>
/// Marks a partial struct as a non-boxing discriminated union whose case types
/// (variants) are supplied as generic type arguments, for example
/// <c>[NonBoxingUnion&lt;T1, T2, T3, T4, T5, T6, T7&gt;]</c>.
/// </summary>
/// <remarks>
/// This is the generic counterpart of <see cref="NonBoxingUnionAttribute"/>; it carries
/// the same meaning, but lets the case types be expressed as type arguments rather than
/// <c>typeof(...)</c> values. See <see cref="NonBoxingUnionAttribute"/> for details.
/// </remarks>
/// <typeparam name="T1">The first case type (variant) of the union.</typeparam>
/// <typeparam name="T2">The second case type (variant) of the union.</typeparam>
/// <typeparam name="T3">The third case type (variant) of the union.</typeparam>
/// <typeparam name="T4">The fourth case type (variant) of the union.</typeparam>
/// <typeparam name="T5">The fifth case type (variant) of the union.</typeparam>
/// <typeparam name="T6">The sixth case type (variant) of the union.</typeparam>
/// <typeparam name="T7">The seventh case type (variant) of the union.</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute<T1, T2, T3, T4, T5, T6, T7> : Attribute;

/// <summary>
/// Marks a partial struct as a non-boxing discriminated union whose case types
/// (variants) are supplied as generic type arguments, for example
/// <c>[NonBoxingUnion&lt;T1, T2, T3, T4, T5, T6, T7, T8&gt;]</c>.
/// </summary>
/// <remarks>
/// This is the generic counterpart of <see cref="NonBoxingUnionAttribute"/>; it carries
/// the same meaning, but lets the case types be expressed as type arguments rather than
/// <c>typeof(...)</c> values. See <see cref="NonBoxingUnionAttribute"/> for details.
/// </remarks>
/// <typeparam name="T1">The first case type (variant) of the union.</typeparam>
/// <typeparam name="T2">The second case type (variant) of the union.</typeparam>
/// <typeparam name="T3">The third case type (variant) of the union.</typeparam>
/// <typeparam name="T4">The fourth case type (variant) of the union.</typeparam>
/// <typeparam name="T5">The fifth case type (variant) of the union.</typeparam>
/// <typeparam name="T6">The sixth case type (variant) of the union.</typeparam>
/// <typeparam name="T7">The seventh case type (variant) of the union.</typeparam>
/// <typeparam name="T8">The eighth case type (variant) of the union.</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute<T1, T2, T3, T4, T5, T6, T7, T8> : Attribute;

/// <summary>
/// Marks a partial struct as a non-boxing discriminated union whose case types
/// (variants) are supplied as generic type arguments, for example
/// <c>[NonBoxingUnion&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9&gt;]</c>.
/// </summary>
/// <remarks>
/// This is the generic counterpart of <see cref="NonBoxingUnionAttribute"/>; it carries
/// the same meaning, but lets the case types be expressed as type arguments rather than
/// <c>typeof(...)</c> values. See <see cref="NonBoxingUnionAttribute"/> for details.
/// </remarks>
/// <typeparam name="T1">The first case type (variant) of the union.</typeparam>
/// <typeparam name="T2">The second case type (variant) of the union.</typeparam>
/// <typeparam name="T3">The third case type (variant) of the union.</typeparam>
/// <typeparam name="T4">The fourth case type (variant) of the union.</typeparam>
/// <typeparam name="T5">The fifth case type (variant) of the union.</typeparam>
/// <typeparam name="T6">The sixth case type (variant) of the union.</typeparam>
/// <typeparam name="T7">The seventh case type (variant) of the union.</typeparam>
/// <typeparam name="T8">The eighth case type (variant) of the union.</typeparam>
/// <typeparam name="T9">The ninth case type (variant) of the union.</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9> : Attribute;

/// <summary>
/// Marks a partial struct as a non-boxing discriminated union whose case types
/// (variants) are supplied as generic type arguments, for example
/// <c>[NonBoxingUnion&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10&gt;]</c>.
/// </summary>
/// <remarks>
/// This is the generic counterpart of <see cref="NonBoxingUnionAttribute"/>; it carries
/// the same meaning, but lets the case types be expressed as type arguments rather than
/// <c>typeof(...)</c> values. See <see cref="NonBoxingUnionAttribute"/> for details.
/// </remarks>
/// <typeparam name="T1">The first case type (variant) of the union.</typeparam>
/// <typeparam name="T2">The second case type (variant) of the union.</typeparam>
/// <typeparam name="T3">The third case type (variant) of the union.</typeparam>
/// <typeparam name="T4">The fourth case type (variant) of the union.</typeparam>
/// <typeparam name="T5">The fifth case type (variant) of the union.</typeparam>
/// <typeparam name="T6">The sixth case type (variant) of the union.</typeparam>
/// <typeparam name="T7">The seventh case type (variant) of the union.</typeparam>
/// <typeparam name="T8">The eighth case type (variant) of the union.</typeparam>
/// <typeparam name="T9">The ninth case type (variant) of the union.</typeparam>
/// <typeparam name="T10">The tenth case type (variant) of the union.</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : Attribute;

/// <summary>
/// Marks a partial struct as a non-boxing discriminated union whose case types
/// (variants) are supplied as generic type arguments, for example
/// <c>[NonBoxingUnion&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11&gt;]</c>.
/// </summary>
/// <remarks>
/// This is the generic counterpart of <see cref="NonBoxingUnionAttribute"/>; it carries
/// the same meaning, but lets the case types be expressed as type arguments rather than
/// <c>typeof(...)</c> values. See <see cref="NonBoxingUnionAttribute"/> for details.
/// </remarks>
/// <typeparam name="T1">The first case type (variant) of the union.</typeparam>
/// <typeparam name="T2">The second case type (variant) of the union.</typeparam>
/// <typeparam name="T3">The third case type (variant) of the union.</typeparam>
/// <typeparam name="T4">The fourth case type (variant) of the union.</typeparam>
/// <typeparam name="T5">The fifth case type (variant) of the union.</typeparam>
/// <typeparam name="T6">The sixth case type (variant) of the union.</typeparam>
/// <typeparam name="T7">The seventh case type (variant) of the union.</typeparam>
/// <typeparam name="T8">The eighth case type (variant) of the union.</typeparam>
/// <typeparam name="T9">The ninth case type (variant) of the union.</typeparam>
/// <typeparam name="T10">The tenth case type (variant) of the union.</typeparam>
/// <typeparam name="T11">The eleventh case type (variant) of the union.</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : Attribute;

/// <summary>
/// Marks a partial struct as a non-boxing discriminated union whose case types
/// (variants) are supplied as generic type arguments, for example
/// <c>[NonBoxingUnion&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12&gt;]</c>.
/// </summary>
/// <remarks>
/// This is the generic counterpart of <see cref="NonBoxingUnionAttribute"/>; it carries
/// the same meaning, but lets the case types be expressed as type arguments rather than
/// <c>typeof(...)</c> values. See <see cref="NonBoxingUnionAttribute"/> for details.
/// </remarks>
/// <typeparam name="T1">The first case type (variant) of the union.</typeparam>
/// <typeparam name="T2">The second case type (variant) of the union.</typeparam>
/// <typeparam name="T3">The third case type (variant) of the union.</typeparam>
/// <typeparam name="T4">The fourth case type (variant) of the union.</typeparam>
/// <typeparam name="T5">The fifth case type (variant) of the union.</typeparam>
/// <typeparam name="T6">The sixth case type (variant) of the union.</typeparam>
/// <typeparam name="T7">The seventh case type (variant) of the union.</typeparam>
/// <typeparam name="T8">The eighth case type (variant) of the union.</typeparam>
/// <typeparam name="T9">The ninth case type (variant) of the union.</typeparam>
/// <typeparam name="T10">The tenth case type (variant) of the union.</typeparam>
/// <typeparam name="T11">The eleventh case type (variant) of the union.</typeparam>
/// <typeparam name="T12">The twelfth case type (variant) of the union.</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : Attribute;

/// <summary>
/// Marks a partial struct as a non-boxing discriminated union whose case types
/// (variants) are supplied as generic type arguments, for example
/// <c>[NonBoxingUnion&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13&gt;]</c>.
/// </summary>
/// <remarks>
/// This is the generic counterpart of <see cref="NonBoxingUnionAttribute"/>; it carries
/// the same meaning, but lets the case types be expressed as type arguments rather than
/// <c>typeof(...)</c> values. See <see cref="NonBoxingUnionAttribute"/> for details.
/// </remarks>
/// <typeparam name="T1">The first case type (variant) of the union.</typeparam>
/// <typeparam name="T2">The second case type (variant) of the union.</typeparam>
/// <typeparam name="T3">The third case type (variant) of the union.</typeparam>
/// <typeparam name="T4">The fourth case type (variant) of the union.</typeparam>
/// <typeparam name="T5">The fifth case type (variant) of the union.</typeparam>
/// <typeparam name="T6">The sixth case type (variant) of the union.</typeparam>
/// <typeparam name="T7">The seventh case type (variant) of the union.</typeparam>
/// <typeparam name="T8">The eighth case type (variant) of the union.</typeparam>
/// <typeparam name="T9">The ninth case type (variant) of the union.</typeparam>
/// <typeparam name="T10">The tenth case type (variant) of the union.</typeparam>
/// <typeparam name="T11">The eleventh case type (variant) of the union.</typeparam>
/// <typeparam name="T12">The twelfth case type (variant) of the union.</typeparam>
/// <typeparam name="T13">The thirteenth case type (variant) of the union.</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : Attribute;

/// <summary>
/// Marks a partial struct as a non-boxing discriminated union whose case types
/// (variants) are supplied as generic type arguments, for example
/// <c>[NonBoxingUnion&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14&gt;]</c>.
/// </summary>
/// <remarks>
/// This is the generic counterpart of <see cref="NonBoxingUnionAttribute"/>; it carries
/// the same meaning, but lets the case types be expressed as type arguments rather than
/// <c>typeof(...)</c> values. See <see cref="NonBoxingUnionAttribute"/> for details.
/// </remarks>
/// <typeparam name="T1">The first case type (variant) of the union.</typeparam>
/// <typeparam name="T2">The second case type (variant) of the union.</typeparam>
/// <typeparam name="T3">The third case type (variant) of the union.</typeparam>
/// <typeparam name="T4">The fourth case type (variant) of the union.</typeparam>
/// <typeparam name="T5">The fifth case type (variant) of the union.</typeparam>
/// <typeparam name="T6">The sixth case type (variant) of the union.</typeparam>
/// <typeparam name="T7">The seventh case type (variant) of the union.</typeparam>
/// <typeparam name="T8">The eighth case type (variant) of the union.</typeparam>
/// <typeparam name="T9">The ninth case type (variant) of the union.</typeparam>
/// <typeparam name="T10">The tenth case type (variant) of the union.</typeparam>
/// <typeparam name="T11">The eleventh case type (variant) of the union.</typeparam>
/// <typeparam name="T12">The twelfth case type (variant) of the union.</typeparam>
/// <typeparam name="T13">The thirteenth case type (variant) of the union.</typeparam>
/// <typeparam name="T14">The fourteenth case type (variant) of the union.</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : Attribute;

/// <summary>
/// Marks a partial struct as a non-boxing discriminated union whose case types
/// (variants) are supplied as generic type arguments, for example
/// <c>[NonBoxingUnion&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15&gt;]</c>.
/// </summary>
/// <remarks>
/// This is the generic counterpart of <see cref="NonBoxingUnionAttribute"/>; it carries
/// the same meaning, but lets the case types be expressed as type arguments rather than
/// <c>typeof(...)</c> values. See <see cref="NonBoxingUnionAttribute"/> for details.
/// </remarks>
/// <typeparam name="T1">The first case type (variant) of the union.</typeparam>
/// <typeparam name="T2">The second case type (variant) of the union.</typeparam>
/// <typeparam name="T3">The third case type (variant) of the union.</typeparam>
/// <typeparam name="T4">The fourth case type (variant) of the union.</typeparam>
/// <typeparam name="T5">The fifth case type (variant) of the union.</typeparam>
/// <typeparam name="T6">The sixth case type (variant) of the union.</typeparam>
/// <typeparam name="T7">The seventh case type (variant) of the union.</typeparam>
/// <typeparam name="T8">The eighth case type (variant) of the union.</typeparam>
/// <typeparam name="T9">The ninth case type (variant) of the union.</typeparam>
/// <typeparam name="T10">The tenth case type (variant) of the union.</typeparam>
/// <typeparam name="T11">The eleventh case type (variant) of the union.</typeparam>
/// <typeparam name="T12">The twelfth case type (variant) of the union.</typeparam>
/// <typeparam name="T13">The thirteenth case type (variant) of the union.</typeparam>
/// <typeparam name="T14">The fourteenth case type (variant) of the union.</typeparam>
/// <typeparam name="T15">The fifteenth case type (variant) of the union.</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : Attribute;

/// <summary>
/// Marks a partial struct as a non-boxing discriminated union whose case types
/// (variants) are supplied as generic type arguments, for example
/// <c>[NonBoxingUnion&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16&gt;]</c>.
/// </summary>
/// <remarks>
/// This is the generic counterpart of <see cref="NonBoxingUnionAttribute"/>; it carries
/// the same meaning, but lets the case types be expressed as type arguments rather than
/// <c>typeof(...)</c> values. See <see cref="NonBoxingUnionAttribute"/> for details.
/// </remarks>
/// <typeparam name="T1">The first case type (variant) of the union.</typeparam>
/// <typeparam name="T2">The second case type (variant) of the union.</typeparam>
/// <typeparam name="T3">The third case type (variant) of the union.</typeparam>
/// <typeparam name="T4">The fourth case type (variant) of the union.</typeparam>
/// <typeparam name="T5">The fifth case type (variant) of the union.</typeparam>
/// <typeparam name="T6">The sixth case type (variant) of the union.</typeparam>
/// <typeparam name="T7">The seventh case type (variant) of the union.</typeparam>
/// <typeparam name="T8">The eighth case type (variant) of the union.</typeparam>
/// <typeparam name="T9">The ninth case type (variant) of the union.</typeparam>
/// <typeparam name="T10">The tenth case type (variant) of the union.</typeparam>
/// <typeparam name="T11">The eleventh case type (variant) of the union.</typeparam>
/// <typeparam name="T12">The twelfth case type (variant) of the union.</typeparam>
/// <typeparam name="T13">The thirteenth case type (variant) of the union.</typeparam>
/// <typeparam name="T14">The fourteenth case type (variant) of the union.</typeparam>
/// <typeparam name="T15">The fifteenth case type (variant) of the union.</typeparam>
/// <typeparam name="T16">The sixteenth case type (variant) of the union.</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : Attribute;
