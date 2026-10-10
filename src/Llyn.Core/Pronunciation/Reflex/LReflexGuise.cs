using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LReflexGuise(bool LReflexGuiseRespelled, bool LReflexGuisePhonemic, bool LReflexGuiseFolded)
{
    public static LReflexGuise? LReflexGuiseFind(IReadOnlyList<LReflexGuise> guises, int index)
    {
        ArgumentNullException.ThrowIfNull(guises);

        return index >= 0 && index < guises.Count ? guises[index] : null;
    }
}
