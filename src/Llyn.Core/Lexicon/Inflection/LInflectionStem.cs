using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LInflectionStem(
    IReadOnlyList<long> LInflectionStemValues,
    IReadOnlyDictionary<string, string> LInflectionStemTemplates,
    IReadOnlyList<LInflectionEnding> LInflectionStemEndings);
