using System.Collections.Generic;

namespace Llyn.Core;

public interface LDraftVault
{
    LDraft LDraftStart(LDraft draft);

    LDraft? LDraftRead(long id);

    IReadOnlyList<LDraft> LDraftScan();

    void LDraftSave(LDraft draft);

    bool LDraftClaimCheck(long id);

    bool LDraftForeignCheck(long id);

    void LDraftFinish(long id);

    LDraftRelease LDraftDelete(long id, IReadOnlySet<long> held);

    LDraftRelease LDraftCancel(long id, IReadOnlySet<long> held);

    LDraftRelease LDraftSweep(IReadOnlySet<long> held);

    void LDraftCourtSave(LCourt link);

    IReadOnlyList<LCourt> LDraftCourtScan();

    void LDraftCourtDelete(long linkId);

    IReadOnlyList<long> LDraftCourtSettle(long draftId, long realId);
}
