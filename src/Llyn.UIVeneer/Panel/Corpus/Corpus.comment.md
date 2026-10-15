# Corpus

This folder holds the corpus panel, which lists every Example the workspace holds beside the entries that quote it.
The panel keeps its layout in `PCorpus`.
The excerpt and transcript dictionaries hold the shapes of its reading and editing sides.
Those two dictionaries are loose, answer no event and have no class of their own.
The corpus driver subscribes its own handlers on each realized part.

## `PCorpus`

The C# shell only calls `InitializeComponent()`.
It arranges the repertoire panel's pattern for an Example instead of a Situation.

### Seams

Seams part the example catalog, the entries quoting the chosen example, and the example itself.
The row seam runs under the ordering bar and the command row.
It is bled past the panel margin to meet the navigation's edge.
The column seams sit in the middle of their gutters and reach the foot of the window.
No seam encloses anything, because a line marks a division and a box would claim an object.

### `PRank`

The ordering button and the search field are one control over the catalog column.
The catalog column is wider than the entry panels take, because it holds sentences rather than headwords.
The ordering picker is the shared `PChoiceOrder`, placed as `PCorpusOrder`, and the panel anchors its menu under the whole bar.
Ordering by recency is not offered, because the `example` row carries no creation or modification time.
An Example whose text is unwritten keeps a place in every ordering, ordered by its mark rather than dropped.

### `PGauze`

It holds the search field and the language filter over the middle column, copied from the tenor panel's `PGrille`.
`PDredge` narrows the entries quoting the chosen Example by typed text.
The language filter is the shared `PChoiceFilter`, placed as `PCorpusFilter`.

### `PCorpusRail`

It is the shared `PPanelRail` as the panel's action row.
Fresh opens the editor on an Example nothing quotes yet.
This panel is the only place such an Example can arise.
Elsewhere one is written from the card that quotes it.
Save stores whichever editor is in front, the Entry open in `PEditor` or the held Example.
It always stands in the rail.
Reading walks the window's trail of records, and writing walks the chronicle of the editor in front.
Export takes the Entry shown, and print takes what is read.

### `PAnthology`

It is the catalog of every Example the workspace holds, including one nothing quotes.
A row reads its flag, sentence and language, with its usage count at the far end.
The count is shown here because it decides which delete the panel offers.
The cited Source and the translation are not shown.
A row says what the sentence is, not where it came from.
The reading beside the catalog carries the Source.
The translation is text of the sentence itself and says nothing about where it is used.
The panel fills the named row parts and marks the chosen row.

### `PDisplay`

It is the entry display the library and tenor panels share, drawn in the same cell as the Example reading.
It shows an Entry or an Example, never both, so it stays collapsed until an Entry row is chosen.
The mode toggle swaps it for `PEditor`, so an Entry is read and written here as in the tenor panel.

### `PEditor`

It is the shared entry editor, drawn in the same cell as the display.
It stands in front only while an Entry is shown and the toggle is on the editing side.

### `PExcerpt`

It reads one Example, laid out as the repertoire panel lays out a Situation.
It shares the read-and-edit mechanism and the rail's mode toggle with the entry panels.
The sentence stands at the head of the page.
The language and usage count stand as chips on the row beneath.
The translations and the cited Source are headings over their values, as a situation's description is.
No field is labeled, because position and dress say what a value is.
A never-written translation or citation is not drawn at all.
An unknown citation reads the unknown mark where the Source would stand.
A never-written sentence reads the unwritten text in the muted colour, because the head of the page cannot be empty.
The editing side still draws every slot, so the two kinds of empty are one toggle apart rather than lost.

### `PExcerptText`

The same control the display cards use draws the sentence, so a word of it can be clicked.
It is dressed as the head of the page.
It is smaller than a headword, because a sentence is longer than a word.
The panel answers the click, because it knows which Example is open.

### `PQuotation`

The middle column lists every Entry whose cards quote the selected Example, one row per Entry.
An Entry quoting it from several cards is still one row.
While no Example is chosen, every Entry is listed, as the tenor panel lists every Entry under no register.
Choosing a row shows that Entry in `PDisplay`, in place of the Example reading, without leaving the tab.
The list is read-only, because a reference is added or dropped on the card holding it.
An Example nothing quotes shows the empty state rather than hiding the column.

### `PTranscript`

It is the editable view of the selected Example, laid out as the reading side lays it out.
The panel adds the five Mention command bindings to this grid from code.
So the transcript's menu and its chip line reach the same handlers.
Every slot the reading side draws is drawn here in the same place.
A bare field stands where the text stood.
The sentence is written in the head of the page, with its Mention chips right under it.
The language is chosen inside the same chip the reading side draws.
It is required, so it is chosen rather than typed.
The sentence and the translation are three-state, and an empty field says which kind of empty through its placeholder.
The translation is the sentence rendered in plain text and links nothing, unlike the Translation a Meaning carries.
Each Gloss row is drawn as the reading side draws it, the flag and then the text.
The flag is a switch that opens the language list.
The plus and minus handles stand at the end of the row and show under the pointer.
The entry editor's rows carry them the same way.
Plus adds a row after this one and minus drops this one.
`PTranscriptSeed` stands in while there is no row.
It is a bare field showing the placeholder, and its focus or plus adds the first.
Its plus stands in `PTranscriptShelf`, which the look sheet reveals under the pointer or focus.
`PCitationSheet` is as wide as the field it opens under, and the panel sets that on opening.
`PCitationField` holds the single Source the Example cites, a pointer that is cleared without touching the Source itself.
Typing into it opens `PCitationDrawer` with the matching Sources.
The drawer carries no width cap of its own, since the panel caps it at its window's width on opening.
The editor carries no buttons of its own.
Save is the rail's save, delete is `PCorpusBin`, and there is no discard.
Leaving the editor asks about the draft as the repertoire panel does.

### `PTranscriptAdditionIcon`

The seed's plus icon is named, so the driver sets its source by contract.

### `PTranscriptTally`

It shows how many places quote the Example, kept visible while it is being edited.
Rewriting an Example here rewrites what every one of them quotes.
A user correcting a sentence must see how far the correction reaches without leaving the editor.

### `PCorpusBin`

It deletes the shown Example, from the reading side or the editing side alike.
It stands in the corner of the surface as the repertoire panel's bin does, apart from the browsing beside it.
It is disabled while nothing is selected.
It is also disabled while an Entry is shown, and while the editor stands on an unstored Example.

### Catalog spacing

The catalog uses the shared `Theme.Catalog.Frame` and `Theme.Catalog.Scroll` styles.
The shared `Theme.Catalog.Row` keeps the same rounded shape, padding and row spacing across browse panels.
The anthology and quotation rows take it directly, and the panel marks the chosen one.

## `PCorpusExcerpt`

It holds the anthology meta and the excerpt page the rows open.
`Theme.Excerpt.Flag` hides an empty flag through its `Empty` row in the look sheet.
`Theme.Display.Value` is the face the read Gloss takes, reached by a dynamic reference.

### `Theme.Excerpt.Heading`

It has the height, margin and size of the entry display's section headings.
So the excerpt page reads as one more page of the same reader.

### `Theme.Excerpt.Chip`

The language chip is shaped as a speech chip and coloured in the accent, with the flag before the name.
The same shape dresses the language toggle on the editing side.
So the language reads the same whether read or chosen.

### `Theme.Excerpt.Tally`

The usage count is a chip of the same shape in the raised surface colour.
So it reads as a figure and not a language.
It reads as a sentence, because a bare number beside a sentence says nothing about what it counts.

## `PCorpusTranscript`

It holds the transcript fields, the Gloss row and the pickers under it.
The language choice and the citation row stand here because both open from the transcript.

### `Theme.Transcript.Sentence`

The field uses the size and weight the excerpt page prints the sentence in.
So the sentence does not move when the page turns from reading to writing.

### `Theme.Transcript.Speaker`

The toggle takes the height, margin and padding of the speech chip under the excerpt's language chip.
So the language stands in the same box when read and when chosen.

### `Theme.Transcript.Row`

The Gloss row and the seed share this surface, transparent so the pointer finds it.
The look sheet reveals `PTranscriptShelf` while the row is under the pointer or holds focus.

### `Theme.Transcript.Control`

The Gloss row's buttons start invisible and take no pointer.
The look sheet raises both while the row is hovered or focused.
A hidden button that still took clicks would drop a Gloss the reader never saw.

### `Theme.Transcript.Addition`

The style carries no icon, since a setter's element would be one instance shared by every button.
Each button holds its own bare icon, and the panel's fill gives it a source.

### `Theme.Transcript.Line`

It is the editable Gloss row, shaped as `Theme.Gloss.Display` shapes the read row.
It is written here rather than taken from the card's row template.
That template dresses its Gloss in the card's own face.
The reading side uses the display value face, so the editing side must too.

### `Theme.Transcript.Citation`

The citation is a bare text field, so the cited Source reads where and as the reading side reads it.
The frame shows only under the pointer or focus, which says the text can be changed.

### `Theme.Citation.Row`

It shows one offered Source for the citation field, with its usage count against the right edge.
It answers the pointer going down, as the candidate row of the entry editor does.
The popup takes the window's activation when pressed, and the press on `PCitationRow` must land before that.

### `Theme.Language.Choice`

It shows one language in the speaker dropdown, its flag before its name.
The driver subscribes the click, since it knows which transcript is open.
The flag and name reuse the Gloss option part names, so the language item fill serves both.

### `PGlossFlag`

The flag, ring, popup, picker list and text are named for `PGloss.PGlossRowApply`.
The shared fill names the list's value path.
The driver's own fill wires `PGlossAddition`, `PGlossRemoval`, the text and the list's choice.

## Stamp

Commit: `2f9a06b8eb95`
- `PCorpus.xaml` `ef2d336e5e27d519`
- `PCorpus.xaml.cs` `edacad6453b8037f`
- `PCorpusExcerpt.xaml` `4767f2cc016f085c`
- `PCorpusTranscript.xaml` `0971b525a138f847`
