using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LInflection(
    string LInflectionEntryId,
    int LInflectionPosition,
    string LInflectionText,
    string? LInflectionLocal,
    string? LInflectionSpeechId,
    IReadOnlyList<LFeature> LInflectionFeatures);
