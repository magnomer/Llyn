using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LRegisterPort
{
    IReadOnlyList<LCatalogRegister> LEngineRegisterFind(LVista vista);

    LRegister LEngineRegisterCreate(string name);
}
