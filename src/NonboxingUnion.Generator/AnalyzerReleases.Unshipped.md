; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
NBU001 | NonboxingUnion | Error | A non-boxing union must be declared partial
NBU002 | NonboxingUnion | Error | A non-boxing union must declare at least one case
NBU003 | NonboxingUnion | Error | A non-boxing union is nested in an unsupported type
