using System.Collections.Generic;

namespace Llyn.Core;

public interface LInflectionVault
{
    IReadOnlyList<LInflection> LInflectionAppend(long entryId, IReadOnlyList<LInflection> inflections);

    IReadOnlyList<LInflection> LInflectionRead(long entryId);

    IReadOnlyList<LInflection> LInflectionSet(long entryId, IReadOnlyList<LInflection> inflections);

    void LInflectionRegularSave(long inflectionId, bool regular);
}
