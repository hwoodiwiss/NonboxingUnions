using System.Collections.Immutable;

namespace NonboxingUnion.Generator.Parsing;

internal sealed record UnionVariant(
    string CaseTypeName,
    string ConstructorParameterType,
    string StorageTypeName,
    string FieldName,
    string MemberName,
    bool IsReferenceType,
    bool IsNullableValueType);

internal sealed record ContainingType(
    string Accessibility,
    bool IsStatic,
    string Keyword,
    string Name);

internal sealed record UnionToGenerate
{
    public UnionToGenerate(
        string @namespace,
        string accessibility,
        string name,
        ImmutableArray<ContainingType> containingTypes,
        ImmutableArray<UnionVariant> variants)
    {
        Namespace = @namespace;
        Accessibility = accessibility;
        Name = name;
        ContainingTypes = containingTypes;
        Variants = variants;
    }

    public string Namespace { get; }

    public string Accessibility { get; }

    public string Name { get; }

    public ImmutableArray<ContainingType> ContainingTypes { get; }

    public ImmutableArray<UnionVariant> Variants { get; }

    public bool Equals(UnionToGenerate? other)
    {
        if (other is null)
        {
            return false;
        }

        return Namespace == other.Namespace
            && Accessibility == other.Accessibility
            && Name == other.Name
            && ContainingTypes.SequenceEqual(other.ContainingTypes)
            && Variants.SequenceEqual(other.Variants);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = (hash * 31) + Namespace.GetHashCode();
            hash = (hash * 31) + Accessibility.GetHashCode();
            hash = (hash * 31) + Name.GetHashCode();
            foreach (var containingType in ContainingTypes)
            {
                hash = (hash * 31) + containingType.GetHashCode();
            }

            foreach (var variant in Variants)
            {
                hash = (hash * 31) + variant.GetHashCode();
            }

            return hash;
        }
    }
}
