using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CProffer(
    string CProfferText,
    IReadOnlyList<CProfferRow> CProfferRows,
    bool CProfferShown);
