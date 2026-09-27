using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CDiweiLine(
    string CDiweiLineReading,
    string CDiweiLineLabel,
    bool CDiweiLineRounded,
    IReadOnlyList<string> CDiweiLineCharacters);
