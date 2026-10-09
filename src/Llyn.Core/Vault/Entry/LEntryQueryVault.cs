using System.Collections.Generic;

namespace Llyn.Core;

public interface LEntryQueryVault
{
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
