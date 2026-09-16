using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private void LEngineWorkspaceUpdate()
    {
        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();
        foreach (LEntry entry in new LEntryArchive(_lEngineDatabase).LEntryFind(string.Empty))
        {
            LEngineRespellingUpdate(entry);
            LEngineEpithetUpdate(entry.LEntryId);
            LEngineParadigmUpdate(entry);
        }

        session.LDatabaseSessionCommit();
    }

    private void LEngineRespellingUpdate(LEntry entry)
    {
        LPronunciationArchive pronunciations = new(_lEngineDatabase);
        foreach (LPronunciation row in pronunciations.LPronunciationRead(entry.LEntryId))
        {
            if (!string.IsNullOrEmpty(row.LPronunciationRespelling) || string.IsNullOrEmpty(row.LPronunciationIpa))
            {
                continue;
            }

            LPronunciationDraft spoken = LEngineRespellingResolve(
                entry.LEntryLanguage,
                new LPronunciationDraft(
                    row.LPronunciationIpa, LPronunciationDraftVariety: row.LPronunciationVariety ?? string.Empty));
            if (spoken.LPronunciationDraftRespelling.Length > 0)
            {
                pronunciations.LPronunciationUpdate(
                    row with { LPronunciationRespelling = spoken.LPronunciationDraftRespelling });
            }
        }

        LReflexArchive reflexes = new(_lEngineDatabase);
        IReadOnlyList<LReflex> stored = reflexes.LReflexRead(entry.LEntryId);
        List<LReflex> spelled = new(stored.Count);
        bool changed = false;
        foreach (LReflex row in stored)
        {
            string respelling = row.LReflexRespelling.Length > 0 || row.LReflexText.Length == 0
                ? row.LReflexRespelling
                : LEngineRespellingResolve(new LReflexDraft(row.LReflexLanguage, LReflexDraftText: row.LReflexText))
                    .LReflexDraftRespelling;
            changed |= respelling.Length != row.LReflexRespelling.Length;
            spelled.Add(row with { LReflexRespelling = respelling });
        }

        if (changed)
        {
            reflexes.LReflexSet(entry.LEntryId, spelled);
        }
    }
}
