using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LCardPort
{
    IReadOnlyList<LUsage> LEngineIncomingRead(long entryId);

    IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LEngineTranslationRead(LEntryDraft shown);

    LCollocationLayout LEngineCollocationRead(
        LEntryDraft shown, IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> targets);

    IReadOnlySet<(LCardKind, long)> LEngineFoldRead(long entryId);

    void LEngineFoldSave(long entryId, LCardKind kind, long cardId);

    void LEngineFoldDelete(long entryId, LCardKind kind, long cardId);

    (LOwner, int)? LEngineCardFind(LEntryDraft draft, LCardKind kind, long id);
}
