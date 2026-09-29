using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CExample(
    string CExampleLanguage,
    CStateValue CExampleText,
    long? CExampleSource,
    string CExampleCitation,
    IReadOnlyList<CGlossDraft> CExampleGloss,
    IReadOnlyList<CMentionDraft> CExampleMention,
    IReadOnlyList<CMentionMark> CExampleExcerpt);
