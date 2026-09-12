using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private static IReadOnlyDictionary<long, LTranslationTarget> PEditorTargetEmpty => new Dictionary<long, LTranslationTarget>();

    private IReadOnlyDictionary<long, LTranslationTarget> PEditorTargetRead(LEntryDraft draft)
    {
        List<long> ids = [];
        PEditorTargetRead(draft.LEntryDraftMeanings, ids);
        PEditorTargetRead(draft.LEntryDraftCollocations, ids);

        Dictionary<long, LTranslationTarget> targets = [];
        if (ids.Count == 0)
        {
            return targets;
        }

        try
        {
            foreach (LTranslationTarget target in _lEngine.LEngineTargetRead(_pEditorDraft, ids))
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

    private static void PEditorTargetRead(IReadOnlyList<LCardDraft> cards, List<long> ids)
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
