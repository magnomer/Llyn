using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LPressPrinter(
    string LPressPrinterName,
    bool LPressPrinterDefault,
    IReadOnlyList<LPressPaper> LPressPrinterPaper,
    IReadOnlyList<LPressSide> LPressPrinterSide,
    IReadOnlyList<LPressColor> LPressPrinterColor,
    bool LPressPrinterCollation,
    int? LPressPrinterCopies,
    bool LPressPrinterKnown);
