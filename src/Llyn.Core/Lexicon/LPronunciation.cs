using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LPronunciation(
    string LPronunciationId,
    string LPronunciationEntryId,
    string? LPronunciationLevel,
    string? LPronunciationIpa,
    IReadOnlyList<LSyllable> LPronunciationSyllables,
    IReadOnlyList<LRepresentation> LPronunciationRepresentations);
