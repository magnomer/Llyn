using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LInflection(
    long LInflectionId,
    long LInflectionEntryId,
    int LInflectionPosition,
    string LInflectionText,
    string? LInflectionLocal,
    long? LInflectionSpeechId,
    IReadOnlyList<long> LInflectionMorphology,
    bool LInflectionRegular = false);
