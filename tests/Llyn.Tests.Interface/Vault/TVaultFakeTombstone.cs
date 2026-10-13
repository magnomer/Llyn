using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TVaultFakeTombstone : LTombstoneVault
{
    private readonly Dictionary<long, LTombstone> _tVaultFakeRows = [];

    public LTombstone LTombstoneRecord(long entryId, long revisionId)
    {
        LTombstone stone = new(entryId, revisionId, DateTimeOffset.UtcNow.ToString("O"));
        _tVaultFakeRows[entryId] = stone;
        return stone;
    }

    internal LTombstone? TTombstoneRead(long entryId) => _tVaultFakeRows.GetValueOrDefault(entryId);
}
