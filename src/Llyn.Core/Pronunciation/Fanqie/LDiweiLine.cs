using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LDiweiLine(
    string LDiweiLineReading,
    string LDiweiLineLabel,
    bool LDiweiLineRounded,
    int LDiweiLineRank,
    IReadOnlyList<string> LDiweiLineCharacters);
