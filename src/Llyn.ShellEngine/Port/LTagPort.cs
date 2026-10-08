using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LTagPort
{
    IReadOnlyList<LCatalogTag> LEngineTagFind(LVista vista);

    LTag LEngineTagCreate(string text);
}
