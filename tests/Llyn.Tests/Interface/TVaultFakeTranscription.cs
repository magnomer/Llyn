using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TVaultFakeTranscription : LTranscriptionVault
{
    private readonly Dictionary<long, IReadOnlyList<LTranscription>> _tVaultFakeRows = [];

    public IReadOnlyList<LTranscription> LTranscriptionRead(long entryId) =>
        _tVaultFakeRows.TryGetValue(entryId, out IReadOnlyList<LTranscription>? rows) ? rows : [];

    public IReadOnlyList<LTranscription> LTranscriptionSet(long entryId, IReadOnlyList<LTranscription> transcriptions)
    {
        List<LTranscription> stored = [];
        foreach (LTranscription row in transcriptions)
        {
            stored.Add(row with { LTranscriptionId = stored.Count + 1, LTranscriptionEntryId = entryId });
        }

        _tVaultFakeRows[entryId] = stored;
        return stored;
    }
}
