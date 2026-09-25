using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LLanguage(
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
    IReadOnlyList<LSourceSpec>? LLanguageMorphologies = null,
    bool LLanguageTonal = false,
    LGlyph? LLanguageGlyph = null,
    IReadOnlyList<LScriptStyle>? LLanguageScripts = null,
    IReadOnlyList<LFanqieBook>? LLanguageFanqieBooks = null,
    LHypothesis? LLanguageHypothesis = null,
    bool LLanguageSilent = false,
    IReadOnlyList<LReflexRule>? LLanguageReflexRules = null,
    bool LLanguagePhonemic = false,
    IReadOnlyList<LAnatomyRule>? LLanguageAnatomies = null,
    IReadOnlyList<LAnatomyTone>? LLanguageAnatomyTones = null,
    LShengfuRule? LLanguageShengfu = null)
{
    public IReadOnlyList<LScheme> LLanguageSchemes { get; init; } = LLanguageSchemes ?? [];

    public IReadOnlyList<LVariety> LLanguageVarieties { get; init; } = LLanguageVarieties ?? [];

    public IReadOnlyList<LRespelling> LLanguageCleanups { get; init; } = LLanguageCleanups ?? [];

    public IReadOnlyList<LRespelling> LLanguageRespellings { get; init; } = LLanguageRespellings ?? [];

    public IReadOnlyList<LSourceSpec> LLanguageFrequencies { get; init; } = LLanguageFrequencies ?? [];

    public IReadOnlyList<LSourceSpec> LLanguageMorphologies { get; init; } = LLanguageMorphologies ?? [];

    public LFont LLanguageGloss { get; init; } = LLanguageGloss ?? new LFont(null, 0);

    public IReadOnlyList<LScriptStyle> LLanguageScripts { get; init; } = LLanguageScripts ?? [];

    public IReadOnlyList<LFanqieBook> LLanguageFanqieBooks { get; init; } = LLanguageFanqieBooks ?? [];

    public IReadOnlyList<LReflexRule> LLanguageReflexRules { get; init; } = LLanguageReflexRules ?? [];

    public IReadOnlyList<LAnatomyRule> LLanguageAnatomies { get; init; } = LLanguageAnatomies ?? [];

    public IReadOnlyList<LAnatomyTone> LLanguageAnatomyTones { get; init; } = LLanguageAnatomyTones ?? [];
}
