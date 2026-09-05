# PCorpus.xaml

## `<Grid x:Name="PRank" Grid.Row="0" Grid.Column="0" Margin="0,0,20,18">`

The ordering button and the search field share the catalog column, so their combined edge is the catalog's.
The action row over the broader column stands in the same top row, as the repertoire panel arranges it.
The catalog column is wider than the entry panels take, because it holds sentences rather than headwords.

## `<Popup x:Name="PRankMenu" ...>`

The orderings the catalog may be listed in, one radio row each, carrying its choice in `Tag`.
Ordering by recency is not offered, because the `example` row carries no creation or modification time.
An Example whose text is unwritten still has a place in every ordering, ordered by its mark rather than dropped.

## `<StackPanel Grid.Row="0" Grid.Column="1" ...>`

The action row of the panel.
`PCorpusFresh` opens the editor on an Example nothing quotes yet.
This panel is the only place such an Example can arise, because elsewhere one is written from the card that quotes it.
Export and print are mock-up controls and are not wired.

## `<ItemsControl x:Name="PAnthology">`

The catalog of every Example the workspace holds, including one nothing quotes.
A row reads its sentence over its language and cited Source, with its usage count at the far end.
That count is shown here and not only in the display, because it decides which delete the panel offers.
The translation is not shown here: it is text of the sentence itself and says nothing about where the sentence is used.

## `<Grid x:Name="PExcerpt">`

The reading of one Example, and the sides quoting it beneath.
`PExcerpt` here stands on an Example rather than on an Entry.
What it shares with the entry panels is the read-and-edit mechanism and the `PCorpusScribe` toggle, not the object.
Each field is drawn whether or not it holds anything.
A field is stored data, so a hidden row would hide the difference between never written and written-but-unreadable.
A row therefore reads its value, the unreadable mark, or the unrecorded mark in the muted colour.

## `<ItemsControl x:Name="PQuotation">`

Everything quoting the selected Example, one row per quoting side.
A row names the Entry, Meaning or Collocation and the Entry it belongs to, so the relationship is never flattened.
Choosing a row leaves for that Entry in the library panel.
The list is read-only: a reference is added or dropped on the card holding it, never here.
An Example nothing quotes shows the empty state rather than hiding the region.

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

## `<Grid Margin="10,6,10,4">` inside `PCitationMenu`

Writing a Source name and adding it, for a Source the shelf does not carry yet.
The Example is pointed at the new Source in the same act, because that is why it was created here.
Creating a Source from here creates it with the typed name as its title, as the card row does.

