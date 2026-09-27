using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CDiweiPage(
    string CDiweiPageLanguage,
    string CDiweiPageKey,
    IReadOnlyList<CDiweiSection> CDiweiPageSections,
    bool CDiweiPageEmpty);
