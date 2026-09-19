using System.Collections.Generic;

namespace Llyn.Core;

public interface LMorphologyVault
{
    LFeature LFeatureCreate(LFeature feature);

    LMorphology LMorphologyCreate(LMorphology value);

    IReadOnlyList<LFeature> LFeatureRead(long speechValueId);

    LFeature? LFeatureFind(long speechValueId, string name);

    LMorphology? LMorphologyRead(long id);

    IReadOnlyList<LMorphology> LMorphologyScan(long featureId);

    LMorphology? LMorphologyFind(long featureId, string name);

    LMorphology? LMorphologyCodeFind(string language, long speechCode, long featureCode, long code);
}
