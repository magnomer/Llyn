using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private void LEngineWorkspaceUpdate()
    {
        using LVaultSession session = _lEngineVault.LVaultSessionStart();
        foreach (LEntry entry in _lEngineEntries.LEntryFind(string.Empty))
        {
            try
            {
                LEngineRespellingUpdate(entry);
                LEngineEpithetUpdate(entry.LEntryId);
                LEngineParadigmUpdate(entry);
            }
            catch (Exception exception) when (exception is not OutOfMemoryException)
            {
                _lEngineAudit.LAuditRecord(exception);
            }
        }

        session.LVaultSessionCommit();
    }

    private void LEngineRespellingUpdate(LEntry entry)
    {
        LPronunciationVault pronunciations = _lEnginePronunciations;
        foreach (LPronunciation row in pronunciations.LPronunciationRead(entry.LEntryId))
        {
            if (!string.IsNullOrEmpty(row.LPronunciationRespelling) || string.IsNullOrEmpty(row.LPronunciationIpa))
            {
                continue;
            }

            LPronunciationDraft spoken = _lEngineLanguageCache.LLanguageRespellingResolve(
                entry.LEntryLanguage,
                new LPronunciationDraft(
                    row.LPronunciationIpa, LPronunciationDraftVariety: row.LPronunciationVariety ?? string.Empty));
            if (spoken.LPronunciationDraftRespelling.Length > 0)
            {
                pronunciations.LPronunciationUpdate(
                    row with { LPronunciationRespelling = spoken.LPronunciationDraftRespelling });
            }
        }

        LReflexVault reflexes = _lEngineReflexes;
        IReadOnlyList<LReflex> stored = reflexes.LReflexRead(entry.LEntryId);
        List<LReflex> spelled = new(stored.Count);
        bool changed = false;
        foreach (LReflex row in stored)
        {
            string respelling = row.LReflexRespelling.Length > 0 || row.LReflexText.Length == 0
                ? row.LReflexRespelling
                : _lEngineLanguageCache.LLanguageRespellingResolve(
                    new LReflexDraft(row.LReflexLanguage, LReflexDraftText: row.LReflexText)).LReflexDraftRespelling;
            LAnatomy anatomy = row.LReflexText.Length == 0
                ? row.LReflexAnatomy
                : _lEngineLanguageCache.LLanguageAnatomyResolve(
                    entry.LEntryLanguage,
                    new LReflexDraft(
                        row.LReflexLanguage,
                        LReflexDraftText: row.LReflexText,
                        LReflexDraftRespelling: respelling)).LReflexDraftAnatomy;
            changed |= respelling.Length != row.LReflexRespelling.Length || anatomy != row.LReflexAnatomy;
            spelled.Add(row with { LReflexRespelling = respelling, LReflexAnatomy = anatomy });
        }

        if (changed)
        {
            reflexes.LReflexSet(entry.LEntryId, spelled);
        }
    }
}
