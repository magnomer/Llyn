using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LPronunciation(
    long LPronunciationId,
    long LPronunciationEntryId,
    string? LPronunciationLevel,
    string? LPronunciationIpa,
    IReadOnlyList<LSyllable> LPronunciationSyllables,
    IReadOnlyList<LRepresentation> LPronunciationRepresentations);
