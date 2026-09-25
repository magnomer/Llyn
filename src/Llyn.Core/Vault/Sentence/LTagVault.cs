using System.Collections.Generic;

namespace Llyn.Core;

public interface LTagVault
{
    IReadOnlyList<LTag> LTagMeaningRead(long meaningId);

    IReadOnlyList<LTag> LTagCollocationRead(long collocationId);

    IReadOnlyList<long> LTagMeaningSave(long meaningId, IReadOnlyList<LTag> tags);

    IReadOnlyList<long> LTagCollocationSave(long collocationId, IReadOnlyList<LTag> tags);

    LTag? LTagRead(long id);

    long LTagResolve(string text);

    IReadOnlyList<LTag> LTagCatalogRead();
}
