namespace Convention.Tests;

internal sealed record TAuditModuleRow(
    string TAuditProject,
    string TAuditName,
    string TAuditEntrance,
    IReadOnlyList<string> TAuditFiles,
    IReadOnlyList<string> TAuditContracts)
{
    public string TAuditKey => $"{TAuditProject}/{TAuditName}";
}

