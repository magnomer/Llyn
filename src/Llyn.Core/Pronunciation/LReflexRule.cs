using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LReflexRule(
    string LReflexRuleLanguage,
    string LReflexRuleUrl,
    IReadOnlyDictionary<string, string> LReflexRuleForm,
    string LReflexRulePattern,
    string? LReflexRuleTemplate = null,
    bool LReflexRuleEvery = false,
    string? LReflexRuleBusy = null,
    int LReflexRuleInterval = 0,
    IReadOnlyDictionary<string, string>? LReflexRuleHeaders = null,
    string? LReflexRuleEpithet = null,
    string? LReflexRuleClip = null,
    bool LReflexRuleFirst = false,
    IReadOnlyList<LRespellingRule>? LReflexRuleRewrite = null,
    string? LReflexRuleRegion = null,
    string? LReflexRuleSplit = null,
    string? LReflexRuleRemark = null,
    string? LReflexRuleUntil = null,
    bool LReflexRuleFolded = false,
    IReadOnlyList<LRespellingRule>? LReflexRuleRecast = null,
    bool LReflexRuleSuperscript = false)
{
    public const string LReflexRuleText = "text";

    public const string LReflexRuleKind = "kind";

    public const string LReflexRuleNote = "note";

    public const string LReflexRuleMain = "main";

    public const string LReflexRuleSense = "remark";

    public string LReflexRuleTemplate { get; init; } = LReflexRuleTemplate ?? "{" + LReflexRuleText + "}";

    public IReadOnlyDictionary<string, string> LReflexRuleHeaders { get; init; } =
        LReflexRuleHeaders ?? new Dictionary<string, string>();

    public IReadOnlyList<LRespellingRule> LReflexRuleRewrite { get; init; } = LReflexRuleRewrite ?? [];

    public IReadOnlyList<LRespellingRule> LReflexRuleRecast { get; init; } = LReflexRuleRecast ?? [];
}
