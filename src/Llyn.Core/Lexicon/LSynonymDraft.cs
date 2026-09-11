namespace Llyn.Core;

public sealed record LSynonymDraft(
    long LSynonymDraftEntry = 0,
    long LSynonymDraftMeaning = 0)
{
    public bool LSynonymDraftEmpty =>
        LSynonymDraftEntry == 0 && LSynonymDraftMeaning == 0;
}
