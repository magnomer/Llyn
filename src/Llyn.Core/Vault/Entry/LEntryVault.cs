using System.Collections.Generic;

namespace Llyn.Core;

public interface LEntryVault
{
    LEntry LEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches);

    LEntry? LEntryRead(long id);

    LEntryDraft? LEntryLoad(long id);

    IReadOnlyList<LForm> LEntryFormRead(long id);

    IReadOnlyList<LSpeech> LEntrySpeechRead(long id);

    void LEntryUpdate(LEntry entry);

    void LEntryFormSet(long id, IReadOnlyList<LForm> forms);

    void LEntrySpeechSet(long id, IReadOnlyList<LSpeech> speeches);

    void LEntryUpdatedSet(long id);

    string LEntryEpithetRead(long entryId);

    void LEntryEpithetSave(long entryId, string epithet);

    void LEntryGraspSet(long entryId, int grasp);

    void LEntryDelete(long id);

    IReadOnlyList<LEntry> LEntryFind(string query);

    IReadOnlyList<LEntry> LEntryHeadwordFind(string language, string headword);

    IReadOnlyList<LEntry> LEntryHeadwordScan(string language, string text);

    IReadOnlyList<LEntry> LEntryScan(IReadOnlyList<long> ids, string query);

    IReadOnlyDictionary<long, string> LEntryEpithetScan(IReadOnlyList<long> ids);

    IReadOnlyList<LEntry> LEntryTagFind(long tagId);

    IReadOnlyList<LEntry> LEntryRegisterFind(long registerId);

    IReadOnlyList<LEntry> LEntrySituationFind(long situationId);

    IReadOnlyList<LEntry> LEntryExampleFind(long exampleId);

    IReadOnlyList<LEntry> LEntryReferenceFind(long referenceId);

    long LEntryCountRead();
}
