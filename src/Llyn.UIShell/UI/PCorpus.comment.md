# PCorpus.xaml

## `<Rectangle ... Style="{StaticResource Theme.Panel.SeamRow}" />`

The seams that part the example catalog, the entries quoting the chosen example, and the example itself.
The row seam runs under the ordering bar and the command row.
It is bled past the panel margin so it meets the navigation's own edge.
The column seam sits in the middle of the gutter and reaches the foot of the window.
Neither seam encloses anything, which is the whole rule: a line marks a division, a box would claim an object.

## `<Border x:Name="PRank" ... Style="{StaticResource Theme.Search.Bar}">`

The ordering button and the search field are one control over the catalog column.
The action row over the broader column stands in the same top row, as the repertoire panel arranges it.
The catalog column is wider than the entry panels take, because it holds sentences rather than headwords.
The catalog carries no language filter of its own, as the tag catalog carries none.
The entries in the middle column are what is filtered.

## `<Border x:Name="PGauze" ... Style="{StaticResource Theme.Search.Bar}">`

The search field and the language filter over the middle column, copied from the tenor panel's `PGrille`.
`PDredge` narrows the entries quoting the chosen Example by typed text.
`PGauzeDropper` opens the menu of loaded languages, and `PGauzeMark` shows while any is hidden.

## `<Popup x:Name="PRankDropdown" ...>`

The orderings the catalog may be listed in, one radio row each, carrying its choice in `Tag`.
Ordering by recency is not offered, because the `example` row carries no creation or modification time.
An Example whose text is unwritten still has a place in every ordering, ordered by its mark rather than dropped.

## `<StackPanel Grid.Row="0" Grid.Column="1" ...>`

The action row of the panel.
`PCorpusFresh` opens the editor on an Example nothing quotes yet.
This panel is the only place such an Example can arise.
Elsewhere one is written from the card that quotes it.
`PCorpusStore` saves whichever editor is in front, the Entry open in `PEditor` or the held Example.
It stands in the rail at all times, as the repertoire panel's save does.
Export and print are mock-up controls and are not wired.

## `<Grid x:Name="PTranscript">`

The scribe binds the four Mention commands on its root.
So the transcript's menu and its chip line reach the same handlers.
The transcript field carries the shared linking menu, and the chip line stands right under it.

## `<ItemsControl x:Name="PAnthology">`

The catalog of every Example the workspace holds, including one nothing quotes.
A row reads its flag, its sentence and its language, with its usage count at the far end.
That count is shown here and not only in the display, because it decides which delete the panel offers.
The cited Source is not shown here.
A row says what the sentence is, not where it came from.
The reading beside the catalog carries the Source, so nothing is lost by leaving it off the row.
The translation is not shown here either.
It is text of the sentence itself and says nothing about where the sentence is used.

## `<local:PDisplay x:Name="PDisplay" Visibility="Collapsed" />`

The entry display the library and tenor panels share, drawn in the same cell as the Example reading.
It shows an Entry or an Example, never both, so it stays collapsed until an Entry row is chosen.
The mode toggle swaps it for `PEditor`, so an Entry is read and written here as in the tenor panel.

## `<local:PEditor x:Name="PEditor" Padding="30,18,30,30" Visibility="Collapsed" />`

The entry editor the library and tenor panels share, drawn in the same cell as the display.
It stands in front only while an Entry is shown and the toggle is on the editing side.

## `<Grid x:Name="PExcerpt">`

The reading of one Example, laid out as the repertoire panel lays out a Situation.
`PExcerpt` here stands on an Example rather than on an Entry.
What it shares with the entry panels is the read-and-edit mechanism, the `PCorpusScribe` toggle, and the shape of the page.
The sentence stands at the head of the page.
The language and the usage count stand as chips on the row beneath it.
The translations and the cited Source are headings over their values, as a situation's description is.
No field is labeled, because the entry display labels nothing: position and dress say what a value is.
A never-written translation or citation is not drawn at all, as a situation with no description draws none.
An unknown citation reads the unknown mark where the Source would stand.
A never-written sentence reads the unwritten text in the muted colour, because the head of the page cannot be empty.
The editing side still draws every slot, so the two kinds of empty are one toggle apart rather than lost.

### `<local:PMention x:Name="PExcerptText" ...>`

The sentence is drawn by the same control the display cards use, so a word of it can be clicked.
It is dressed as the head of the page.
It is smaller than a headword, because a sentence is longer than a word.
The click is answered by the panel, which knows which Example is open.

## `<ItemsControl x:Name="PQuotation">`

The middle column: every Entry whose cards quote the selected Example, one row per Entry.
An Entry quoting it from several cards is still one row.
While no Example is chosen, every Entry is listed, as the tenor panel lists every Entry under no register.
A row reads the flag, the headword and the language, as a cohort row does.
Choosing a row shows that Entry in `PDisplay`, in place of the Example reading, without leaving the tab.
The list is read-only: a reference is added or dropped on the card holding it, never here.
An Example nothing quotes shows the empty state rather than hiding the column.

## `<Grid x:Name="PTranscript" Visibility="Collapsed">`

The editable view of the selected Example, laid out as the reading side lays it out.
Every slot the reading side draws is drawn here in the same place.
A bare field stands where the text stood.
The sentence is written in the head of the page, with its Mention chips right under it.
The language is chosen inside the same chip the reading side draws.
It is required, so it is chosen rather than typed.
The sentence and the translation are both three-state.
An empty field says which kind of empty it is through its placeholder.
The translation is the sentence rendered in plain text and links nothing, unlike the Translation a Meaning carries.
Each Gloss row is drawn as the reading side draws it, the flag and then the text.
A bare field stands where the text stood.
The flag is a switch that opens the language list.
The plus and minus handles stand at the end of the row and show under the pointer.
That is how the entry editor's rows carry them.
Plus adds a row after this one and minus drops this one.
`PTranscriptSeed` stands in while there is no row.
It is a bare field showing the placeholder whose focus or plus adds the first.
`PCitation` is the single Source the Example cites, a pointer that is cleared without touching the Source itself.
It is drawn as the text the reading side draws, with a small arrow that opens the shelf.
The editor carries no buttons of its own.
Save is the rail's `PCorpusStore`, delete is `PCorpusBin`, and there is no discard.
That is how the repertoire panel arranges a Situation.
Leaving the editor asks about the draft as it does there.

## `<TextBlock x:Name="PTranscriptTally" ...>`

How many places quote the Example, kept visible while it is being edited.
Rewriting an Example here rewrites what every one of them quotes.
A user correcting a sentence must see how far the correction reaches without leaving the editor.

## `<Button x:Name="PCorpusBin" ...>`

Deletes the shown Example, from the reading side or the editing side alike.
It stands in the corner of the surface as the repertoire panel's bin does, apart from the browsing beside it.
It is disabled while nothing is selected and while an Entry is shown.
It is disabled too while the editor stands on an Example nothing has stored yet.

## Catalog spacing

The catalog uses the shared Theme.Catalog.Frame and Theme.Catalog.Scroll styles.
Rows keep 6 device-independent pixels on both sides, with a reserved scrollbar lane so their width stays stable.
The shared Theme.Catalog.Row preserves the same rounded shape, internal padding and row spacing across browse panels.
