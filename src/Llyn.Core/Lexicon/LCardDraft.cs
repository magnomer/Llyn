using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LCardDraft(
    string LCardDraftTitle,
    string LCardDraftExpression,
    string LCardDraftMeaning,
    IReadOnlyList<string> LCardDraftExample,
    IReadOnlyList<string> LCardDraftSituation,
    string LCardDraftSynonym,
    IReadOnlyList<string> LCardDraftTag,
    string LCardDraftId = "");
