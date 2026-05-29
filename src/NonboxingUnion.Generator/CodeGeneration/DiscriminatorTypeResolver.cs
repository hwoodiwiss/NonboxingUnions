namespace NonboxingUnion.Generator.CodeGeneration;

/// <summary>
/// Resolves the smallest unsigned integral type able to represent a union's
/// discriminator. The discriminator reserves the value <c>0</c> for the
/// uninitialized/default state, so the number of representable values must be
/// at least <c>variantCount + 1</c>.
/// </summary>
internal static class DiscriminatorTypeResolver
{
    public static string GetBackingTypeKeyword(int variantCount)
    {
        if (variantCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(variantCount), variantCount, "A union must declare at least one variant.");
        }

        // +1 accounts for the reserved 'None' (default) member with value 0.
        long requiredValues = (long)variantCount + 1;

        return requiredValues switch
        {
            <= byte.MaxValue + 1L => "byte",
            <= ushort.MaxValue + 1L => "ushort",
            <= uint.MaxValue + 1L => "uint",
            _ => "ulong",
        };
    }
}
