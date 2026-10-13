using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TVaultFakeClaim : LClaimVault
{
    private readonly Dictionary<long, LClaim> _tVaultFakeRows = [];
    private int _tVaultFakeProcess;

    internal TVaultFakeClaim(int process)
    {
        _tVaultFakeProcess = process;
    }

    public LClaim LClaimCreate(long draftId) => new(draftId, _tVaultFakeProcess, DateTimeOffset.UtcNow);

    public void LClaimSave(LClaim claim)
    {
        _tVaultFakeRows[claim.LClaimDraft] = claim;
    }

    public LClaim? LClaimRead(long draftId) => _tVaultFakeRows.GetValueOrDefault(draftId);

    public IReadOnlyList<LClaim> LClaimScan() => [.. _tVaultFakeRows.Values.OrderBy(claim => claim.LClaimDraft)];

    public void LClaimDelete(long draftId)
    {
        _tVaultFakeRows.Remove(draftId);
    }

    public bool LClaimCheck(long draftId) => _tVaultFakeRows.ContainsKey(draftId);

    internal void TClaimProcessSet(int process)
    {
        _tVaultFakeProcess = process;
    }
}
