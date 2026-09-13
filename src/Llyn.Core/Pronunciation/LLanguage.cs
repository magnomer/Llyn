using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LLanguage(
    string LLanguageName,
    string? LLanguageFlag,
    LFont LLanguageFont,
    LFont LLanguageExample,
    IReadOnlyList<LSourceSpec> LLanguageLookupSources,
    IReadOnlyList<LSourceSpec> LLanguageHarvestSources,
    IReadOnlyList<LScheme>? LLanguageSchemes = null,
    bool LLanguageSeparated = true,
    IReadOnlyList<LVariety>? LLanguageVarieties = null,
    bool LLanguageVarietyFlagged = true,
    LFont? LLanguageGloss = null,
    IReadOnlyList<LRespelling>? LLanguageCleanups = null,
    IReadOnlyList<LRespelling>? LLanguageRespellings = null,
    IReadOnlyList<LSourceSpec>? LLanguageFrequencies = null,
    IReadOnlyList<LBand>? LLanguageBands = null,
    IReadOnlyList<LSourceSpec>? LLanguageMorphologies = null)
{
    public IReadOnlyList<LScheme> LLanguageSchemes { get; init; } = LLanguageSchemes ?? [];

    public IReadOnlyList<LVariety> LLanguageVarieties { get; init; } = LLanguageVarieties ?? [];

    public IReadOnlyList<LRespelling> LLanguageCleanups { get; init; } = LLanguageCleanups ?? [];

    public IReadOnlyList<LRespelling> LLanguageRespellings { get; init; } = LLanguageRespellings ?? [];

    public IReadOnlyList<LSourceSpec> LLanguageFrequencies { get; init; } = LLanguageFrequencies ?? [];

    public IReadOnlyList<LBand> LLanguageBands { get; init; } = LLanguageBands ?? [];

    public IReadOnlyList<LSourceSpec> LLanguageMorphologies { get; init; } = LLanguageMorphologies ?? [];

    public LFont LLanguageGloss { get; init; } = LLanguageGloss ?? new LFont(null, 0);
}
