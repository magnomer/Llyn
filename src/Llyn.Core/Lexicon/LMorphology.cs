namespace Llyn.Core;

public sealed record LMorphology(
    string LMorphologyLanguage,
    string LMorphologySpeechId,
    string LMorphologyFeatureId,
    string LMorphologyFeatureName,
    string LMorphologyValueId,
    string LMorphologyValueName,
    int LMorphologyPosition);
