using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LLanguage(
    string LLanguageName,
    string? LLanguageFlag,
    LFont LLanguageFont,
    LFont LLanguageExample,
    IReadOnlyList<LSourceSpec> LLanguageLookupSources,
    IReadOnlyList<LSourceSpec> LLanguageHarvestSources);
