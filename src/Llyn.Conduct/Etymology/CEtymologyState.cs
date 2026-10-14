using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CEtymologyState(
    IReadOnlyList<CAncestor> CEtymologyStateAncestors,
    IReadOnlyList<CAncestorChoice> CEtymologyStateRelations,
    IReadOnlyList<CAncestorChoice> CEtymologyStateLanguages,
    string CEtymologyStateHeadword = "");
