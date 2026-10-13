# Source

The drivers of the Source panel, which lists Sources, the Entries citing them, and a read area.
It stands apart from its neighbours because a Source is independent data owned by nothing.
The panel is not a view of one citation of a Source.
The visible label is Source and the internal base is Reference, because Source already names a pronunciation source.

## `QReference`

The driver of the Source panel.
It is the chronicle host its own rail walks, so the rail's undo and redo reach the session.
Every branch it once carried lives in the Conduct shelf `CShelf` and the two `CPanel` it holds.
The driver forwards each handler to the shelf and writes the page's parts when a notice arrives.
The Source list and the entry list have drivers of their own, `QShelf` and `QFootnote`.
The rail, the order picker and the language filter are shared drivers, built here and fed from Conduct.
The rail is built with Fresh and Portrait shown, since an Entry is read and exported here.
The order picker hangs its menu under the whole `PGrade` bar.
The driver picks the area in front by the shelf's side verdict.
The rail takes the scribe verdict, the mode and bin enablement, and the store verdict.
Each named part of the page is found through `QContract.QContractFind` under its markup name.

`QReferenceIntroduce` puts the driver to work on the Conduct shelf, which builds its own editor.
Only the medium knows its dispatcher, so the marshal for the area's notices is built here.
The Source list driver also gets the colophon, whose tally it repaints from each Source answer.
The Source rows notice also repaints the imprint's tally, since a stored entry may have moved its count.
The grade menu is built once, from the orders `CShelf.CShelfOrderRead` offers.
The Source list's aperture feeds both the order picker and the language filter.
The display view builds the lectern over the editor's display, which Conduct attached to the footnote panel.
The print and portrait command bindings are added last, so no can-execute query meets a shelf not yet built.

`QReferenceVistaRefine` answers the workspace opening, after the shelf has restored its vistas.
The entry list answers the same opening with its own load, so both loads run side by side.
The flags load before the first rows are built, because an entry row reads its flag at construction.
The filter menu is built from the languages that load answers.
The Source list driver paints the rows of that same answer last.
Its one request is `CShelfRollLoad`, which runs the flag fill and then answers the rows it paints.

`QReferenceShownCheck` tells the shelf whether the Source page is on screen.
A bulletin is then acted on only while shown.

`QReferenceExitRefine` releases the entry player and closes both pickers' menus when the window exits.
The imprint's kind menu closes with them.

`QChronicleUndoObserve` walks the session back one step, with the caret kept where it was.
`QChronicleRedoObserve` is its mirror.

`QReferencePressObserve` prints the entry being read, or else the source being read, as the engine portrays it.
The shelf asks for the ticket through the envoy, picks the vista and words the page.
Nothing is read back from the screen.

`QReferencePortraitRefine` enables export from the shelf's portrait verdict, which holds only while an entry is read.

## `QShelf`

The left column of the Source panel, with the Source search field over it.
It lists every Source the workspace holds, including one nothing cites.
Choosing a row shows that Source in the colophon, through the shelf's select gate.
It subscribes what it paints itself, so the owner `QReference` only builds, introduces and first paints it.
`QShelfIntroduce` takes the colophon driver, because each Source answer carries the colophon's tally.
`QShelfRefine` paints the rows, the empty notice and the colophon tally from one shelf answer.
The rows are spliced, so the list keeps its scroll position.
`QReferenceVistaRefine` hands in the roll its flag load answered, so the paint asks Conduct nothing.

## `QShelfItem`

One Source row in the shelf list.
The id is identity and never displayed.
A Source is shown by the first field it actually states, then by its id if it names itself nowhere.
It is never offered as a blank row the reader could not tell from the next one.
`Untitled` is a stated title and shows as one, and `Anonymous` is a credited Author.
The name, the credits, the year and the citation count arrive ready from Conduct.
Only the lookup of the localized key and the count's format happen here.
The chosen mark is handed to the constructor, because the setter writes the same field from a plain value.
`QShelfItemBuild` is a plain copy loop, so the panel that asks for it carries no loop of its own.
The citation count decides whether a delete is legal, so the catalog carries it and not only the editor.
`QShelfItemChosen` is the only value that changes after the row is built.
The engine row carries it, and `QSplice` moves the mark in place, so the list keeps its scroll position.
`QShelfItemMatch` compares rows with the chosen mark left aside.
`QSplice` keeps the held rows when every pair matches, so their containers survive a refresh.
`QShelfItemSync` copies the chosen mark of the fresh row onto the kept row.
`QShelfItemRefine` is attached by both the sources panel and the authors panel.
Each of them takes the clicks on its own list.

## `QFootnote`

The middle column of the Source panel, with the entry search field over it.
It lists every Entry quoting an Example that cites the chosen Source, or every Entry while none is chosen.
Choosing a row shows that Entry in the display, through the shelf's entry gate.
It subscribes what it paints itself, so the owner `QReference` only builds and introduces it.
`QFootnoteVistaRefine` answers the workspace opening for the entry list, which has its own first paint.
It subscribes before the owner, so it starts before the Source list's own load.
Its one request is `CFootnoteRowsLoad`, which runs the flag fill and then answers the rows it paints.
`QFootnoteRefine` shows the empty text while no row stands.
The aperture chooses the text's key by whether the list is being searched.
The form taking rows paints what a flag-fill load answered, so the paint asks Conduct nothing.

## `QFootnoteItem`

One Entry as a row of the sources panel's entry list.
It mirrors the taxonomy panel's row, because every middle column answers the same shape of question.
That question is which Entries cite the Source chosen in the shelf beside them.
An Entry referencing from several of its cards is still one row, because the row stands for the Entry.
The headword and the shown name are separate, so two Entries sharing a headword can be numbered apart.
`QFootnoteItemEpithet` is empty when the setting is off or the entry keeps none.
`QFootnoteItemBuild` keeps the order the shelf returned.
Each row carries its own chosen mark into the constructor, so the mark has one kind of writer.
`QFootnoteItemChosen` is the one value that changes after the row is built.
The engine row carries it, and `QSplice` moves it in place, so the list keeps its scroll position.
`QFootnoteItemMatch` and `QFootnoteItemSync` follow the same match and sync rule as `QShelfItem`.
`QFootnoteItemRefine` leads the epithet with an en space, as the other catalog rows set it apart.

## `QColophon`

The driver of the read area over one Source, shared by the sources panel and the authors panel.
It draws the sheet it is handed and asks the engine for nothing.
The owning driver builds it over the nested `PColophon` page and decides what is read.
The owning driver also decides when the page is shown, because only it knows what else stands in the cell.
Every part is pulled from the page by contract ID, so no name scope is copied.
`QColophonRefine` gives each field one text and one look, and shows the page.
A never-written field hides its heading, so a blank line never stands for two facts.
An unknown field is dressed as the placeholder the edit side shows, so the two sides read alike.
`QColophonTallyRefine` rewrites the citation chip alone, for when the count changes under a Source that stays shown.
`QColophonClearRefine` hides the page and shows the prompt in its place.

## Stamp

Commit: `c2f335c331fe`
- `QColophon.cs` `e705a093da63a352`
- `QFootnote.cs` `0659f4f617103495`
- `QFootnoteItem.cs` `4a667633a340bc4f`
- `QReference.cs` `53c16810e2c9f1c7`
- `QShelf.cs` `d249657b23c1f4b4`
- `QShelfItem.cs` `b3332826cebe73ac`
