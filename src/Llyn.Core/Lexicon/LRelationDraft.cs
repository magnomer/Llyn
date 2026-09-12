namespace Llyn.Core;

public sealed record LRelationDraft(
    string LRelationDraftType,
    string? LRelationDraftLabel,
    string? LRelationDraftLabels,
    LStateAnchor LRelationDraftEntry,
    LStateAnchor LRelationDraftMeaning,
    long LRelationDraftId = 0)
{
    public string LRelationDraftType { get; init; } = LRelationDraftType ?? string.Empty;

    public LStateAnchor LRelationDraftEntry { get; init; } =
        LRelationDraftEntry ?? LStateAnchor.LStateAnchorUnspecified;

    public LStateAnchor LRelationDraftMeaning { get; init; } =
        LRelationDraftMeaning ?? LStateAnchor.LStateAnchorUnspecified;

    public bool LRelationDraftEmpty =>
        LRelationDraftEntry.LStateAnchorEmpty && LRelationDraftMeaning.LStateAnchorEmpty;
}
