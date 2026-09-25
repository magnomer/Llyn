using System.Collections.Generic;

namespace Llyn.Core;

public interface LLacunaVault
{
    IReadOnlyList<LLacuna> LLacunaRead(long entryId);

    void LLacunaSave(long entryId, IReadOnlyList<long?> morphologyIds);

    void LLacunaDelete(long entryId);
}
