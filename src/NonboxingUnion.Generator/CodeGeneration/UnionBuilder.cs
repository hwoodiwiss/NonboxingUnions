using System.Text;

using NonboxingUnion.Generator.Parsing;

namespace NonboxingUnion.Generator.CodeGeneration;

internal sealed class UnionBuilder
{
    private readonly StringBuilder _builder;
    private readonly string _typeIndentation;
    private readonly string _memberIndentation;
    private readonly string _bodyIndentation;

    public UnionBuilder(StringBuilder builder, int startingIndentation)
    {
        _builder = builder;
        _typeIndentation = new string(' ', startingIndentation);
        _memberIndentation = new string(' ', startingIndentation + Constants.IndentationPerLevel);
        _bodyIndentation = new string(' ', startingIndentation + Constants.IndentationPerLevel + Constants.IndentationPerLevel);
    }

    public UnionBuilder AddTypeDeclaration(UnionToGenerate union)
    {
        var fullTypeName = GetFullTypeName(union);

        _builder.AppendLine($"{_typeIndentation}[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"{AssemblyMetadata.Name}\", \"{AssemblyMetadata.Version}\")]");
        _builder.AppendLine($"{_typeIndentation}[{Constants.UnionAttributeType}]");
        _builder.AppendLine($"{_typeIndentation}{union.Accessibility} partial struct {fullTypeName} : {Constants.UnionInterfaceType}, global::System.IEquatable<{fullTypeName}>");

        foreach (var typeParam in union.TypeParameters)
        {
            if (!string.IsNullOrEmpty(typeParam.ConstraintClause))
            {
                _builder.AppendLine($"{_typeIndentation}    {typeParam.ConstraintClause}");
            }
        }

        _builder.AppendLine($"{_typeIndentation}{{");

        return this;
    }

    public UnionBuilder AddDiscriminator(UnionToGenerate union)
    {
        var backingType = DiscriminatorTypeResolver.GetBackingTypeKeyword(union.Variants.Length);

        _builder.AppendLine($"{_memberIndentation}private enum {Constants.DiscriminatorTypeName} : {backingType}");
        _builder.AppendLine($"{_memberIndentation}{{");
        _builder.AppendLine($"{_bodyIndentation}{Constants.NoneMemberName} = 0,");

        var index = 1;
        foreach (var variant in union.Variants)
        {
            _builder.AppendLine($"{_bodyIndentation}{variant.MemberName} = {index},");
            index++;
        }

        _builder.AppendLine($"{_memberIndentation}}}");
        _builder.AppendLine();

        _builder.AppendLine($"{_memberIndentation}private {Constants.DiscriminatorTypeName} {Constants.DiscriminatorFieldName};");

        foreach (var variant in union.Variants)
        {
            _builder.AppendLine($"{_memberIndentation}private {variant.StorageTypeName} {variant.FieldName};");
        }

        _builder.AppendLine();

        return this;
    }

    public UnionBuilder AddConstructors(UnionToGenerate union)
    {
        foreach (var variant in union.Variants)
        {
            _builder.AppendLine($"{_memberIndentation}public {union.Name}({variant.ConstructorParameterType} value)");
            _builder.AppendLine($"{_memberIndentation}{{");
            _builder.AppendLine($"{_bodyIndentation}{Constants.DiscriminatorFieldName} = {Constants.DiscriminatorTypeName}.{variant.MemberName};");
            _builder.AppendLine($"{_bodyIndentation}{variant.FieldName} = value;");
            _builder.AppendLine($"{_memberIndentation}}}");
            _builder.AppendLine();
        }

        return this;
    }

    public UnionBuilder AddValueProperty(UnionToGenerate union)
    {
        _builder.AppendLine($"{_memberIndentation}public object? Value => {Constants.DiscriminatorFieldName} switch");
        _builder.AppendLine($"{_memberIndentation}{{");

        foreach (var variant in union.Variants)
        {
            _builder.AppendLine($"{_bodyIndentation}{Constants.DiscriminatorTypeName}.{variant.MemberName} => {variant.FieldName},");
        }

        _builder.AppendLine($"{_bodyIndentation}_ => null,");
        _builder.AppendLine($"{_memberIndentation}}};");
        _builder.AppendLine();

        return this;
    }

    public UnionBuilder AddHasValueProperty()
    {
        _builder.AppendLine($"{_memberIndentation}public bool HasValue => {Constants.DiscriminatorFieldName} != {Constants.DiscriminatorTypeName}.{Constants.NoneMemberName};");
        _builder.AppendLine();

        return this;
    }

    public UnionBuilder AddTryGetValueMethods(UnionToGenerate union)
    {
        foreach (var variant in union.Variants)
        {
            // For reference types and unconstrained type parameters the field is nullable
            // (T? / Dog?) for inactive cases, so the null-forgiving operator bridges the
            // gap to the non-nullable out contract.
            // For nullable value-type cases the case type collapses to the underlying type,
            // so the stored Nullable<T> is unwrapped.
            var assignment = variant switch
            {
                { IsNullableValueType: true } => $"{variant.FieldName}.GetValueOrDefault()",
                { HasNullableStorage: true } => $"{variant.FieldName}!",
                _ => variant.FieldName,
            };

            _builder.AppendLine($"{_memberIndentation}public bool TryGetValue(out {variant.CaseTypeName} value)");
            _builder.AppendLine($"{_memberIndentation}{{");
            _builder.AppendLine($"{_bodyIndentation}value = {assignment};");
            _builder.AppendLine($"{_bodyIndentation}return {Constants.DiscriminatorFieldName} == {Constants.DiscriminatorTypeName}.{variant.MemberName};");
            _builder.AppendLine($"{_memberIndentation}}}");
            _builder.AppendLine();
        }

        return this;
    }

    public UnionBuilder AddEqualityMembers(UnionToGenerate union)
    {
        var fullTypeName = GetFullTypeName(union);

        // Equals(T) compares the active discriminator and the corresponding field.
        _builder.AppendLine($"{_memberIndentation}public bool Equals({fullTypeName} other)");
        _builder.AppendLine($"{_memberIndentation}{{");
        _builder.AppendLine($"{_bodyIndentation}if ({Constants.DiscriminatorFieldName} != other.{Constants.DiscriminatorFieldName})");
        _builder.AppendLine($"{_bodyIndentation}{{");
        _builder.AppendLine($"{_bodyIndentation}    return false;");
        _builder.AppendLine($"{_bodyIndentation}}}");
        _builder.AppendLine();
        _builder.AppendLine($"{_bodyIndentation}return {Constants.DiscriminatorFieldName} switch");
        _builder.AppendLine($"{_bodyIndentation}{{");

        foreach (var variant in union.Variants)
        {
            _builder.AppendLine($"{_bodyIndentation}    {Constants.DiscriminatorTypeName}.{variant.MemberName} => global::System.Collections.Generic.EqualityComparer<{variant.StorageTypeName}>.Default.Equals({variant.FieldName}, other.{variant.FieldName}),");
        }

        _builder.AppendLine($"{_bodyIndentation}    _ => true,");
        _builder.AppendLine($"{_bodyIndentation}}};");
        _builder.AppendLine($"{_memberIndentation}}}");
        _builder.AppendLine();

        _builder.AppendLine($"{_memberIndentation}public override bool Equals(object? obj) => obj is {fullTypeName} other && Equals(other);");
        _builder.AppendLine();

        _builder.AppendLine($"{_memberIndentation}public override int GetHashCode()");
        _builder.AppendLine($"{_memberIndentation}{{");
        _builder.AppendLine($"{_bodyIndentation}var hash = new global::System.HashCode();");
        _builder.AppendLine($"{_bodyIndentation}hash.Add({Constants.DiscriminatorFieldName});");
        _builder.AppendLine($"{_bodyIndentation}switch ({Constants.DiscriminatorFieldName})");
        _builder.AppendLine($"{_bodyIndentation}{{");

        foreach (var variant in union.Variants)
        {
            _builder.AppendLine($"{_bodyIndentation}    case {Constants.DiscriminatorTypeName}.{variant.MemberName}:");
            _builder.AppendLine($"{_bodyIndentation}        hash.Add({variant.FieldName});");
            _builder.AppendLine($"{_bodyIndentation}        break;");
        }

        _builder.AppendLine($"{_bodyIndentation}}}");
        _builder.AppendLine();
        _builder.AppendLine($"{_bodyIndentation}return hash.ToHashCode();");
        _builder.AppendLine($"{_memberIndentation}}}");
        _builder.AppendLine();

        _builder.AppendLine($"{_memberIndentation}public static bool operator ==({fullTypeName} left, {fullTypeName} right) => left.Equals(right);");
        _builder.AppendLine();
        _builder.AppendLine($"{_memberIndentation}public static bool operator !=({fullTypeName} left, {fullTypeName} right) => !left.Equals(right);");
        _builder.AppendLine();

        return this;
    }

    public UnionBuilder EndType()
    {
        _builder.AppendLine($"{_typeIndentation}}}");

        return this;
    }

    /// <summary>
    /// Returns <c>Name</c> for non-generic unions, or <c>Name&lt;T, TError&gt;</c>
    /// (using the declared type parameter names) for generic ones.
    /// </summary>
    private static string GetFullTypeName(UnionToGenerate union)
    {
        if (union.TypeParameters.IsDefaultOrEmpty)
        {
            return union.Name;
        }

        var names = new string[union.TypeParameters.Length];
        for (var i = 0; i < union.TypeParameters.Length; i++)
        {
            names[i] = union.TypeParameters[i].Name;
        }

        return $"{union.Name}<{string.Join(", ", names)}>";
    }
}
