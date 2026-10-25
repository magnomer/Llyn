# Leaf

This folder holds the reading view's card area with the cards, their chips and example lines.

## `CDisplayCard`

The card area reads the cards, the usages pointing at the entry and the etymology map.
It also folds or unfolds one card of the chosen entry.
It keeps only the latest answer of its three reads.
Those are the cards, the usages and the etymology state.
The compass decides its shown parts from those answers, so the area cannot recompute them.
Each of the three reads calls the compass refine once the answer is kept.
So the contents follow the page.
`CDisplayCardReset` empties the three answers when an entry opens or closes, so no part of a gone entry lingers.
It takes the rules whole, since the shown draft and the chosen entry change after it is built.
Every read here runs on a repaint and shows its failure through repaint memory, so a lasting fault shows once.

### `CDisplayCardRead`

No shown draft answers no cards, with no section shown, and that empty answer is kept and announced too.
Each read records the draft it minted its line keys from.
The route compares that draft with the shown draft on a click.
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
The card is named by the leaf's own `CLeafCollocated` and `CLeafId`, handed back unread.
The gate maps the flag to the card kind the port saves and deletes under.
No entry chosen writes nothing and answers false.
A refused write shows `Fold.SaveFailed` every time, since the user acted, and answers false.
The caller puts its chevron back on false, since no bulletin will repaint it.
The port raises the fold bulletin.
So this view and every other view on the entry repaint from the store alike.
Folding is no edit, so it never dirties a draft and never enters undo.
`CDisplaySeeOpen` opens a collocation row that a "see" line names.
It unfolds the row through the same gate and answers false when the gate does.
On success it announces the card as sought, so the page scrolls to the row it just opened.
`LDisplayFoldRead` answers every folded card as a kind and id pair, Meanings and Collocations together.
No entry chosen answers an empty set, which keeps the cards drawable.
`CDisplayIncomingRead` has the engine name each usage and its epithet in one read.
Each usage maps through `COeuvre.COeuvreUsageRead`, the one map the vita's citations share.
A refused read shows `Display.IncomingFailed` once until the user acts, and answers no usages.
The kept usages are cleared first, so a refused read never leaves the last entry's usages behind.

### `CDisplayEtymologyRead`

It answers the etymology map of the shown draft as the state the editor's gate also answers.
The map is read from the shown draft's own boxes.
The read needs the chosen entry's id as well, since the lineage walk starts its cycle guard there.
No shown draft or no chosen entry answers an empty state.
The relation list is read for the relation words.
The language choices stay empty, since the reading view offers no picker.
The state carries the shown headword, which stands at the foot of the map.
No box is restorable here, since the reading view edits nothing.
A refused read shows `Display.AncestryFailed` through repaint memory and answers an empty state.
Like the other reads, it keeps its answer, empty or not, and refines the compass.

## `CLecternCard`

The cards of the reading view, ready to paint, with whether their sections show.
`CLecternCardOther` holds the collocations linked to no meaning the draft still holds.
`CLecternCardCollocated` is whether that list holds any card, which shows the Other collocations section.
Linked collocations show inside the leaf of the top-level meaning above their home instead.
`CLecternCardDefined` is whether the entry has a Meaning, which shows the Meaning section.

## `CLeaf`

One meaning or collocation card of the reading view, ready to paint.
Every text arrives worded and every row arrives mapped, so the leaf fill only looks keys up and paints.
`CLeafId` and `CLeafCollocated` are handed back unread to the fold gate.
`CLeafCollocated` says which list the id belongs to, since the fold is looked up by kind and id.
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
`CLeafHomed` is true when the card has collocations homed under it, so the heading and rows show.
`CLeafHeading` is the ready "Collocations" caption over a meaning's rows, empty when nothing homes there.
`CLeafQuoted` and `CLeafShared` are the verdicts a closed row shows its markers on, so the view never measures the text.

### `LLeafRead`

It maps the cards of one list to their ready form.
The caller says whether the list is the collocations, and the map keeps that flag on each leaf.
The nested homed collocations are always read as collocations.
A collocation leaf takes its phrase, flags and markers from the layout.
A meaning leaf takes its homed rows and stub lines.
Each leaf ignores the other's share, so a shared id reads nothing across.
The layout is the engine's answer for where each collocation sits, so the map applies no link rule itself.
It only looks up each card's homed collocations, stub lines, phrase and markers by the card's id.
The nested collocations and the Other collocations read through the same map with the same layout.
Nesting stops there, since nothing homes under a collocation.
The stub is the format of a stub line, filled with the phrase and the home path.
The example wordings give no marker for a count of zero.
The also wording joins the further paths with commas, and no paths give no marker.
The heading is the plain caption with no count, and a meaning with no homed collocation gives none.
The cards come in the order `CFolio.CFolioOrderRead` sets, the order the editor's list shows.
So the reading view, its contents and the editor number every card alike.
The order, the unknown mark, the Source lines and the link targets are read once for the entry.
Each card's links are its own entry of the target map, which the engine keys by card kind and id.
So a meaning and a collocation sharing an id never read each other's links.
A card missing from the map has no links, since a refused target read answers an empty map.
Each picture row carries the address `media` resolves, so the page loads it without asking.
Each sentence line is composed through `examples`, which also answers its line key against the shown entry.
The folded cards come from one entry read as kind and id pairs.
Each leaf looks itself up by its own kind.

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
`CLeafLineSentence` is the line key the engine mints, the handle a click hands back to the find gate.
The line key is the sentence's position among the shown entry's sentences, so it is no stored row id.
The gate reads the text, language and Mentions itself, so none of them rides on the line.
The pieces are the sentence divided at its Mentions, with the unknown mark for an unknown one.
The driver draws them run by run.
The citation is the cited Source's line, empty when the row cites none.
The glosses are the sentence in other languages, in the order the Example keeps them.
`LLeafLineRead` has the engine compose the frame, the sentence and the Source line through `examples`.
It uses the rule the portrait prints by, and the sentence arrives divided around its Mentions in that same call.
A row quoting no Example has no text to divide and no Source line.

## Stamp

Commit: `36e31eb71c98`
- `CDisplayCard.cs` `b0b3725d8dde7c80`
- `CLeaf.cs` `380d9627a7ca6bb2`
- `CLeafChip.cs` `d604e4f1c5b56d4f`
- `CLeafLine.cs` `247bfa9b404d5d91`
- `CLecternCard.cs` `4f3140b154d29b23`
