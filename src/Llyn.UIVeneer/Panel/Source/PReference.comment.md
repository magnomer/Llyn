# PReference.xaml

## `<UserControl.Resources>`

The row and card templates the panel draws with live in `PReferenceShelf.xaml` and `PReferenceFootnote.xaml`.
Only the styles built on another style stay here, because a dictionary standing on its own cannot resolve one.
The rows wear `Theme.Catalog.Row` directly, and the panel marks the chosen one.

## `<Rectangle ... Style="{StaticResource Theme.Panel.SeamRow}" />`

The seams that part the source catalog, the entries citing the chosen source, and the source itself.
The row seam runs under the ordering bar and the command row.
It is bled past the panel margin so it meets the navigation's own edge.
The column seam sits in the middle of the gutter and reaches the foot of the window.
Neither seam encloses anything, which is the whole rule: a line marks a division, a box would claim an object.

## `<Border x:Name="PGrade" ... Style="{StaticResource Theme.Search.Bar}">`

The ordering button and the search field are one control over the catalog column.
The action row over the broader column stands in the same top row, as the corpus panel arranges it.
The catalog column takes 380, because a row carries a name over an author-and-year line.
The dropdowns, icons and field events are wired by the panel, so every part carries a name.

## `<Border x:Name="PTrellis" ... Style="{StaticResource Theme.Search.Bar}">`

The search field and the language filter over the middle column, copied from the tenor panel's `PGrille`.
`PRummage` narrows the entries citing the chosen Source by typed text.
`PTrellisDropper` opens the menu of loaded languages, and `PTrellisMark` shows while any is hidden.

## `<Popup x:Name="PGradeDropdown" ...>`

The orderings the shelf may be listed in, one radio row each, carrying its choice in `Tag`.
There is no ordering by kind of material, because no Source-type discriminator is stored.
Ordering by recency is not offered, because the `source` row carries no creation or modification time.
A field standing Unspecified or Unknown still has a place in every ordering, ordered by its state.

## `<local:PRail Grid.Row="0" Grid.Column="2" ...>`

The action row of the panel.
`PReferenceFresh` opens the edit area on a Source nothing cites yet.
That is the normal case, because a Source is written down first and cited afterwards.
`PReferenceStore` saves whichever editor is in front, the Entry open in `PEditor` or the held Source.
It stands in the rail at all times, as the repertoire panel's save does.
One slot sits between save and export, and it holds whichever pair the mode asks for.
Reading shows `PReferenceVoyage`, whose `PReferenceEarlier` and `PReferenceLater` walk the window's trail of records.
Writing shows `PReferenceChronicle`, whose `PReferenceBackward` and `PReferenceForward` walk the editor's chronicle.
`PReferenceChronicle` starts collapsed, because the panel opens reading.
Each carries no label, only the arrow and a tooltip, and is lit only while a step is there.
`PReferencePortrait` exports the Entry shown, and `PReferencePress` prints what is read.
Every button is named, and the panel sets its icon, command and click from code.

## `<ItemsControl x:Name="PShelf" ...>`

Every Source the workspace holds, including one nothing cites.
The row itself is drawn by a template the merged dictionary holds.
The panel fills each row's named parts and takes each row's click.
`PShelfEmpty` covers both an empty workspace and a search that matches nothing.

## `<local:PDisplay x:Name="PDisplay" Visibility="Collapsed" />`

The entry display the library and tenor panels share, drawn in the same cell as the Source reading.
It shows an Entry or a Source, never both, so it stays collapsed until an Entry row is chosen.
The mode toggle swaps it for `PEditor`, so an Entry is read and written here as in the tenor panel.

## `<local:PEditor x:Name="PEditor" Padding="30,18,30,30" Visibility="Collapsed" />`

The entry editor the library and tenor panels share, drawn in the same cell as the display.
It stands in front only while an Entry is shown and the toggle is on the editing side.

## `<local:PColophon x:Name="PColophon" />`

The reading of one Source, a control of its own and described in `PColophon.comment.md`.
It stands in the same cell as the Entry-shaped `PDisplay` and is shown while a Source is read.
The authors panel reads a Source through the same control, so the two panels cannot drift apart.
The mode toggle is not in here.
It sits in the action row as it does in every built panel.

## `<ItemsControl x:Name="PFootnote" ...>`

The middle column: every Entry whose cards quote an Example citing the selected Source, one row per Entry.
Only Entries are listed, so an Example citing the Source is reached through the Entry quoting it.
While no Source is chosen, every Entry is listed, as the tenor panel lists every Entry under no register.
The row is drawn by a template the merged dictionary holds, and the panel fills it and takes its click.
A Situation is not listed, because a Situation is written rather than quoted and cites no Source.
Choosing a row shows that Entry in `PDisplay`, in place of the Source reading, without leaving the tab.
The list is read-only, because a citation is set or cleared where it is held.

## `<local:PImprint x:Name="PImprint" Margin="21,18,0,0" Visibility="Collapsed" />`

The edit area, a control of its own and described in `PImprint.comment.md`.
It sits over the read area in the same cell and at the same inset.
One Source is either being read or being written.
The panel decides which is shown, so the mode toggle stays in the action row.

## `<Button x:Name="PReferenceBin" ...>`

Deletes the shown Source, from the reading side or the editing side alike.
It stands in the corner of the surface as the repertoire panel's bin does, apart from the browsing beside it.
It is disabled while nothing is selected and while an Entry is shown.
It is disabled too while the edit area stands on a Source nothing has stored yet.
Its icon is the named `PReferenceBinIcon`, set by the panel.

## Catalog spacing

The catalog uses the shared Theme.Catalog.Frame and Theme.Catalog.Scroll styles.
Rows keep 6 device-independent pixels on both sides, with a reserved scrollbar lane so their width stays stable.
The shared Theme.Catalog.Row preserves the same rounded shape, internal padding and row spacing across browse panels.
