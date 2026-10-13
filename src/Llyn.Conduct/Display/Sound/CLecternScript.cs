using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CLecternScript(
    IReadOnlyList<CScriptGroup> CLecternScriptGroups,
    bool CLecternScriptPending,
    CFont CLecternScriptFont);
