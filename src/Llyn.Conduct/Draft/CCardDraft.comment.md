# CCardDraft.cs
Hash: `0dc23bd6c26822d4`

## `public sealed record CCardDraft(long CCardDraftId, int CCardDraftPosition, CStateWording CCardDraftTitle, CStateWording CCardDraftExpression, CStateWording CCardDraftMeaning, IReadOnlyList<CSentenceDraft> CCardDraftSentence, IReadOnlyList<CSituationDraft> CCardDraftSituation, IReadOnlyList<CRegisterDraft> CCardDraftRegister, IReadOnlyList<CTranslationTarget> CCardDraftTranslation, IReadOnlyList<CTagDraft> CCardDraftTag, IReadOnlyList<CImageDraft> CCardDraftImage, IReadOnlyList<CVideoDraft> CCardDraftVideo)`

One meaning or collocation of an entry, as the editor's card shows it.

**Parameters**

- `CCardDraftId`: the stored card, zero for a fresh one.
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
