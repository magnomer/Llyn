using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CTally(string CTallyLanguage, string CTallyKind, IReadOnlyList<CTallyMark> CTallyMarks);
