using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LSituationPort
{
    IReadOnlyList<LCatalogSituation> LEngineSituationFind(LVista vista, string unknown = "", string untitled = "");
}
