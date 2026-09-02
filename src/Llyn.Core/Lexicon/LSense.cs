namespace Llyn.Core;

public sealed record LSense(
    string LSenseId,
    string LSenseEntryId,
    string? LSenseParentId,
    int LSensePosition,
    LStateValue LSenseTitle,
    string? LSenseGloss,
    string? LSenseDefinitionLanguage,
    LStateValue LSenseDefinition,
    string LSenseLabels)
{
    public LStateValue LSenseTitle { get; init; } = LSenseTitle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LSenseDefinition { get; init; } = LSenseDefinition ?? LStateValue.LStateValueUnspecified;
}

