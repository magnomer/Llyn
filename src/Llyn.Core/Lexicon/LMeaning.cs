namespace Llyn.Core;

public sealed record LMeaning(
    string LMeaningId,
    string LMeaningEntryId,
    string? LMeaningParentId,
    int LMeaningPosition,
    LStateValue LMeaningTitle,
    string? LMeaningGloss,
    string? LMeaningDefinitionLanguage,
    LStateValue LMeaningDefinition,
    string LMeaningLabels)
{
    public LStateValue LMeaningTitle { get; init; } = LMeaningTitle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LMeaningDefinition { get; init; } = LMeaningDefinition ?? LStateValue.LStateValueUnspecified;
}

