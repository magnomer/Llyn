using System.Collections.Generic;

namespace Llyn.Core;

public interface LLacunaVault
{
    IReadOnlyList<LLacuna> LLacunaRead(long entryId);

    void LLacunaSave(long entryId, IReadOnlyList<LLacuna> lacunae);

    void LLacunaDelete(long entryId);
}
