using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TVaultFakeReflex : LReflexVault
{
    private readonly Dictionary<long, IReadOnlyList<LReflex>> _tVaultFakeRows = [];

    public IReadOnlyList<LReflex> LReflexRead(long entryId) =>
        _tVaultFakeRows.TryGetValue(entryId, out IReadOnlyList<LReflex>? rows) ? rows : [];

    public IReadOnlyDictionary<long, IReadOnlyList<LReflex>> LReflexAnchorScan(long diweiId) =>
        new Dictionary<long, IReadOnlyList<LReflex>>();

    public IReadOnlyList<LReflex> LReflexSet(long entryId, IReadOnlyList<LReflex> reflexes)
    {
        List<LReflex> stored = [];
        foreach (LReflex row in reflexes)
        {
            stored.Add(row with { LReflexId = stored.Count + 1, LReflexEntryId = entryId });
        }

        _tVaultFakeRows[entryId] = stored;
        return stored;
    }
}
