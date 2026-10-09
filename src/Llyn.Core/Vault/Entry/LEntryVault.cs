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

    string LEntryEpithetRead(long entryId);

    void LEntryEpithetSave(long entryId, string epithet);

    void LEntryGraspSet(long entryId, int grasp);

    void LEntryDelete(long id);
}
