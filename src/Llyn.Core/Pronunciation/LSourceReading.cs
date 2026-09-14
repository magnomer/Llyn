namespace Llyn.Core;

public sealed record LSourceReading(
    string LSourceReadingVariety,
    string LSourceReadingStrategy,
    string? LSourceReadingPattern,
    int LSourceReadingGroup,
    string? LSourceReadingPath,
    bool LSourceReadingPhonetic,
    int LSourceReadingSkip,
    bool LSourceReadingEvery);
