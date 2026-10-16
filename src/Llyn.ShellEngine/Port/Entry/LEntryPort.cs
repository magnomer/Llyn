using System.Collections.Generic;
using System.Globalization;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LEntryPort
{
    LEntryDraft? LEngineEntryLoad(long id);

    (bool, string, string) LEngineStampRead(long entryId, CultureInfo culture);

    string LEngineTallyRead(long? reference);

    IReadOnlyList<LReflexDraft> LEngineReflexRead(LEntryDraft draft);

    long LEngineGlyphResolve(string character, string language);

    (LOwner, int)? LEngineCardFind(LEntryDraft draft, long id);

    static string LEngineUnitFormat(LUnit unit)
    {
        return LUnitClerk.LUnitFormat(unit);
    }

    static string LEngineFreedomFormat(LFreedom freedom)
    {
        return LFreedomClerk.LFreedomFormat(freedom);
    }
}
