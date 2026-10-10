using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LReflexPort
{
    bool LEngineReflexCheck(long entryId);

    void LEngineReflexRebuild(long entryId);

    bool LEngineSpreadCheck(long entryId);

    void LEngineReflexSpread(long entryId, bool opened);

    bool LEngineBoxCheck(long entryId, LFoldBox box);

    void LEngineBoxSpread(long entryId, LFoldBox box, bool opened);

    IReadOnlyList<LReflexGuise> LEngineGuiseRead(string language, IReadOnlyList<string> reflexes);
}
