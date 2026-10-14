using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LAncestorDraft(
    long LAncestorDraftId,
    long? LAncestorDraftChild = null,
    long? LAncestorDraftTarget = null,
    string LAncestorDraftText = "",
    string LAncestorDraftLanguage = "",
    long? LAncestorDraftTongue = null,
    string LAncestorDraftGloss = "",
    long? LAncestorDraftRelation = null)
{
    public string LAncestorDraftText { get; init; } = LAncestorDraftText ?? string.Empty;

    public string LAncestorDraftLanguage { get; init; } = LAncestorDraftLanguage ?? string.Empty;

    public string LAncestorDraftGloss { get; init; } = LAncestorDraftGloss ?? string.Empty;

    public static IReadOnlyList<LAncestorDraft> LAncestorDraftCreate(IReadOnlyList<LAncestor> ancestors)
    {
        List<LAncestorDraft> drafts = new(ancestors.Count);
        foreach (LAncestor ancestor in ancestors)
        {
            drafts.Add(new LAncestorDraft(
                ancestor.LAncestorId,
                ancestor.LAncestorChildId,
                ancestor.LAncestorTargetId,
                ancestor.LAncestorText,
                ancestor.LAncestorLanguage,
                ancestor.LAncestorTongueId,
                ancestor.LAncestorGloss,
                ancestor.LAncestorRelationId));
        }

        return drafts;
    }

    public static IReadOnlyList<LAncestor> LAncestorDraftResolve(IReadOnlyList<LAncestorDraft> drafts)
    {
        Dictionary<long, int> siblings = [];
        List<LAncestor> ancestors = new(drafts.Count);
        foreach (LAncestorDraft draft in drafts)
        {
            long child = draft.LAncestorDraftChild ?? 0;
            int position = siblings.GetValueOrDefault(child);
            siblings[child] = position + 1;
            ancestors.Add(new LAncestor(
                draft.LAncestorDraftId,
                draft.LAncestorDraftChild,
                position,
                draft.LAncestorDraftTarget,
                draft.LAncestorDraftText,
                draft.LAncestorDraftLanguage,
                draft.LAncestorDraftTongue,
                draft.LAncestorDraftGloss,
                draft.LAncestorDraftRelation));
        }

        return ancestors;
    }
}
