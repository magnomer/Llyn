using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LLanguage(
    string LLanguageName,
    string? LLanguageFlag,
    IReadOnlyList<LSourceSpec> LLanguageSources);
