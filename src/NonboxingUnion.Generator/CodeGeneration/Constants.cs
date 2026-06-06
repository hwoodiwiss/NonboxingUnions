namespace NonboxingUnion.Generator.CodeGeneration;

internal static class Constants
{
    public const int IndentationPerLevel = 4;

    public const string NonBoxingUnionAttributeName = "NonboxingUnion.NonBoxingUnionAttribute";

    /// <summary>
    /// The highest generic arity of <c>NonBoxingUnionAttribute&lt;...&gt;</c> the runtime library
    /// declares. Keep in sync with the generic attribute definitions in the NonboxingUnion project.
    /// </summary>
    public const int MaxGenericAttributeArity = 16;

    /// <summary>
    /// The metadata names of every supported form of the attribute: the non-generic
    /// <c>typeof(...)</c> overload plus each generic <c>NonBoxingUnionAttribute&lt;...&gt;</c> arity.
    /// Generic types carry a <c>`N</c> arity suffix in their metadata name.
    /// </summary>
    public static IEnumerable<string> NonBoxingUnionAttributeNames()
    {
        yield return NonBoxingUnionAttributeName;

        for (var arity = 1; arity <= MaxGenericAttributeArity; arity++)
        {
            yield return $"{NonBoxingUnionAttributeName}`{arity}";
        }
    }

    public const string UnionAttributeType = "global::System.Runtime.CompilerServices.Union";

    public const string UnionInterfaceType = "global::System.Runtime.CompilerServices.IUnion";

    public const string DiscriminatorTypeName = "Discriminator";

    public const string DiscriminatorFieldName = "_discriminator";

    public const string NoneMemberName = "None";
}
