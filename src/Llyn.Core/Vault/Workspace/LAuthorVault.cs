using System.Collections.Generic;

namespace Llyn.Core;

public interface LAuthorVault
{
    LAuthor LAuthorCreate(LAuthor author);

    LAuthor? LAuthorRead(long id);

    IReadOnlyList<LAuthor> LAuthorReferenceRead(long referenceId);

    IReadOnlyList<LAuthor> LAuthorAllRead();

    IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> LAuthorReferenceRead();

    void LAuthorUpdate(LAuthor author);

    void LAuthorAbsorb(long kept, long dropped);

    void LAuthorDelete(long id, bool detach);

    IReadOnlyList<LUsage> LAuthorUsageRead(long id);
}
