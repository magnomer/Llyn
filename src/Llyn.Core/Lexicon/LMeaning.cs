namespace Llyn.Core;

public sealed record LMeaning(
    long LMeaningId,
    long LMeaningEntryId,
    long? LMeaningParentId,
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

