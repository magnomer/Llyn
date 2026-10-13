using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LAuthorPort
{
    IReadOnlyList<LCatalogAuthor> LEngineRollFind(LVista? vista);

    IReadOnlyList<LCatalogAuthor> LEngineUnionFind(LVista? roll, string typed);

    (string LUnionDropped, string LUnionKept) LEngineUnionRead(LTenure? held, long kept);

    LVita LEngineVitaRead(LVista? roll);

    void LEngineAuthorAbsorb(long kept, long dropped);

    IReadOnlyList<LCatalogReference> LEngineOeuvreFind(LVista? roll, LVista? oeuvre);

    string LEngineWorkFormat(int count);

    IReadOnlyList<LAuthorRow> LEngineCreditRead(LTenure? held);
}
