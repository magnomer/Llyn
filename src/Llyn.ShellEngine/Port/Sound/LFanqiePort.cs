using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LFanqiePort
{
    bool LEngineFanqieCheck(long entryId);

    IReadOnlyList<LFanqieGroup> LEngineFanqieDivide(long entryId);

    IReadOnlyList<LFanqieGroup> LEngineFanqieRead(long entryId);

    string LEngineReadingRead(long entryId, string headword);

    void LEngineFanqieRebuild(long entryId);

    void LEngineFanqieSet(long entryId, long fanqieId, int rank, bool raise);

    bool LEngineBookCheck(string language);

    bool LEngineBookCheck();
}
