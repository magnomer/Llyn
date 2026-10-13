using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TVaultFakeCourt : LCourtVault
{
    private readonly Dictionary<long, LCourt> _tVaultFakeRows = [];

    public void LCourtSave(LCourt link)
    {
        _tVaultFakeRows[link.LCourtId] = link;
    }

    public IReadOnlyList<LCourt> LCourtScan() => [.. _tVaultFakeRows.Values.OrderBy(link => link.LCourtId)];

    public void LCourtDelete(long id)
    {
        _tVaultFakeRows.Remove(id);
    }

    public IReadOnlyList<LCourt> LCourtSettle(long draftId)
    {
        List<LCourt> settled = [.. _tVaultFakeRows.Values.Where(link => link.LCourtTargetId == draftId)];
        foreach (LCourt link in settled)
        {
            _tVaultFakeRows.Remove(link.LCourtId);
        }

        return settled;
    }

    public void LCourtSweep()
    {
    }
}
