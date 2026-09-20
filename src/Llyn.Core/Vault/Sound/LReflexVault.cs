using System.Collections.Generic;

namespace Llyn.Core;

public interface LReflexVault
{
    IReadOnlyList<LReflex> LReflexRead(long entryId);

    IReadOnlyDictionary<long, IReadOnlyList<LReflex>> LReflexAnchorScan(long diweiId);

    IReadOnlyList<LReflex> LReflexSet(long entryId, IReadOnlyList<LReflex> reflexes);
}
