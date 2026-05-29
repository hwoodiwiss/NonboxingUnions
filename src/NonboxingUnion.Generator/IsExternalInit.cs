#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices;

using System.ComponentModel;

/// <summary>
/// Polyfill for <c>System.Runtime.CompilerServices.IsExternalInit</c>, which is
/// required by the compiler to support <c>init</c> accessors and positional
/// records when targeting <c>netstandard2.0</c>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
internal static class IsExternalInit;
#endif
