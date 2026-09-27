using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CExample(
    string CExampleLanguage,
    CStateValue CExampleText,
    long? CExampleSource,
    IReadOnlyList<CGlossDraft> CExampleGloss,
    IReadOnlyList<CMentionDraft> CExampleMention,
    IReadOnlyList<CMention> CExampleExcerpt);
