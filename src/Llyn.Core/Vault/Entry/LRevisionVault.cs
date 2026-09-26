using System.Collections.Generic;

namespace Llyn.Core;

public interface LRevisionVault
{
    LRevision LRevisionRecord(IReadOnlyList<LRevisionDelta> changes);
}
