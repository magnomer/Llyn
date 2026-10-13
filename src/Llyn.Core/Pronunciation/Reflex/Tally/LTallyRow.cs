using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LTallyRow(
    string LTallyRowLanguage,
    string LTallyRowKind,
    IReadOnlyList<LTallyMark> LTallyRowMarks);
