using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LReflexPort
{
    bool LEngineReflexCheck(long entryId);

    void LEngineReflexRebuild(long entryId);

    IReadOnlyList<LReflexGuise> LEngineGuiseRead(string language, IReadOnlyList<string> reflexes);
}
