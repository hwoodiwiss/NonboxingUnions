namespace NonboxingUnion;

/// <summary>
/// Marks a partial type as a non-boxing discriminated union for which the
/// source generator will emit the union storage and accessor members.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NonBoxingUnionAttribute : Attribute;
