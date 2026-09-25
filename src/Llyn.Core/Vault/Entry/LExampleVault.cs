using System.Collections.Generic;

namespace Llyn.Core;

public interface LExampleVault
{
    LExample LExampleCreate(LExample example);

    LExample? LExampleRead(long id);

    IReadOnlyList<LExample> LExampleRead();

    void LExampleUpdate(LExample example);

    void LExampleTextUpdate(long exampleId, LStateValue text);

    void LExampleSourceUpdate(long exampleId, LStateAnchor source);

    int LExampleReferenceRead(long id);

    IReadOnlyDictionary<long, int> LExampleReferenceRead();

    void LExampleDelete(long id, bool detach);

    IReadOnlyList<LUsage> LExampleUsageRead(long id);
}
