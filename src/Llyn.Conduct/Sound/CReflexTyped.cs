using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CReflexTyped(
    CReflexField CReflexTypedField, string CReflexTypedText, IReadOnlyList<CReflexHead> CReflexTypedHeads);
