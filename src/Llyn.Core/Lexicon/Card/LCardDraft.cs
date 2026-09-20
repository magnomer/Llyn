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
    IReadOnlyList<LCardDraft>? LCardDraftChild = null)
{
    public IReadOnlyList<LCardDraft> LCardDraftChild { get; init; } = LCardDraftChild ?? [];

    public LStateValue LCardDraftTitle { get; init; } =
        LCardDraftTitle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LCardDraftExpression { get; init; } =
        LCardDraftExpression ?? LStateValue.LStateValueUnspecified;

    public LStateValue LCardDraftMeaning { get; init; } =
        LCardDraftMeaning ?? LStateValue.LStateValueUnspecified;

    public LCardDraft LCardDraftNormalize()
    {
        return this with
        {
            LCardDraftTitle = LCardDraftTitle.LStateValueNormalize(),
            LCardDraftExpression = LCardDraftExpression.LStateValueNormalize(),
            LCardDraftMeaning = LCardDraftMeaning.LStateValueNormalize(),
            LCardDraftSentence = LCardDraftSentence.Select(static row => row.LSentenceDraftNormalize()).ToList(),
            LCardDraftSituation = LCardDraftSituation.Select(static row => row.LSituationDraftNormalize()).ToList(),
            LCardDraftRegister = LCardDraftRegister.Select(static row => row.LRegisterDraftNormalize()).ToList(),
            LCardDraftImage = LCardDraftImage.Select(static row => row.LImageDraftNormalize()).ToList(),
            LCardDraftVideo = LCardDraftVideo.Select(static row => row.LVideoDraftNormalize()).ToList(),
            LCardDraftChild = LCardDraftChild.Select(static row => row.LCardDraftNormalize()).ToList(),
        };
    }

    public bool LCardDraftExemplified => LCardDraftSentence.Count > 0;

    public int LCardDraftTally => 1 + LCardDraftChild.Sum(static row => row.LCardDraftTally);

    public bool LCardDraftEmpty =>
        LCardDraftTitle.LStateValueEmpty
        && LCardDraftExpression.LStateValueEmpty
        && LCardDraftMeaning.LStateValueEmpty
        && LCardDraftChild.Count == 0
        && LCardDraftSentence.All(static row => row.LSentenceDraftEmpty)
        && LCardDraftSituation.All(static row => row.LSituationDraftTitle.LStateValueEmpty)
        && LCardDraftRegister.All(static row => row.LRegisterDraftName.LStateValueEmpty)
        && LCardDraftTranslation.All(static id => id == 0)
        && LCardDraftTag.All(static row => string.IsNullOrWhiteSpace(row.LTagDraftText))
        && LCardDraftImage.All(static row => row.LImageDraftEmpty)
        && LCardDraftVideo.All(static row => row.LVideoDraftEmpty);
}
