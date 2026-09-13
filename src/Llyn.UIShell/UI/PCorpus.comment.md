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
`PCorpusStore` saves the Entry open in `PEditor`, and is shown only while that editor is in front.
A Example saves from its own editor, so the rail button never stands for both.
Export and print are mock-up controls and are not wired.

## `<Grid x:Name="PTranscript">`

The scribe binds the four Mention commands on its root.
So the transcript's menu and its chip line reach the same handlers.
The transcript field carries the shared linking menu, and the chip line stands right under it.

## `<ItemsControl x:Name="PAnthology">`

The catalog of every Example the workspace holds, including one nothing quotes.
A row reads its sentence over its language and cited Source, with its usage count at the far end.
That count is shown here and not only in the display, because it decides which delete the panel offers.
The translation is not shown here.
It is text of the sentence itself and says nothing about where the sentence is used.

## `<local:PDisplay x:Name="PDisplay" Visibility="Collapsed" />`

The entry display the library and tenor panels share, drawn in the same cell as the Example reading.
It shows an Entry or an Example, never both, so it stays collapsed until an Entry row is chosen.
The mode toggle swaps it for `PEditor`, so an Entry is read and written here as in the tenor panel.

## `<local:PEditor x:Name="PEditor" Padding="30,18,30,30" Visibility="Collapsed" />`

The entry editor the library and tenor panels share, drawn in the same cell as the display.
It stands in front only while an Entry is shown and the toggle is on the editing side.

## `<Grid x:Name="PExcerpt">`

The reading of one Example.
`PExcerpt` here stands on an Example rather than on an Entry.
What it shares with the entry panels is the read-and-edit mechanism and the `PCorpusScribe` toggle, not the object.
Each field is drawn whether or not it holds anything.
A field is stored data, so a hidden row would hide the difference between never written and written-but-unknown.
A row therefore reads its value, the unknown mark, or the unrecorded mark in the muted colour.

### `<local:PMention x:Name="PExcerptText" ...>`

The sentence is drawn by the same control the display cards use, so a word of it can be clicked.
It keeps the interface face rather than the card example face.
This row is a record field and not a card line.
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

The editable view of the selected Example.
The language stands first, because it is what the sentence beneath it is written in.
It is required by the row, so it is chosen rather than typed.
The sentence and the translation are both three-state, so an empty field says which kind of empty it is.
The translation is the sentence rendered in plain text and links nothing, unlike the Translation a Meaning carries.
`PCitation` is the single Source the Example cites, a pointer that is cleared without touching the Source itself.

## `<TextBlock x:Name="PTranscriptCount" ...>`

How many places quote the Example, kept visible while it is being edited.
Rewriting an Example here rewrites what every one of them quotes.
A user correcting a sentence must see how far the correction reaches without leaving the editor.

## `<Button x:Name="PTranscriptRemoval" ...>`

Deletes the shown Example.
It stands with discard and save because it acts on the record rather than on the browsing beside it.
It is disabled while the editor stands on an Example nothing has stored yet.

## Catalog spacing

The catalog uses the shared Theme.Catalog.Frame and Theme.Catalog.Scroll styles.
Rows keep 6 device-independent pixels on both sides, with a reserved scrollbar lane so their width stays stable.
The shared Theme.Catalog.Row preserves the same rounded shape, internal padding and row spacing across browse panels.
