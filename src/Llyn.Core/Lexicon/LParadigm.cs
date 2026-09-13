using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LParadigm(
    long LParadigmSpeechCode,
    IReadOnlyList<long> LParadigmMorphology,
    IReadOnlyList<LParadigmRule>? LParadigmRegular = null,
    IReadOnlyList<long>? LParadigmExcept = null)
{
    public IReadOnlyList<LParadigmRule> LParadigmRegular { get; init; } = LParadigmRegular ?? [];

    public IReadOnlyList<long> LParadigmExcept { get; init; } = LParadigmExcept ?? [];
}
