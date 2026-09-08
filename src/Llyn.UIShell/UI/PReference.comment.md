# PReference.xaml

## `<Grid x:Name="PGrade" Grid.Row="0" Grid.Column="0" Margin="0,0,20,18">`

The ordering button and the search field share the catalog column, so their combined edge is the catalog's.
The action row over the broader column stands in the same top row, as the corpus panel arranges it.
The catalog column takes 420, because a row carries a name over an author-and-year line.

## `<Popup x:Name="PGradeDropdown" ...>`

The orderings the shelf may be listed in, one radio row each, carrying its choice in `Tag`.
There is no ordering by kind of material, because no Source-type discriminator is stored.
Ordering by recency is not offered, because the `source` row carries no creation or modification time.
A field standing Unspecified or Unknown still has a place in every ordering, ordered by its state.

## `<StackPanel Grid.Row="0" Grid.Column="1" ...>`

The action row of the panel.
`PReferenceFresh` opens the edit area on a Source nothing cites yet.
That is the normal case, because a Source is written down first and cited afterwards.
Export and print are mock-up controls and are not wired.

## `<ItemsControl x:Name="PShelf">`

Every Source the workspace holds, including one nothing cites.
The row shows the resolved name over its credits and year, with the citation count at the right.
`PShelfEmpty` covers both an empty workspace and a search that matches nothing.

## `<Grid x:Name="PColophon">`

The read area, this panel's own rather than the Entry-shaped `PDisplay`.
Every field is shown, including one standing Unspecified, so a reader learns what the Source does not state.
The mode toggle is not in here.
It sits in the action row as it does in every built panel.

## `<ItemsControl x:Name="PFootnote">`

The Entries and Examples citing the selected Source, each row leading to the panel that holds it.
A Situation is not listed, because a Situation is written rather than quoted and cites no Source.
The list is read-only, because a citation is set or cleared where it is held.

## `<Grid x:Name="PImprint">`

The edit area, the selected Source as the panel writes it.
Each of the five stated fields carries a `?` toggle beside it that records the value as unknown.
The three states are distinct facts, so clearing a value is not the same as recording it unknown.

## `<Grid x:Name="PAuthor" Margin="0,7,0,0">`

The ordered credits and the workspace's authors offered for crediting.
`PAuthorSwitch` stands disabled until the Source exists, because a credit is an association row on a stored id.
`PAuthorUnknown` records that the authorship is unknown, which is not the same as no credit yet.

## `<StackPanel Grid.Row="1" Margin="24,10,24,20" ...>`

The usage figure stays visible while editing, because one correction reaches every place citing the Source.
`PImprintRemoval` offers a plain delete at zero and a detach-and-delete otherwise.
