using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CEnsignSheet<CEnsignSheetKind>(
    IReadOnlyList<string> CEnsignSheetLanguages,
    CEnsignSheetKind CEnsignSheetRows);
