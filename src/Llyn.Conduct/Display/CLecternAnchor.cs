using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CLecternAnchor(
    bool CLecternAnchorOffered,
    IReadOnlyDictionary<long, string> CLecternAnchorTexts);
