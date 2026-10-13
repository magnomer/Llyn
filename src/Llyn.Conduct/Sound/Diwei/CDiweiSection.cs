using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CDiweiSection(
    string CDiweiSectionLabel,
    IReadOnlyList<CDiweiLine> CDiweiSectionLines,
    IReadOnlyList<CTally> CDiweiSectionTallies,
    bool CDiweiSectionSwitched,
    bool CDiweiSectionRespelled);
