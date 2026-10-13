using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CSoundingScript(
    IReadOnlyList<CScriptGroup> CSoundingScriptGroups,
    bool CSoundingScriptPending,
    bool CSoundingScriptRebuildable,
    CFont CSoundingScriptFont);
