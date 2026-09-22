using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LScriptStyle(
    string LScriptStyleName,
    string LScriptStyleUrl,
    IReadOnlyDictionary<string, string> LScriptStyleForm,
    string LScriptStylePattern,
    int LScriptStyleImage,
    int LScriptStyleCaption,
    string? LScriptStylePrefix = null,
    IReadOnlyList<LRespellingRule>? LScriptStyleRewrite = null,
    string? LScriptStyleGloss = null,
    IReadOnlyList<LEpoch>? LScriptStyleEpoch = null)
{
    public IReadOnlyList<LRespellingRule> LScriptStyleRewrite { get; init; } = LScriptStyleRewrite ?? [];

    public IReadOnlyList<LEpoch> LScriptStyleEpoch { get; init; } = LScriptStyleEpoch ?? [];
}
