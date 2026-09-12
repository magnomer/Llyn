using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LCardDraft(
    LStateValue LCardDraftTitle,
    LStateValue LCardDraftExpression,
    LStateValue LCardDraftMeaning,
    IReadOnlyList<LSentenceDraft> LCardDraftSentence,
    IReadOnlyList<LSituationDraft> LCardDraftSituation,
    IReadOnlyList<LRegisterDraft> LCardDraftRegister,
    IReadOnlyList<long> LCardDraftTranslation,
    IReadOnlyList<LTagDraft> LCardDraftTag,
    IReadOnlyList<LImageDraft> LCardDraftImage,
    IReadOnlyList<LVideoDraft> LCardDraftVideo,
    int LCardDraftPosition,
    long LCardDraftId = 0,
    IReadOnlyList<LCardDraft>? LCardDraftChild = null,
    string? LCardDraftGloss = null,
    string? LCardDraftLanguage = null,
    string LCardDraftLabels = "")
{
    public string LCardDraftLabels { get; init; } = LCardDraftLabels ?? string.Empty;

    public IReadOnlyList<LCardDraft> LCardDraftChild { get; init; } = LCardDraftChild ?? [];

    public LStateValue LCardDraftTitle { get; init; } =
        LCardDraftTitle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LCardDraftExpression { get; init; } =
        LCardDraftExpression ?? LStateValue.LStateValueUnspecified;

    public LStateValue LCardDraftMeaning { get; init; } =
        LCardDraftMeaning ?? LStateValue.LStateValueUnspecified;

    public bool LCardDraftEmpty =>
        LCardDraftTitle.LStateValueEmpty
        && LCardDraftExpression.LStateValueEmpty
        && LCardDraftMeaning.LStateValueEmpty
        && string.IsNullOrWhiteSpace(LCardDraftGloss)
        && string.IsNullOrWhiteSpace(LCardDraftLabels)
        && LCardDraftChild.Count == 0
        && LCardDraftSentence.All(static row => row.LSentenceDraftEmpty)
        && LCardDraftSituation.All(static row => row.LSituationDraftTitle.LStateValueEmpty)
        && LCardDraftRegister.All(static row => row.LRegisterDraftName.LStateValueEmpty)
        && LCardDraftTranslation.All(static id => id == 0)
        && LCardDraftTag.All(static row => string.IsNullOrWhiteSpace(row.LTagDraftText))
        && LCardDraftImage.All(static row => row.LImageDraftEmpty)
        && LCardDraftVideo.All(static row => row.LVideoDraftEmpty);
}
