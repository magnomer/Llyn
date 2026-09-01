using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LExample(
    string LExampleId,
    string LExampleLanguage,
    string LExampleText,
    string? LExampleLocal,
    string? LExampleSourceId,
    IReadOnlyList<LTranslation> LExampleTranslations);
