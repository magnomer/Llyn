using System.Collections.Generic;

namespace Llyn.Core;

public interface LCollocationVault
{
    LCollocation LCollocationCreate(LCollocation collocation);

    IReadOnlyList<LCollocation> LCollocationRead(long entryId);

    void LCollocationUpdate(LCollocation collocation);

    void LCollocationDelete(long id);

    void LCollocationOrderSet(long entryId, IReadOnlyList<long> order);
}
