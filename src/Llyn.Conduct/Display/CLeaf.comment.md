# CLeaf.cs
Hash: `17e14298d5b0d117`

## `public sealed record CLeaf(long CLeafId, int CLeafPosition, CStateWording CLeafTitle, CStateWording CLeafExpression, CStateWording CLeafMeaning, IReadOnlyList<CLeafChip> CLeafSituation, IReadOnlyList<CLeafChip> CLeafRegister, IReadOnlyList<CLeafChip> CLeafTag, IReadOnlyList<CTranslationTarget> CLeafTranslation, IReadOnlyList<CLeafLine> CLeafSentence, IReadOnlyList<CImageDraft> CLeafImage, IReadOnlyList<CVideoDraft> CLeafVideo, bool CLeafFolded, bool CLeafStored)`

One meaning or collocation card of the reading view, ready to paint.
Every text arrives worded and every row arrives mapped, so the leaf fill only looks keys up and paints.

**Parameters**

- `CLeafId`: the card's id, handed back unread to the fold gate.
- `CLeafPosition`: the card's number, shown in its badge.
- `CLeafTitle`: the title, with the unknown mark's key while unknown and muted while never written.
  A muted title leaves the card's kind caption in its place.
- `CLeafExpression`: the collocation's expression, muted while never written, so its line folds.
- `CLeafMeaning`: the definition, muted while never written, so its line folds.
- `CLeafSituation`: the situation chips, in the order the card keeps them.
- `CLeafRegister`: the register chips, in the order the card keeps them.
- `CLeafTag`: the tag chips, in the order the card keeps them.
- `CLeafTranslation`: the entries the card links to, named, without a link the engine no longer finds.
- `CLeafSentence`: the example lines, each ready with its frame, sentence and Source line.
- `CLeafImage`: the picture rows, each carrying whether its location is empty.
- `CLeafVideo`: the video rows, each carrying whether its location is empty.
- `CLeafFolded`: whether the user folded the card, read from the store.
- `CLeafStored`: whether the card can fold, since only a stored card holds a fold.
  It is the card draft's own `LCardDraftStored` rule passed on, so the view never reads the id.

## `internal static IReadOnlyList<CLeaf> LLeafRead(IReadOnlyList<LCardDraft> cards, LSentenceOrder order, string mark, IReadOnlyDictionary<long, string> citations, IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> targets, IReadOnlySet<long> folds, LMediaPort media, LExamplePort examples)`

Maps the shown entry's cards of one list to their ready form.
The cards come in the order `CFolio.CFolioOrderRead` sets, the order the editor's list shows.
So the reading view, its contents and the editor number every card alike.
The order, the unknown mark, the Source lines and the link targets are read once for the entry.
Each card's links are its own entry of the target map, which the engine keys by every card.
A card missing from the map has no links, since a refused target read answers an empty map.
Shared wording, chip and media maps keep presentation rules consistent with the editor.
Each picture row carries the address `media` resolves, so the page loads it without asking.
Each sentence line is composed through `examples`.
The folded ids come from one entry read, independent of the lexical card draft.
