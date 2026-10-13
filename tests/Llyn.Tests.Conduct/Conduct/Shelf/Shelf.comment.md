# Shelf

This folder holds the sources panel's gates.
They cover the source catalog, imprint edit area and credits, footnote entry list, draft, roll and vista.

## `TFootnote`

Covers the sources panel's entry list end to end on a real workspace.
With no Source chosen it lists every entry, and a chosen Source that nothing cites empties it.
An unmatched query empties the rows, and the empty list then reads as unmatched rather than vacant.
A fresh entry opens in the editor, citing the chosen Source, or citing nothing while none is chosen.
The create and the vista restore are internal helpers, reached through relays.
Its flag-fill load answers the rows and the catalog languages once the fill has run.
The list is built over the atelier's ports and the desk of a fresh entry editor.
It starts a source vista as the parent and a footnote vista that the list and the editor share.
Its seams answer that the tab is in front and that every finish succeeds.

## `TImprint`

Covers the source editor's fields and draft on a real workspace.
A typed title crosses raw and the engine holds it specified, while a blank year holds nothing.
A kind tag round-trips through the draft and leaves a chronicle step back, and a null tag sends nothing.
A tag naming the kind the Source already has sends nothing either.
A session save on an unchanged Source keeps the draft, and one after a change commits it.
Opening a stored Source announces its fields, and the empty read gives the blank Source's hints.
The kind menu offers every kind in menu order, each with its tag and localization key.
Hostile kind rows give an empty menu, or one option per ordinal tag, the first kind kept.
A cancel drops the draft and closes the byline with one notice.

## `TImprintPrepare`

Builds the editor through `TInterfaceCitation.TImprintCreate` over the atelier and a source vista.
Its marshal runs each desk notice at once.
The credit and byline suites share it.

## `TImprintCredit`

Covers the source editor's credit rows and the one blank row it keeps.
A fresh draft opens with one blank row, and no draft reads as no rows.
The blank row arrives placed among the rows, with id zero, no name and no move.
Add under a credit opens a blank row beneath it and asks for the caret.
Add on the blank row only asks for the caret, and add with no credit elsewhere does nothing.
Neither raises a change or touches the engine's credits.
Remove on the blank row closes it again.
Later and earlier shift a credit through the engine, so the rows come back reordered with their move verdicts.
The blank row and a row with no place move nothing.
Add and move hand only the row's place and id, and remove only the id, as the driver does.
Enter on a blank name reverts, and on a new name it creates the Author.
Enter on an unchanged name keeps the credit.
Enter on a blank name in the blank row keeps that row open, and a new name there closes it.
No draft or no row leaves the key untaken, and Escape with no byline reverts the field.

## `TShelf`

Covers the sources panel's gates end to end on a real workspace.
A chosen Source shows its colophon, enables the bin and reads its worded tally.
A fresh request with a Source chosen opens an entry that cites it from its first paint.
A fresh request with nothing chosen opens a Source draft in the source editor.
The bin on the entry side leaves the chosen Source, and only the Source side deletes it.
A null order keeps the order already set.
An unmatched query empties the shelf and closes the shown Source.
A hidden language marks the shelf filtered, and an empty filter clears the mark.
The leave and draft gates live in `TShelfDraft`, and the notices live in `TShelfVista`.

## `TShelfPrepare`

Builds the shelf over the atelier with its own editor, and the shelf restores both vistas itself.
Its seam answers that the tab is in front, and its marshal runs each notice at once.
`TShelfDraft`, `TShelfImprint`, `TShelfRoll`, `TShelfVista` and `TFaultPortrait` share it.

## `TShelfDraft`

Covers how the sources panel leaves, saves and finishes a held draft on a real workspace.
It builds each shelf through `TShelf.TShelfPrepare` and stores entries through `TShelf.TShelfEntrySave`.
A kept leave on a Source click records no voyage station and stays on the held draft.
A stored leave saves the held draft and shows the stored Source.
The rail's undo, redo and save act through the session on the Source draft while the Source side edits.
A finish without storing drops the draft of the side in front.
Only the save test sets the edit delay to zero.

## `TShelfImprint`

Covers how the sources panel wires its source editor on a real workspace.
Closing the atelier closes an offered byline through the shelf's registered closure, with one notice.
The draft is persisted before the notice, so the deferred title raises no second notice.

## `TShelfRoll`

Covers the shelf's one rows answer on a real workspace.
The rows, the empty verdict and the chosen Source's worded tally arrive together, so the view reads once.
A chosen Source the query filters out closes before the tally is read, so the tally counts nothing.
Each row words its authors and year ready, by their text or by the unknown or unset key.
A credited author shows even while the authorship is marked unknown.

## `TShelfVista`

Covers the sources panel's vista restore and its observers on a real workspace.
A second restore carries both held queries into the fresh vistas.
After it a Reference notice raises each list once, through the marshal, from the fresh vistas only.
A create alone restores once, so the same notice after only a create raises once too.
An entry notice refills the Source rows, so their tallies follow, and the footnote rows through the marshal.
An entry notice with the entry side closed shows the chosen Source again.
A workspace notice closes the chosen Source through the marshal.
Closing the atelier cancels the entry editor's desk and stops playback.
The flag-fill load answers the roll and its languages once the fill has run.
`TShelfEntrySave` here repeats the helper in `TShelf`.

## Stamp

Commit: `c2f335c331fe`
- `TFootnote.cs` `ab8f0e092db8fcaa`
- `TImprint.cs` `ccaa649781c90db8`
- `TImprintCredit.cs` `891538d46c0dfe30`
- `TShelf.cs` `fa452ff3c4f6ef9b`
- `TShelfDraft.cs` `85afc10466e4018a`
- `TShelfImprint.cs` `35bdb3a43e8334c3`
- `TShelfRoll.cs` `9cbd88b27104960e`
- `TShelfVista.cs` `0a87912c5b3211cd`
