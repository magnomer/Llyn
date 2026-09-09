namespace Llyn.Core;

public sealed record LRelationDraft(
    string LRelationDraftType,
    string? LRelationDraftLabel,
    string? LRelationDraftLabels,
    string LRelationDraftEntry = "",
    string LRelationDraftMeaning = "")
{
    public string LRelationDraftType { get; init; } = LRelationDraftType ?? string.Empty;

    public string LRelationDraftEntry { get; init; } = LRelationDraftEntry ?? string.Empty;

    public string LRelationDraftMeaning { get; init; } = LRelationDraftMeaning ?? string.Empty;

    public bool LRelationDraftEmpty =>
        LRelationDraftEntry.Length == 0 && LRelationDraftMeaning.Length == 0;
}
