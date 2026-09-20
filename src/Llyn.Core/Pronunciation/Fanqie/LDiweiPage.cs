using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LDiweiPage(
    string LDiweiPageLanguage,
    string LDiweiPageKey,
    bool LDiweiPageFinal,
    IReadOnlyList<LDiweiSection> LDiweiPageSections)
{
    public static readonly LDiweiPage LDiweiPageBlank = new(string.Empty, string.Empty, false, []);

    public bool LDiweiPageEmpty => LDiweiPageSections.Count == 0;
}
