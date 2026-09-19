using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TVaultFake : LEntryVault
{
    private readonly Dictionary<long, LEntry> _tVaultFakeRows = [];

    internal int TVaultFakeReads { get; private set; }

    internal LEntry TVaultFakeAdd(LEntry entry)
    {
        LEntry stored = entry with { LEntryId = _tVaultFakeRows.Count + 1 };
        _tVaultFakeRows[stored.LEntryId] = stored;
        return stored;
    }

    public LEntry LEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches) =>
        TVaultFakeAdd(entry);

    public LEntry? LEntryRead(long id)
    {
        TVaultFakeReads++;
        return _tVaultFakeRows.GetValueOrDefault(id);
    }

    public LEntryDraft? LEntryLoad(long id) => throw new NotSupportedException();

    public IReadOnlyList<LForm> LEntryFormRead(long id) => [];

    public IReadOnlyList<LSpeech> LEntrySpeechRead(long id) => [];

    public void LEntryUpdate(LEntry entry) => throw new NotSupportedException();

    public void LEntryFormSet(long id, IReadOnlyList<LForm> forms) => throw new NotSupportedException();

    public void LEntrySpeechSet(long id, IReadOnlyList<LSpeech> speeches) => throw new NotSupportedException();

    public void LEntryUpdatedSet(long id) => throw new NotSupportedException();

    public string LEntryEpithetRead(long entryId) => string.Empty;

    public void LEntryEpithetSave(long entryId, string epithet) => throw new NotSupportedException();

    public void LEntryGraspSet(long entryId, int grasp) => throw new NotSupportedException();

    public void LEntryDelete(long id) => throw new NotSupportedException();

    public IReadOnlyList<LEntry> LEntryFind(string query) => [.. _tVaultFakeRows.Values];

    public IReadOnlyList<LEntry> LEntryHeadwordFind(string language, string headword) => [];

    public IReadOnlyList<LEntry> LEntryHeadwordScan(string language, string text) => [];

    public IReadOnlyList<LEntry> LEntryScan(IReadOnlyList<long> ids, string query) => [];

    public IReadOnlyDictionary<long, string> LEntryEpithetScan(IReadOnlyList<long> ids) =>
        new Dictionary<long, string>();

    public IReadOnlyList<LEntry> LEntryTagFind(long tagId) => [];

    public IReadOnlyList<LEntry> LEntryRegisterFind(long registerId) => [];

    public IReadOnlyList<LEntry> LEntrySituationFind(long situationId) => [];

    public IReadOnlyList<LEntry> LEntryExampleFind(long exampleId) => [];

    public IReadOnlyList<LEntry> LEntryReferenceFind(long referenceId) => [];

    public long LEntryCountRead() => _tVaultFakeRows.Count;
}
