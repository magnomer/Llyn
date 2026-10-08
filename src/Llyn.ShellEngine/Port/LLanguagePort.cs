using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LLanguagePort
{
    IReadOnlyList<int> LEngineContourScale { get; }

    void LEngineSoundStart(long entryId);

    IReadOnlyList<LContour> LEngineContourRead(string language, string ipa);

    bool LEngineSilentCheck(string language);

    LAccentSheet LEngineAccentRead(LEntryDraft draft);

    Task<LAccentSheet> LEngineAccentLoad(
        LEntryDraft draft, Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store);
}
