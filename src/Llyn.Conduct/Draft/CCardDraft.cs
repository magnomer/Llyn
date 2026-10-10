using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CCardDraft(
    long CCardDraftId,
    int CCardDraftPosition,
    CStateWording CCardDraftTitle,
    CStateWording CCardDraftExpression,
    CStateWording CCardDraftMeaning,
    IReadOnlyList<CSentenceDraft> CCardDraftSentence,
    IReadOnlyList<CSituationDraft> CCardDraftSituation,
    IReadOnlyList<CRegisterDraft> CCardDraftRegister,
    IReadOnlyList<CTranslationTarget> CCardDraftTranslation,
    IReadOnlyList<CTagDraft> CCardDraftTag,
    IReadOnlyList<CImageDraft> CCardDraftImage,
    IReadOnlyList<CVideoDraft> CCardDraftVideo,
    bool CCardDraftFolded,
    bool CCardDraftStored);
