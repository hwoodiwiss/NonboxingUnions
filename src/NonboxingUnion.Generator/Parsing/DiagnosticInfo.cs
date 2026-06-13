namespace NonboxingUnion.Generator.Parsing;

/// <summary>
/// A value-equatable description of a diagnostic to report. Reporting is
/// deferred to the source-output stage so that the extraction stage stays free
/// of <see cref="Diagnostic"/>/<see cref="Location"/> references that would
/// break incremental caching.
/// </summary>
internal sealed record DiagnosticInfo(
    DiagnosticDescriptor Descriptor,
    LocationInfo? Location,
    string? MessageArgument)
{
    public Diagnostic ToDiagnostic()
    {
        var location = Location?.ToLocation();

        return MessageArgument is null
            ? Diagnostic.Create(Descriptor, location)
            : Diagnostic.Create(Descriptor, location, MessageArgument);
    }
}
