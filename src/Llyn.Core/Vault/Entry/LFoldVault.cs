using System.Collections.Generic;

namespace Llyn.Core;

public interface LFoldVault
{
    void LFoldSave(long cardId);

    void LFoldDelete(long cardId);

    IReadOnlySet<long> LFoldRead(long entryId);

    void LFoldReflexSpread(long entryId, bool opened);

    bool LFoldReflexCheck(long entryId);

    void LFoldBoxSpread(long entryId, LFoldBox box, bool opened);

    bool LFoldBoxCheck(long entryId, LFoldBox box);

    void LFoldStemSpread(long entryId, string key, bool opened);

    bool LFoldStemCheck(long entryId, string key);
}
