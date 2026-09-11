using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private static IReadOnlyDictionary<string, LTranslationTarget> PEditorTargetEmpty =>
        new Dictionary<string, LTranslationTarget>(StringComparer.Ordinal);

    private IReadOnlyDictionary<string, LTranslationTarget> PEditorTargetRead(LEntryDraft draft)
    {
        List<long> ids = [];
        PEditorTargetRead(draft.LEntryDraftMeanings, ids);
        PEditorTargetRead(draft.LEntryDraftCollocations, ids);

        Dictionary<string, LTranslationTarget> targets = new(StringComparer.Ordinal);
        if (ids.Count == 0)
        {
            return targets;
        }

        try
        {
            foreach (LTranslationTarget target in _lEngine.LEngineTargetRead(ids))
            {
                targets[target.LTranslationTargetId] = target;
            }
        }
        catch (Exception)
        {
            targets.Clear();
        }

        return targets;
    }

    private static void PEditorTargetRead(IReadOnlyList<LCardDraft> cards, List<string> ids)
    {
        foreach (LCardDraft card in cards)
        {
            foreach (long id in card.LCardDraftTranslation)
            {
                if (!ids.Contains(id))
                {
                    ids.Add(id);
                }
            }
        }
    }
}
