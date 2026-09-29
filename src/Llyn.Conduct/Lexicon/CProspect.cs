using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CProspect(
    string CProspectText,
    string CProspectWord,
    IReadOnlyList<CVistaRow> CProspectRows,
    bool CProspectShown,
    bool CProspectChosen);
