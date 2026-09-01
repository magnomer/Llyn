namespace Llyn.Core;

public sealed record LSense(
    string LSenseId,
    string LSenseEntryId,
    string? LSenseParentId,
    int LSensePosition,
    string? LSenseTitle,
    string? LSenseGloss,
    string? LSenseDefinitionLanguage,
    string? LSenseDefinition,
    string LSenseLabels);
