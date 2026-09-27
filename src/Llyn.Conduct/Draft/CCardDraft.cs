using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CCardDraft(
    long CCardDraftId,
    int CCardDraftPosition,
    CStateValue CCardDraftTitle,
    CStateValue CCardDraftExpression,
    CStateValue CCardDraftMeaning,
    IReadOnlyList<CSentenceDraft> CCardDraftSentence,
    IReadOnlyList<CSituationDraft> CCardDraftSituation,
    IReadOnlyList<CRegisterDraft> CCardDraftRegister,
    IReadOnlyList<long> CCardDraftTranslation,
    IReadOnlyList<CTagDraft> CCardDraftTag,
    IReadOnlyList<CImageDraft> CCardDraftImage,
    IReadOnlyList<CVideoDraft> CCardDraftVideo,
    IReadOnlyList<CCardDraft> CCardDraftChild);
