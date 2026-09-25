using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LDiweiPage(
    string LDiweiPageLanguage,
    string LDiweiPageKey,
    IReadOnlyList<LDiweiSection> LDiweiPageSections)
{
    public static readonly LDiweiPage LDiweiPageBlank = new(string.Empty, string.Empty, []);

    public bool LDiweiPageEmpty => LDiweiPageSections.Count == 0;
}
