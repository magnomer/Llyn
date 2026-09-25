using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TVaultFakeRevision : LRevisionVault
{
    private readonly Dictionary<long, IReadOnlyList<LRevisionChange>> _tVaultFakeRows = [];

    public LRevision LRevisionRecord(IReadOnlyList<LRevisionChange> changes)
    {
        long id = _tVaultFakeRows.Count + 1;
        _tVaultFakeRows[id] = changes;
        return new LRevision(id, DateTimeOffset.UtcNow.ToString("O"));
    }

    internal IReadOnlyList<LRevisionChange> TRevisionChangeRead(long revisionId) =>
        _tVaultFakeRows.GetValueOrDefault(revisionId, []);
}
