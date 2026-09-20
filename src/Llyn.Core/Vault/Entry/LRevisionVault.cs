using System.Collections.Generic;

namespace Llyn.Core;

public interface LRevisionVault
{
    LRevision LRevisionRecord(IReadOnlyList<LRevisionChange> changes);

    LRevision? LRevisionRead(long id);

    IReadOnlyList<LRevisionChange> LRevisionChangeRead(long revisionId);
}
