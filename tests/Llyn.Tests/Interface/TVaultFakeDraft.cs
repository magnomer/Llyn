using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TVaultFakeDraft : LDraftVault
{
    private readonly Dictionary<long, LDraft> _tVaultFakeRows = [];
    private readonly HashSet<long> _tVaultFakeStale = [];

    public void LDraftSave(LDraft draft)
    {
        _tVaultFakeRows[draft.LDraftId] = draft;
    }

    public LDraft? LDraftRead(long id) => _tVaultFakeRows.GetValueOrDefault(id);

    public IReadOnlyList<LDraft> LDraftScan() => [.. _tVaultFakeRows.Values.OrderBy(draft => draft.LDraftId)];

    public void LDraftDelete(long id)
    {
        _tVaultFakeRows.Remove(id);
    }

    public IReadOnlyList<long> LDraftSweep()
    {
        List<long> dropped = [.. _tVaultFakeStale.Where(_tVaultFakeRows.ContainsKey)];
        _tVaultFakeStale.Clear();
        return dropped;
    }

    internal void TDraftStaleSet(long id)
    {
        _tVaultFakeStale.Add(id);
    }
}
