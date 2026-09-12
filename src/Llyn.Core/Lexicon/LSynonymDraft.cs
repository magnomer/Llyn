namespace Llyn.Core;

public sealed record LSynonymDraft(
    LStateAnchor LSynonymDraftEntry,
    LStateAnchor LSynonymDraftMeaning,
    long LSynonymDraftId = 0)
{
    public LStateAnchor LSynonymDraftEntry { get; init; } =
        LSynonymDraftEntry ?? LStateAnchor.LStateAnchorUnspecified;

    public LStateAnchor LSynonymDraftMeaning { get; init; } =
        LSynonymDraftMeaning ?? LStateAnchor.LStateAnchorUnspecified;

    public bool LSynonymDraftEmpty =>
        LSynonymDraftEntry.LStateAnchorEmpty && LSynonymDraftMeaning.LStateAnchorEmpty;
}
