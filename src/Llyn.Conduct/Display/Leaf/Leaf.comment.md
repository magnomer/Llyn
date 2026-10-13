# Leaf

This folder holds the reading view's card area with the cards, their chips and example lines.
It also holds the etymology the area reads beside them.

## `CDisplayCard`

The card area reads the cards, the usages pointing at the entry and the etymology.
It also folds or unfolds one card of the chosen entry.
It holds no state.
It takes the rules whole, since the shown draft and the chosen entry change after it is built.
Every read here runs on a repaint and shows its failure through repaint memory, so a lasting fault shows once.

### `CDisplayCardRead`

No shown draft answers no cards, with no section shown.
Each linked collocation is a row in the leaf of the top-level meaning that holds its home.
The collocations with no live link fill the Other list.
Where each collocation sits, and each row's phrase and markers, come from one engine read.
So Conduct applies no link rule itself.
The link targets already read are handed to that read, so each row's flags need no second read.
That read reaches no store, so it needs no fallback.
The stub, marker and caption wordings under `Display.Collocation` are read once and carried to every leaf.
The order, the Source lines, the link targets and the folds are four reads, each answering a fallback when refused.
Each refusal also shows its own notice, so the page still draws.
The unknown mark is the engine's word, so a frame or a sentence embeds it ready.
The folds are one read for the whole entry, never one read per card.
The order read falls back to the default order, and the other three reads fall back to empty results.
The failure keys are `Display.OrderFailed`, `Display.CitationFailed`, `Display.TranslationFailed` and `Fold.ReadFailed`.
The Source lines are read on every card read, so a Source edited elsewhere reads fresh.

`CDisplayFoldToggle` makes one save or delete call and answers whether the port returned without throwing.
A nonpositive card id still yields true, without confirming a stored row change.
An unsaved card writes nothing, since the clerk skips its nonpositive id.
The card id is the leaf's own `CLeafId`, handed back unread.
No entry chosen writes nothing and answers false.
A refused write shows `Fold.SaveFailed` every time, since the user acted, and answers false.
The caller puts its chevron back on false, since no bulletin will repaint it.
The port raises the fold bulletin.
So this view and every other view on the entry repaint from the store alike.
Folding is no edit, so it never dirties a draft and never enters undo.
`LDisplayFoldRead` answers the ids of every folded card, Meanings and Collocations together.
No entry chosen answers an empty set, which keeps the cards drawable.
`CDisplayIncomingRead` has the engine name each usage and its epithet in one read.
Each usage maps through `COeuvre.COeuvreUsageRead`, the one map the vita's citations share.
A refused read shows `Display.IncomingFailed` once until the user acts, and answers no usages.
`CDisplayEtymologyRead` has the engine answer one record naming the links and saying whether a narrative or a link stands.
Whether the narrative holds words is the etymology draft's rule, so no trim happens here.
A refused read shows `Display.EtymologyFailed` once until the user acts.
It answers no links, and the field then shows by its narrative alone.

## `CLecternCard`

The cards of the reading view, ready to paint, with whether their sections show.
`CLecternCardOther` holds the collocations linked to no meaning the draft still holds.
`CLecternCardCollocated` is whether that list holds any card, which shows the Other collocations section.
Linked collocations show inside the leaf of the top-level meaning above their home instead.
`CLecternCardDefined` is whether the entry has a Meaning, which shows the Meaning section.

## `CLecternEtymology`

The etymology of the shown entry, ready to show.
The targets are the source links whose entries still stand, named and in order.
`CLecternEtymologyShown` is whether the field has a narrative or a link to show.
`CLecternEtymologyNarrated` is whether the narrative holds words, which shows its read face.
`CLecternEtymologyLinked` is whether a source link still stands, which shows the row of links.
`CLecternEtymologyDerived` is whether the entry has an etymology at all, which shows its section.

## `CLeaf`

One meaning or collocation card of the reading view, ready to paint.
Every text arrives worded and every row arrives mapped, so the leaf fill only looks keys up and paints.
`CLeafId` is handed back unread to the fold gate.
`CLeafTitle` carries the unknown mark's key while unknown and is muted while never written.
A muted title leaves the card's kind caption in its place.
`CLeafMeaning` is muted while never written, so its line folds.
The situation, register and tag chips come in the order the card keeps them.
`CLeafTranslation` names the entries the card links to, without a link the engine no longer finds.
Each sentence line is ready with its frame, sentence and Source line.
Each picture and video row carries whether its location is empty.
`CLeafFolded` is read from the store.
`CLeafStored` is the card draft's own `LCardDraftStored` rule passed on, so the view never reads the id.
Only a stored card can fold.
`CLeafCollocation` holds the collocations whose home is this meaning or one below it, shown as folding rows.
A collocation's home is its first link to a meaning the draft still holds.
It is empty for a collocation leaf and for a meaning without collocations.
`CLeafSee` holds one "see" line for each collocation linking here or below but homed under another meaning.
`CLeafSeeId` is handed back unread to the fold gate and the contents.
So a click opens the row and scrolls to it.
`CLeafSeeText` names the phrase, or the unknown mark, and the path of the home meaning.
`CLeafPhrase` heads a collocation row with its expression or else its title.
It reads blank and muted on a meaning leaf.
`CLeafFlag` lists the translation languages a closed collocation row shows one flag for.
`CLeafExample` and `CLeafAlso` are a closed row's ready markers, empty when there is nothing to count.
The phrase and the three markers are empty on a meaning leaf, since only collocations have them.
`CLeafHeading` is the ready "Collocations" caption over a meaning's rows, empty when nothing homes there.
`CLeafQuoted` and `CLeafShared` are the verdicts a closed row shows its markers on, so the view never measures the text.

### `LLeafRead`

It maps the cards of one list to their ready form.
The layout is the engine's answer for where each collocation sits, so the map applies no link rule itself.
It only looks up each card's homed collocations, stub lines, phrase and markers by the card's id.
The nested collocations and the Other collocations read through the same map with the same layout.
Nesting stops there, since nothing homes under a collocation.
The stub is the format of a stub line, filled with the phrase and the home path.
The example wordings give no marker for a count of zero.
The also wording joins the further paths with commas, and no paths give no marker.
The heading words the count of homed collocations, and a meaning with none gives no caption.
The cards come in the order `CFolio.CFolioOrderRead` sets, the order the editor's list shows.
So the reading view, its contents and the editor number every card alike.
The order, the unknown mark, the Source lines and the link targets are read once for the entry.
Each card's links are its own entry of the target map, which the engine keys by every card.
A card missing from the map has no links, since a refused target read answers an empty map.
Each picture row carries the address `media` resolves, so the page loads it without asking.
Each sentence line is composed through `examples`.
The folded ids come from one entry read, independent of the lexical card draft.

## `CLeafChip`

One situation, register or tag chip of a reading card, ready to paint and to open.
The wording carries the unknown mark's key while unknown.
The subject picks the tab a click opens.
`CLeafChipStored` is false for a record never saved, which opens nothing.
`LLeafChipRead` words a situation from its title and a register from its name.
A tag is worded from its text, which has no unknown state.

## `CLeafLine`

One example line of a reading card, ready to paint.
The head is the frame in brackets before the sentence, empty when the row states none.
`CLeafLineSentence` is the handle a click hands back to the find gate.
The gate reads the text, language and Mentions itself, so none of them rides on the line.
The pieces are the sentence divided at its Mentions, with the unknown mark for an unknown one.
The driver draws them run by run.
The citation is the cited Source's line, empty when the row cites none.
The glosses are the sentence in other languages, in the order the Example keeps them.
`LLeafLineRead` has the engine compose the frame, the sentence and the Source line through `examples`.
It uses the rule the portrait prints by, and the sentence arrives divided around its Mentions in that same call.
A row quoting no Example has no text to divide and no Source line.

## Stamp

Commit: `c2f335c331fe`
- `CDisplayCard.cs` `34833f92d482bf8e`
- `CLeaf.cs` `00a7c9c3e7c4b2b8`
- `CLeafChip.cs` `d604e4f1c5b56d4f`
- `CLeafLine.cs` `6e7cb04876fe7741`
- `CLecternCard.cs` `4f3140b154d29b23`
- `CLecternEtymology.cs` `0fc451bebb59080d`
