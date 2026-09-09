namespace Llyn.Core;

public sealed record LSynonymDraft(
    string LSynonymDraftEntry = "",
    string LSynonymDraftMeaning = "")
{
    public string LSynonymDraftEntry { get; init; } = LSynonymDraftEntry ?? string.Empty;

    public string LSynonymDraftMeaning { get; init; } = LSynonymDraftMeaning ?? string.Empty;

    public bool LSynonymDraftEmpty =>
        LSynonymDraftEntry.Length == 0 && LSynonymDraftMeaning.Length == 0;
}
