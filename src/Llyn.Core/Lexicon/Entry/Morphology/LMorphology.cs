namespace Llyn.Core;

public sealed record LMorphology(
    long LMorphologyId,
    long LMorphologyFeatureId,
    long LMorphologyCode,
    string LMorphologyName,
    int LMorphologyPosition);
