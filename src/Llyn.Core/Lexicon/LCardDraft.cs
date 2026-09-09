using System.Collections.Generic;

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
}
