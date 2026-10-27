namespace Convention.Tests;

internal sealed record TAuditModuleFinding(
    string TAuditKind,
    string TAuditProject,
    string TAuditModule,
    string TAuditPath,
    string TAuditSpan,
    string TAuditSymbol)
{
    public string TAuditPair => $"{TAuditKind}:{TAuditProject}/{TAuditModule}";

    public string TAuditKey => string.IsNullOrEmpty(TAuditPath)
        ? TAuditPair
        : $"{TAuditPair}:{TAuditPath}:{TAuditSpan}:{TAuditSymbol}";
}

