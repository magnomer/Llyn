namespace Llyn.Core;

public sealed record LFeature(
    long LFeatureId,
    long LFeatureSpeechId,
    long LFeatureCode,
    string LFeatureName,
    int LFeaturePosition);
