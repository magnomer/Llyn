using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CExampleDraft(
    CStateValue CExampleDraftText,
    long? CExampleDraftReference,
    IReadOnlyList<CGlossDraft> CExampleDraftGloss,
    IReadOnlyList<CMentionDraft> CExampleDraftMention);
