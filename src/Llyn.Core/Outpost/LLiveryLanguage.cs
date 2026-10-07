using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LLiveryLanguage(
    string LLiveryLanguageName,
    IReadOnlyList<LCatalogPronunciation> LLiveryLanguagePronunciation,
    IReadOnlyList<LLiveryStem> LLiveryLanguageStem,
    IReadOnlyList<LLiveryDiwei> LLiveryLanguageDiwei);
