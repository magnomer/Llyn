using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LShengfuRule(
    string LShengfuRuleUrl,
    string LShengfuRulePattern,
    IReadOnlyDictionary<string, string> LShengfuRuleForm,
    string LShengfuRuleSource = "",
    string? LShengfuRuleBusy = null,
    int LShengfuRuleInterval = 0,
    string LShengfuRuleSeparator = "·")
{
    public const string LShengfuRuleGroup = "shengfu";

    public string LShengfuRuleSource { get; init; } = LShengfuRuleSource ?? string.Empty;

    public string LShengfuRuleSeparator { get; init; } = LShengfuRuleSeparator ?? string.Empty;
}
