using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LStemPort
{
    long? LEngineStemFind(string language, string? key);

    IReadOnlyList<LStem> LEngineStemFind(LVista vista);

    bool LEngineStemCheck();

    LStemPage LEngineStemResolve(long? id);

    long LEngineStemResolve(long? id, string character);

    void LEngineStemSpread(long? id, string character, bool opened);

    IReadOnlyList<LVistaRow> LEngineKindredFind(LVista grove, LVista vista);
}
