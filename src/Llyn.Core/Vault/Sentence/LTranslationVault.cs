using System.Collections.Generic;

namespace Llyn.Core;

public interface LTranslationVault
{
    IReadOnlyList<LTranslation> LTranslationMeaningRead(long meaningId);

    IReadOnlyList<LTranslation> LTranslationCollocationRead(long collocationId);

    void LTranslationMeaningSave(long meaningId, IReadOnlyList<LTranslation> translations);

    void LTranslationCollocationSave( long collocationId, IReadOnlyList<LTranslation> translations);

    IReadOnlyList<LTranslationTarget> LTranslationTargetRead(IReadOnlyList<long> ids);

    IReadOnlyList<LUsage> LTranslationIncomingRead(long entryId);
}
