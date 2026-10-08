using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LEntryPort
{
    LEntryDraft? LEngineEntryLoad(long id);

    (bool, string, string) LEngineStampRead(long entryId);

    string LEngineTallyRead(long? reference);

    IReadOnlyList<LReflexDraft> LEngineReflexRead(LEntryDraft draft);

    long LEngineGlyphResolve(string character, string language);

    string LEngineUnitFormat(LUnit unit);

    (LOwner, int)? LEngineCardFind(LEntryDraft draft, long id);
}
