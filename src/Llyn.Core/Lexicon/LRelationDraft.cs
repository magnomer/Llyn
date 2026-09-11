namespace Llyn.Core;

public sealed record LRelationDraft(
    string LRelationDraftType,
    string? LRelationDraftLabel,
    string? LRelationDraftLabels,
    long LRelationDraftEntry = 0,
    long LRelationDraftMeaning = 0)
{
    public string LRelationDraftType { get; init; } = LRelationDraftType ?? string.Empty;

    public bool LRelationDraftEmpty =>
        LRelationDraftEntry == 0 && LRelationDraftMeaning == 0;
}
