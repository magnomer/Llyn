using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LVistaPort
{
    IReadOnlyList<LVistaRow> LEngineEntryFind(LVista vista);

    IReadOnlyList<LVistaRow> LEngineEntryFind(LVista? parent, LVista? child);

    IReadOnlyList<string> LEngineNameResolve(IReadOnlyList<string> labels);

    LDraft? LEngineVistaLoad(LVista vista);

    LDraft? LEngineVistaLoad(LVista vista, long? id);

    string LEngineFileRead(LVista? vista);

    void LEngineSideSave(LVista vista);

    int LEngineUsageRead(LVista vista);

    string LEngineTallyRead(LVista vista);

    LRevision? LEngineVistaDelete(LVista vista);
}
