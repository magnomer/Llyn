# CLeaf.cs
Hash: `00a7c9c3e7c4b2b8`

## `public sealed record CLeaf(long CLeafId, int CLeafPosition, CStateWording CLeafTitle, CStateWording CLeafMeaning, IReadOnlyList<CLeafChip> CLeafSituation, IReadOnlyList<CLeafChip> CLeafRegister, IReadOnlyList<CLeafChip> CLeafTag, IReadOnlyList<CTranslationTarget> CLeafTranslation, IReadOnlyList<CLeafLine> CLeafSentence, IReadOnlyList<CImageDraft> CLeafImage, IReadOnlyList<CVideoDraft> CLeafVideo, bool CLeafFolded, bool CLeafStored, IReadOnlyList<CLeaf> CLeafCollocation, IReadOnlyList<(long CLeafSeeId, string CLeafSeeText)> CLeafSee, CStateWording CLeafPhrase, IReadOnlyList<string> CLeafFlag, string CLeafExample, string CLeafAlso, string CLeafHeading)`

One meaning or collocation card of the reading view, ready to paint.
Every text arrives worded and every row arrives mapped, so the leaf fill only looks keys up and paints.

**Parameters**

- `CLeafId`: the card's id, handed back unread to the fold gate.
- `CLeafPosition`: the card's number, shown in its badge.
- `CLeafTitle`: the title, with the unknown mark's key while unknown and muted while never written.
  A muted title leaves the card's kind caption in its place.
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
- `CLeafCollocation`: the collocations whose home is this meaning or one below it, shown as folding rows under it.
  A collocation's home is its first link to a meaning the draft still holds.
  It is empty for a collocation leaf and for a meaning without collocations.
- `CLeafSee`: one "see" line for each collocation linking here or below but homed under another meaning.
  `CLeafSeeId` is the collocation's id, handed back unread to the fold gate and the contents.
  So a click on the line opens the row and scrolls to it.
  `CLeafSeeText` is the ready line, naming the phrase, or the unknown mark, and the path of the home meaning.
- `CLeafPhrase`: the phrase a collocation row is headed with, its expression or else its title.
  It is unspecified for a meaning leaf, so it reads blank and muted there.
- `CLeafFlag`: the translation languages a closed collocation row shows one flag for.
- `CLeafExample`: a closed row's ready "N examples" marker, empty when no example is filled.
- `CLeafAlso`: a closed row's ready "also N" marker, empty when the collocation links to no further meaning.
  The phrase and the three markers are empty on a meaning leaf, since only collocations have them.
- `CLeafHeading`: the ready "Collocations · N" caption over a meaning's rows, empty when nothing homes there.

## `public bool CLeafQuoted`

Whether the row has an example marker.
A closed row shows its "N examples" marker on this verdict, so the view never measures the text.

## `public bool CLeafShared`

Whether the row has an "also N" marker, that is, whether the collocation links to a further meaning.
A closed row shows the marker on this verdict, so the view never measures the text.

## `internal static IReadOnlyList<CLeaf> LLeafRead(IReadOnlyList<LCardDraft> cards, LCollocationLayout? layout, LSentenceOrder order, string mark, string stub, string example, string exampleOne, string also, string heading, IReadOnlyDictionary<long, string> citations, IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> targets, IReadOnlySet<long> folds, LMediaPort media, LExamplePort examples)`

Maps the shown entry's cards of one list to their ready form.
`layout` is the engine's answer for where each collocation sits, so the map applies no link rule itself.
It only looks up each card's homed collocations, stub lines, phrase and markers by the card's id.
The nested collocations read through the same map with the same layout.
So each row finds its own phrase and markers by its id.
Nesting stops there, since nothing homes under a collocation.
The other collocations read with the layout too, so their rows carry the same markers.
`stub` is the format of a stub line, filled with the phrase and the home path.
`example` and `exampleOne` word the example count, plural and singular, and a count of zero gives no marker.
`also` words the further paths, joined with commas, and no paths give no marker.
`heading` words the count of homed collocations, and a meaning with none gives no caption.
The cards come in the order `CFolio.CFolioOrderRead` sets, the order the editor's list shows.
So the reading view, its contents and the editor number every card alike.
The order, the unknown mark, the Source lines and the link targets are read once for the entry.
Each card's links are its own entry of the target map, which the engine keys by every card.
A card missing from the map has no links, since a refused target read answers an empty map.
Shared wording, chip and media maps keep presentation rules consistent with the editor.
Each picture row carries the address `media` resolves, so the page loads it without asking.
Each sentence line is composed through `examples`.
The folded ids come from one entry read, independent of the lexical card draft.
