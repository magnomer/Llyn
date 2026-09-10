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
    IReadOnlyList<string> LCardDraftTranslation,
    string LCardDraftSynonym,
    IReadOnlyList<string> LCardDraftTag,
    IReadOnlyList<LImageDraft> LCardDraftImage,
    IReadOnlyList<LVideoDraft> LCardDraftVideo,
    int LCardDraftPosition,
    string LCardDraftId = "",
    IReadOnlyList<LCardDraft>? LCardDraftChild = null,
    string? LCardDraftGloss = null,
    string? LCardDraftLanguage = null,
    string LCardDraftLabels = "",
    IReadOnlyList<LRelationDraft>? LCardDraftRelation = null,
    IReadOnlyList<LSynonymDraft>? LCardDraftInterlink = null,
    string LCardDraftKey = "")
{
    public string LCardDraftLabels { get; init; } = LCardDraftLabels ?? string.Empty;

    public string LCardDraftKey { get; init; } = LCardDraftKey ?? string.Empty;

    public IReadOnlyList<LCardDraft> LCardDraftChild { get; init; } = LCardDraftChild ?? [];

    public IReadOnlyList<LRelationDraft> LCardDraftRelation { get; init; } =
        LCardDraftRelation ?? [];

    public IReadOnlyList<LSynonymDraft> LCardDraftInterlink { get; init; } =
        LCardDraftInterlink ?? [];

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
        && string.IsNullOrWhiteSpace(LCardDraftSynonym)
        && string.IsNullOrWhiteSpace(LCardDraftGloss)
        && string.IsNullOrWhiteSpace(LCardDraftLabels)
        && LCardDraftChild.Count == 0
        && LCardDraftSentence.All(static row => row.LSentenceDraftEmpty)
        && LCardDraftSituation.All(static row => row.LSituationDraftTitle.LStateValueEmpty)
        && LCardDraftRegister.All(static row => row.LRegisterDraftName.LStateValueEmpty)
        && LCardDraftTranslation.All(static text => string.IsNullOrWhiteSpace(text))
        && LCardDraftTag.All(static text => string.IsNullOrWhiteSpace(text))
        && LCardDraftImage.All(static row => row.LImageDraftEmpty)
        && LCardDraftVideo.All(static row => row.LVideoDraftEmpty)
        && LCardDraftRelation.All(static row => row.LRelationDraftEmpty)
        && LCardDraftInterlink.All(static row => row.LSynonymDraftEmpty);
}
