using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LParadigmLine(
    string LParadigmLineGroup,
    string LParadigmLineLabel,
    IReadOnlyList<LParadigmForm> LParadigmLineForms);
