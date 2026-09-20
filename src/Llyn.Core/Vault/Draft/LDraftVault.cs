using System.Collections.Generic;

namespace Llyn.Core;

public interface LDraftVault
{
    void LDraftSave(LDraft draft);

    LDraft? LDraftRead(long id);

    IReadOnlyList<LDraft> LDraftScan();

    void LDraftDelete(long id);

    IReadOnlyList<long> LDraftSweep();
}
