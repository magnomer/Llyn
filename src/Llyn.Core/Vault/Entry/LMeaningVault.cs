using System.Collections.Generic;

namespace Llyn.Core;

public interface LMeaningVault
{
    LMeaning LMeaningCreate(LMeaning meaning);

    IReadOnlyList<LMeaning> LMeaningRead(long entryId);

    LMeaning? LMeaningSingleRead(long id);

    void LMeaningUpdate(LMeaning meaning);

    void LMeaningParentUpdate(long id, long? parentId);

    void LMeaningMove(long id, int position);

    void LMeaningDelete(long id);

    long? LMeaningHolderRead(long id);

    void LMeaningOrderSet(long entryId, long? parentId, IReadOnlyList<long> order);
}
