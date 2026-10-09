using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LParadigmPort
{
    bool LEngineInflectionCheck(long entryId);

    IReadOnlyList<LParadigmRow> LEngineParadigmScan(long entryId);

    string LEngineLanguageResolve(long entryId);

    LParadigmStatus LEngineParadigmCheck(LParadigmRow row, bool pending, bool enabled);

    LParadigmView? LEngineInflectionRead(long entryId, bool pending, bool enabled);
}
