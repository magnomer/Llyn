using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CScriptGroup(
    string CScriptGroupHeading,
    string CScriptGroupStyle,
    string CScriptGroupGloss,
    IReadOnlyList<CScriptImage> CScriptGroupImages);
