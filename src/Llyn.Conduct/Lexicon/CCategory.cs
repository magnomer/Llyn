using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CCategory(
    IReadOnlyList<CCategoryRow> CCategoryRows, bool CCategoryDeclared, bool CCategoryMatched, bool CCategoryShown)
{
    public string? CCategoryHint =>
        !CCategoryDeclared ? "Speech.Empty" : CCategoryMatched ? null : "Speech.Absent";
}
