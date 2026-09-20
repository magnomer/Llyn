using System.Collections.Generic;

namespace Llyn.Core;

public interface LGlossVault
{
    IReadOnlyList<LGloss> LGlossExampleRead(long exampleId);

    IReadOnlyList<long> LGlossExampleSave(long exampleId, IReadOnlyList<LGloss> glosses);
}
