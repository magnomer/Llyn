using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TVaultFakePronunciation : LPronunciationVault
{
    private readonly Dictionary<long, LPronunciation> _tVaultFakeRows = [];

    public LPronunciation LPronunciationCreate(LPronunciation pronunciation)
    {
        LPronunciation stored = pronunciation with { LPronunciationId = _tVaultFakeRows.Count + 1 };
        _tVaultFakeRows[stored.LPronunciationId] = stored;
        return stored;
    }

    public IReadOnlyList<LPronunciation> LPronunciationRead(long entryId) =>
        [.. _tVaultFakeRows.Values.Where(row => row.LPronunciationEntryId == entryId)];

    public void LPronunciationUpdate(LPronunciation pronunciation) =>
        _tVaultFakeRows[pronunciation.LPronunciationId] = pronunciation;

    public void LPronunciationOrderSet(long entryId, IReadOnlyList<long> order)
    {
    }

    public void LPronunciationDelete(long id) => _tVaultFakeRows.Remove(id);

    public void LPronunciationAudioSave(long pronunciationId, string file, string? source) =>
        throw new NotSupportedException();

    public LPronunciationAudio? LPronunciationAudioRead(long pronunciationId) => null;

    public IReadOnlyList<string> LPronunciationAudioScan() => [];

    public long? LPronunciationHolderRead(long id) =>
        _tVaultFakeRows.TryGetValue(id, out LPronunciation? row) ? row.LPronunciationEntryId : null;
}
