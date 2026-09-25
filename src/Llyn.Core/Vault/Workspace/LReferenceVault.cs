using System.Collections.Generic;

namespace Llyn.Core;

public interface LReferenceVault
{
    LReference LReferenceCreate(LReference reference);

    LReference? LReferenceRead(long id);

    IReadOnlyList<LReference> LReferenceAllRead();

    void LReferenceUpdate(LReference reference);

    void LReferenceDelete(long id, bool detach);

    void LReferenceAuthorAttach(long referenceId, long authorId, int position);

    void LReferenceAuthorDetach(long referenceId, long authorId);

    IReadOnlyDictionary<long, int> LReferenceUsageRead();

    IReadOnlyList<LUsage> LReferenceUsageRead(long id);
}
