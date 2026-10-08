using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CReflexTyped(
    CReflexField CReflexTypedField, string CReflexTypedText, IReadOnlyList<CReflexHead> CReflexTypedHeads)
{
    public string CReflexTypedKey => CReflex.LReflexKeyRead(CReflexTypedText);
}
