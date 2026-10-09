using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LInflectionSheet(
    IReadOnlyList<string> LInflectionSheetHeaders,
    IReadOnlyList<LInflectionLine> LInflectionSheetLines);
