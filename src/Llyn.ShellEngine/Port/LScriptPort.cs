using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LScriptPort
{
    bool LEngineScriptCheck(long entryId);

    void LEngineScriptRebuild(long entryId);

    IReadOnlyList<LScriptGroup> LEngineScriptDivide(long entryId);

    IReadOnlyList<LScriptGroup> LEngineScriptRead(long entryId);

    bool LEngineStyleCheck(string language);
}
