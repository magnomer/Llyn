# CCardDraft.cs
Hash: `5cf4a9a29f742d4c`

## `public sealed record CCardDraft(long CCardDraftId, int CCardDraftPosition, CStateWording CCardDraftTitle, CStateWording CCardDraftExpression, CStateWording CCardDraftMeaning, IReadOnlyList<CSentenceDraft> CCardDraftSentence, IReadOnlyList<CSituationDraft> CCardDraftSituation, IReadOnlyList<CRegisterDraft> CCardDraftRegister, IReadOnlyList<CTranslationTarget> CCardDraftTranslation, IReadOnlyList<CTagDraft> CCardDraftTag, IReadOnlyList<CImageDraft> CCardDraftImage, IReadOnlyList<CVideoDraft> CCardDraftVideo, bool CCardDraftFolded, bool CCardDraftStored)`

One meaning or collocation of an entry, as the editor's card shows it.

**Parameters**

- `CCardDraftId`: the card address, positive for stored rows and nonpositive for unsaved cards.
- `CCardDraftPosition`: the card's place in its list.
- `CCardDraftTitle`: the card's title, worded, whose field shows no hint.
- `CCardDraftExpression`: the collocation's expression, worded with the expression hint.
- `CCardDraftMeaning`: the card's definition, worded with the hint its sheet names.
- `CCardDraftSentence`: the card's sentences.
- `CCardDraftSituation`: the card's situations.
- `CCardDraftRegister`: the card's registers.
- `CCardDraftTranslation`: the entries the card links to, ready in the card's order.
- `CCardDraftTag`: the card's tags.
- `CCardDraftImage`: the card's images.
- `CCardDraftVideo`: the card's videos.
- `CCardDraftFolded`: whether the user folded the card, read from the store and never from the draft.
- `CCardDraftStored`: whether the card can fold, since only a stored card holds a fold.
  It is the card draft's own `LCardDraftStored` rule passed on, so the view never reads the id.
