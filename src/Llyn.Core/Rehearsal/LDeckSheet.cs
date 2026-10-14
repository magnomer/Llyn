using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LDeckSheet(
    IReadOnlyList<(string LDeckSheetKey, string LDeckSheetValue)> LDeckSheetSettings,
    IReadOnlyList<string> LDeckSheetHeader,
    IReadOnlyList<IReadOnlyList<string>> LDeckSheetRows);
