using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LCardPort
{
    IReadOnlyList<LUsage> LEngineIncomingRead(long entryId);

    IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LEngineTranslationRead(LEntryDraft shown);

    LEtymologyResult LEngineEtymologyRead(LEntryDraft draft);

    IReadOnlySet<long> LEngineFoldRead(long entryId);

    void LEngineFoldSave(long entryId, long cardId);

    void LEngineFoldDelete(long entryId, long cardId);
}
