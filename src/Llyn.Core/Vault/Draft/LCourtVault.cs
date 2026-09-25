using System.Collections.Generic;

namespace Llyn.Core;

public interface LCourtVault
{
    void LCourtSave(LCourt link);

    IReadOnlyList<LCourt> LCourtScan();

    void LCourtDelete(long id);

    IReadOnlyList<LCourt> LCourtSettle(long draftId);

    void LCourtSweep();
}
