using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LPronunciation(
    long LPronunciationId,
    long LPronunciationEntryId,
    int LPronunciationPosition,
    string? LPronunciationVariety,
    string? LPronunciationIpa,
    IReadOnlyList<LSyllable> LPronunciationSyllables);
