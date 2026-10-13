# Source

The source folder holds the sources panel and the colophon page that reads one Source.
It stands apart from the entry panels because a Source is written down first and cited afterwards.
The colophon is a control of its own.
This is since the sources panel and the authors panel both read a Source through it.
Their two readings cannot drift apart.
The panel row and card templates live in dictionaries here, because the panel outgrew one file.

## `PColophon`

The shell behind `PColophon.xaml`, placed by the Source panel and the guild.
Its constructor only calls `InitializeComponent()`.
The markup is the control itself, and the Deportment class `QColophon` fills its named parts.
The title, heading, value and chip styles live in the control, because two panels read a Source through it.
The chips are built on the speech chip, so the control merges the speech theme to reach it.
`Colophon.Heading` has the height, margin and size of the entry display's section headings.
So a Source reads under the same headings as an entry and a Situation.

## `PColophonBody`

The reading of one Source, laid out as the repertoire panel lays out a Situation.
The title stands at the head of the page.
The kind and the citation count stand as chips on the row beneath it.
The authors, the year, the address and the note are headings over their values.
No field is labeled, because the entry display labels nothing.
A never-written field is not drawn at all.
An unknown value reads the unknown mark where the value would stand, and unknown authorship reads it under its heading.
A never-written title reads the untitled text in the muted colour, because the head of the page cannot be empty.
The editing side still draws every slot, so the two kinds of empty are one toggle apart.

## `PColophonUnselected`

The prompt shown while no Source is chosen, centred where the reading would stand.

## `PReference`

The shell behind `PReference.xaml`, the reference page the window places.
Its constructor only calls `InitializeComponent()`.
The row and card templates live in `PReferenceShelf.xaml` and `PReferenceFootnote.xaml`.
Only styles built on another style stay in the panel, because a standalone dictionary cannot resolve one.
The rows wear `Theme.Catalog.Row` directly, and the panel marks the chosen one.

## `Theme.Shelf.Meta`

The shelf rows reach this key by a dynamic reference, so each panel merging the shelf defines it.
The authors panel defines the same style, and the two must stay alike.

## Seams

The seams part the source catalog, the entries citing the chosen source, and the source itself.
The row seam runs under the ordering bar and the command row and is bled past the panel margin.
The two column seams sit in the middle of the gutters and reach the foot of the window.
Neither seam encloses anything, because a line marks a division and a box would claim an object.

## `PGrade` and `PTrellis`

`PGrade` joins the ordering button and the search field into one control over the catalog column.
The catalog column takes 380, because a row carries a name beside an author-and-year line.
The ordering picker is the shared `PChoiceOrder`, placed as `PReferenceOrder`, and its menu anchors under the whole bar.
There is no ordering by kind of material, because no Source-type discriminator is stored.
Ordering by recency is not offered, because the `source` row carries no creation or modification time.
A field standing Unspecified or Unknown still has a place in every ordering, ordered by its state.
`PTrellis` holds the search field and language filter over the middle column, copied from the tenor panel's `PGrille`.
`PRummage` narrows the entries citing the chosen Source by typed text.
The language filter is the shared `PChoiceFilter`, placed as `PReferenceFilter`.

## `PReferenceRail`

The panel's action row, the shared `PPanelRail`.
While no Source is held, Fresh opens the edit area on a Source nothing cites yet.
That is the normal case, because a Source is written down first and cited afterwards.
While one is held, Fresh opens a new Entry for the held Source to cite.
Save stores whichever editor is in front, the Entry open in `PEditor` or the held Source.
It stands in the rail at all times.
The undo pair walks the editor's chronicle.
Export takes the Entry shown, and print takes what is read.

## `PShelf` and `PReferenceShelf`

`PShelf` lists every Source the workspace holds, including one nothing cites.
`PReferenceShelf.xaml` draws the row with the resolved name beside its credits and year, and the citation count at the right.
The authors panel merges it too, so a Source reads alike in both catalogs.
The panel fills each row's named parts.
It takes each row's click once on the whole list, through a routed handler.
The row names no handler and binds nothing, because a dictionary carries no code.
`PShelfRow` wears the shared catalog row by a dynamic reference.
This is since a standalone dictionary cannot resolve a style built on another.
`PShelfEmpty` covers both an empty workspace and a search that matches nothing.

## `PDisplay`, `PEditor` and `PColophon`

`PDisplay` and `PEditor` are the entry display and editor the library and tenor panels share.
They are drawn in the same cell as the Source reading.
`PDisplay` shows an Entry or a Source, never both, so it stays collapsed until an Entry row is chosen.
The mode toggle swaps it for `PEditor`.
`PEditor` stands in front only while an Entry is shown and the toggle is on the editing side.
The `PColophon` control is shown in that cell while a Source is read.
The mode toggle is not in the colophon and sits in the action row as in every built panel.

## `PFootnote` and `PReferenceFootnote`

The middle column lists every Entry whose cards quote an Example citing the selected Source, one row per Entry.
Only Entries are listed, so an Example citing the Source is reached through the Entry quoting it.
A Situation is not listed, because a Situation is written rather than quoted and cites no Source.
While no Source is chosen, every Entry is listed, as the tenor panel lists every Entry under no register.
`PReferenceFootnote.xaml` draws the row with the flag, the headword and the language, and it leads to that Entry.
`PFootnoteRow` wears the shared catalog row by a dynamic reference, for the same reason as the shelf row.
Every part is named, and the panel fills it, marks the chosen row and takes its click.
Choosing a row shows that Entry in `PDisplay` in place of the Source reading, without leaving the tab.
The list is read-only.
A citation is set or cleared where it is held.

## `PImprint`

The edit area is a control of its own.
It sits over the read area in the same cell and at the same top inset.
One Source is either being read or being written.
The panel decides which is shown, so the mode toggle stays in the action row.

## `PReferenceBin`

Deletes the shown Source, from the reading side or the editing side alike.
It stands in the corner of the surface as the repertoire panel's bin does.
It is disabled while nothing is selected and while an Entry is shown.
It is disabled too while the edit area stands on a Source nothing has stored yet.
Its icon is the named `PReferenceBinIcon`, set by the rail driver, which also takes its click.

## Catalog spacing

The catalog uses the shared `Theme.Catalog.Frame` and `Theme.Catalog.Scroll` styles.
The scroll bar takes its lane only when the list overflows, and the rows then narrow.

## Stamp

Commit: `c2f335c331fe`
- `PColophon.xaml` `c00de7416e62b43a`
- `PColophon.xaml.cs` `7067a963243840b8`
- `PReference.xaml` `bc37229db77dd1ab`
- `PReference.xaml.cs` `a6aadd02ae8e481b`
- `PReferenceFootnote.xaml` `6d39d9bac7df131f`
- `PReferenceShelf.xaml` `fa263b4fc98f1928`
