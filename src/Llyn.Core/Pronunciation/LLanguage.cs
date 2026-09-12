using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LLanguage(
    string LLanguageName,
    string? LLanguageFlag,
    LFont LLanguageFont,
    LFont LLanguageExample,
    IReadOnlyList<LSourceSpec> LLanguageLookupSources,
    IReadOnlyList<LSourceSpec> LLanguageHarvestSources,
    IReadOnlyList<string>? LLanguageSchemes = null,
    bool LLanguageSeparated = true)
{
    public IReadOnlyList<string> LLanguageSchemes { get; init; } = LLanguageSchemes ?? [];
}
