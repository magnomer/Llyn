using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public void LEngineLeftoverSweep()
    {
        lock (_lEngineGate)
        {
            IReadOnlyList<long> dropped = _lEngineDrafts.LDraftSweep();
            _lEngineCourts.LCourtSweep();

            foreach (long id in dropped)
            {
                LEngineDraftCancel(id);
            }

            foreach (LClaim claim in _lEngineClaims.LClaimScan())
            {
                if (_lEngineDrafts.LDraftRead(claim.LClaimDraft) is null)
                {
                    _lEngineClaims.LClaimDelete(claim.LClaimDraft);
                }
            }

            foreach (LDraft draft in _lEngineDrafts.LDraftScan())
            {
                if (_lEngineDraftHeld.Contains(draft.LDraftId)
                    || LEngineClaimCheck(draft.LDraftId))
                {
                    continue;
                }

                if (draft.LDraftEntryId <= 0)
                {
                    if (!LEngineDraftCheck(draft))
                    {
                        LEngineDraftCancel(draft.LDraftId);
                    }

                    continue;
                }

                if (draft.LDraftExample is LExample sentence)
                {
                    if (LEngineExampleRead(draft.LDraftEntryId) is LExample kept
                        && LEngineExampleMatch(kept, sentence))
                    {
                        LEngineDraftCancel(draft.LDraftId);
                    }

                    continue;
                }

                if (draft.LDraftSituation is LSituation situation)
                {
                    if (LEngineSituationRead(draft.LDraftEntryId) is LSituation standing
                        && LEngineSituationMatch(standing, situation))
                    {
                        LEngineDraftCancel(draft.LDraftId);
                    }

                    continue;
                }

                if (draft.LDraftReference is LReference reference)
                {
                    if (LEngineReferenceRead(draft.LDraftEntryId) is LReference cited
                        && LEngineReferenceMatch(cited, reference))
                    {
                        LEngineDraftCancel(draft.LDraftId);
                    }

                    continue;
                }

                if (draft.LDraftAuthorHeld is LAuthor author)
                {
                    if (!LEngineAuthorCheck(draft, author))
                    {
                        LEngineDraftCancel(draft.LDraftId);
                    }

                    continue;
                }

                LEntryDraft? stored = LEngineEntryLoad(draft.LDraftEntryId);
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
            foreach (LDraft draft in _lEngineDrafts.LDraftScan())
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
