using System.Collections.Generic;

namespace Llyn.Core;

public interface LClaimVault
{
    LClaim LClaimCreate(long draftId);

    void LClaimSave(LClaim claim);

    LClaim? LClaimRead(long draftId);

    IReadOnlyList<LClaim> LClaimScan();

    void LClaimDelete(long draftId);

    bool LClaimCheck(long draftId);
}
