using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LReferencePort
{
    IReadOnlyList<LCatalogReference> LEngineReferenceFind(LVista vista);

    IReadOnlyList<LCatalogReference> LEngineReferenceFind();

    IReadOnlyDictionary<long, string> LEngineCitationRead(LEntryDraft shown);

    string LEngineCitationRead(LDraft? draft);

    LColophon LEngineColophonRead(LDraft draft);

    LImprint LEngineImprintRead(LDraft? draft);

    IReadOnlyList<(string LReferenceKindTag, string LReferenceKindKey)> LEngineKindRead();
}
