# Nonboxing Unions

[![NuGet](https://img.shields.io/nuget/v/NonboxingUnion.svg?maxAge=3600)](https://www.nuget.org/packages/NonboxingUnion/)

> :warning: This package is currently in preview, and is subject to change.

## Background

C# is gaining first-class [discriminated unions](https://github.com/dotnet/csharplang/blob/main/proposals/TypeUnions.md). A union is a value that can be exactly one of a fixed, known set of case types at any given time, and which can be exhaustively pattern matched.

The natural way to model "one of several types" today is to store the value in an `object` field. That works, but it forces every value-type case (`int`, `bool`, your own structs, …) to be **boxed** onto the heap every time it is placed into the union, and unboxed every time it is read back out. For hot paths and allocation-sensitive code, that overhead defeats much of the point of using a value type in the first place.

The runtime exposes `[System.Runtime.CompilerServices.Union]` and `System.Runtime.CompilerServices.IUnion` so that a type can opt in to the union language feature (implicit conversions from each case type, and exhaustive pattern matching). But it leaves the *storage strategy* up to you. The boxing implementation the compiler can generate for you stores everything in a single `object?` field.

That's where Nonboxing Unions comes in. With Nonboxing Unions, you annotate a `partial struct` with the case types it should hold, and a source generator emits a union whose storage keeps each value-type case **unboxed**:

- a discriminator that records which case is active, backed by the **smallest unsigned integer** that can represent the cases;
- one strongly-typed field per case, so value types are stored inline with no heap allocation and **no memory overlap** (`StructLayout`/`FieldOffset` are deliberately avoided, as overlapping unrelated fields hurts the JIT and is unsound for fields containing references);
- the union access members (`Value`, `HasValue`, `TryGetValue`), value equality, and a constructor for each case;
- the `[Union]` attribute and `IUnion` implementation, so the type participates in the C# union language feature (implicit conversions and exhaustive pattern matching).

## Usage

Mark a `partial struct` with `[NonBoxingUnion(...)]`, passing the case types as `typeof(...)` arguments. The struct body can be left empty &mdash; the generator fills in everything.

```csharp
using NonboxingUnion;

[NonBoxingUnion(typeof(int), typeof(bool))]
public partial struct IntOrBool;
```

**Assigning a case** uses the implicit conversions provided by the union language feature:

```csharp
IntOrBool union = 42;     // holds an int
union = true;             // now holds a bool
```

**Pattern matching** is exhaustive over the case types, plus `null` for the uninitialized (`default`) state:

```csharp
string description = union switch
{
    int i  => $"int {i}",
    bool b => $"bool {b}",
    null   => "no value",
};
```

Because each case is stored in its own typed field, matching `int i` reads an `int` directly with no unboxing.

**The generated access members:**

```csharp
IntOrBool union = 42;

union.HasValue;                       // true  (false for default)
union.Value;                          // object? -> 42 (boxed only when you ask for it)

if (union.TryGetValue(out int value)) // true; value == 42
{
    // ...
}

union.TryGetValue(out bool _);        // false; the active case is int, not bool
```

A `default(IntOrBool)` has no active case: `HasValue` is `false`, `Value` is `null`, and every `TryGetValue` returns `false`.

### Supported case types

| Case kind | Example | Notes |
| --- | --- | --- |
| Value types | `typeof(int)`, `typeof(MyStruct)` | Stored inline, never boxed. |
| Reference types | `typeof(string)`, `typeof(Dog)` | Stored in a nullable field; the case type and constructor parameter stay non-nullable. |
| Nullable value types | `typeof(int?)` | Collapses to the underlying type (`int`) for the case type and `TryGetValue` out parameter, per the union spec. |
| Nested types | `typeof(Outer.Inner.Thing)` | Fully-qualified names are used, so nesting and namespaces are handled correctly. |

The union struct itself may be nested inside other types &mdash; all of its containing types must be `partial`.

For more detail on storage layout, the discriminator, and the generated members, see [docs/generated-code.md](docs/generated-code.md).

## Requirements

- The annotated type must be a `partial struct`.
- It must declare at least one case type.
- Every containing type must also be `partial`.
- A target framework whose runtime provides `System.Runtime.CompilerServices.UnionAttribute` and `IUnion`, with the C# union language feature enabled.
