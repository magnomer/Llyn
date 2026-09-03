using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LCardDraft(
    LStateValue LCardDraftTitle,
    LStateValue LCardDraftExpression,
    LStateValue LCardDraftMeaning,
    IReadOnlyList<LExampleDraft> LCardDraftExample,
    IReadOnlyList<LSituationDraft> LCardDraftSituation,
    IReadOnlyList<string> LCardDraftTranslation,
    string LCardDraftSynonym,
    IReadOnlyList<string> LCardDraftTag,
    IReadOnlyList<LStateValue> LCardDraftImage,
    string LCardDraftId = "")
{
    public LStateValue LCardDraftTitle { get; init; } =
        LCardDraftTitle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LCardDraftExpression { get; init; } =
        LCardDraftExpression ?? LStateValue.LStateValueUnspecified;

    public LStateValue LCardDraftMeaning { get; init; } =
        LCardDraftMeaning ?? LStateValue.LStateValueUnspecified;
}
