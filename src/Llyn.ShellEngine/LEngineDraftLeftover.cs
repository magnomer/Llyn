using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public void LEngineLeftoverSweep()
    {
        lock (_lEngineGate)
        {
            LDraftArchive.LDraftArchiveSweep(_lEngineWorkspace);
            LCourtArchive.LCourtArchiveSweep(_lEngineWorkspace);

            foreach (LDraft draft in LDraftArchive.LDraftArchiveScan(_lEngineWorkspace))
            {
                if (_lEngineDraftHeld.Contains(draft.LDraftId)
                    || LEngineClaimCheck(draft.LDraftId)
                    || string.IsNullOrWhiteSpace(draft.LDraftEntry))
                {
                    continue;
                }

                if (draft.LDraftExample is LExample sentence)
                {
                    if (LEngineExampleRead(draft.LDraftEntry) is LExample kept
                        && LEngineExampleMatch(kept, sentence))
                    {
                        LEngineDraftCancel(draft.LDraftId);
                    }

                    continue;
                }

                if (draft.LDraftSituation is LSituation situation)
                {
                    if (LEngineSituationRead(draft.LDraftEntry) is LSituation standing
                        && LEngineSituationMatch(standing, situation))
                    {
                        LEngineDraftCancel(draft.LDraftId);
                    }

                    continue;
                }

                if (draft.LDraftReference is LReference reference)
                {
                    if (LEngineReferenceRead(draft.LDraftEntry) is LReference cited
                        && LEngineReferenceMatch(cited, reference))
                    {
                        LEngineDraftCancel(draft.LDraftId);
                    }

                    continue;
                }

                LEntryDraft? stored = LEngineEntryLoad(draft.LDraftEntry);
                if (stored is null || !LEngineDraftMatch(stored, draft.LDraftContent))
                {
                    continue;
                }

                LEngineDraftCancel(draft.LDraftId);
            }
        }
    }

    public IReadOnlyList<LDraft> LEngineLeftoverRead()
    {
        lock (_lEngineGate)
        {
            List<LDraft> leftovers = [];
            foreach (LDraft draft in LDraftArchive.LDraftArchiveScan(_lEngineWorkspace))
            {
                if (_lEngineDraftHeld.Contains(draft.LDraftId)
                    || !LEngineDraftCheck(draft.LDraftId)
                    || LEngineClaimCheck(draft.LDraftId))
                {
                    continue;
                }

                leftovers.Add(draft);
            }

            return leftovers;
        }
    }
}
