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
    bool LLanguageSeparated = true,
    IReadOnlyList<LVariety>? LLanguageVarieties = null,
    bool LLanguageVarietyFlagged = true,
    LFont? LLanguageGloss = null)
{
    public IReadOnlyList<string> LLanguageSchemes { get; init; } = LLanguageSchemes ?? [];

    public IReadOnlyList<LVariety> LLanguageVarieties { get; init; } = LLanguageVarieties ?? [];

    public LFont LLanguageGloss { get; init; } = LLanguageGloss ?? new LFont(null, 0);
}
