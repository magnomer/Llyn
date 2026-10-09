using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LParadigmTable(
    IReadOnlyList<string> LParadigmTableHeaders,
    IReadOnlyList<LParadigmLine> LParadigmTableLines);
