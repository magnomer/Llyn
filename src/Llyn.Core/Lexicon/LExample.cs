using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LExample(
    string LExampleId,
    string LExampleLanguage,
    LStateValue LExampleText,
    string? LExampleLocal,
    LStateValue LExampleSource,
    IReadOnlyList<LTranslation> LExampleTranslations)
{
    public LStateValue LExampleText { get; init; } = LExampleText ?? LStateValue.LStateValueUnspecified;

    public LStateValue LExampleSource { get; init; } = LExampleSource ?? LStateValue.LStateValueUnspecified;
}

