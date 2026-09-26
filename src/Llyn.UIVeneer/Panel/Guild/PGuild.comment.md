# PGuild.xaml

## `<UserControl.Resources>`

The roll row lives in `PGuildTemplate.xaml`, and the source row in `PReferenceShelf.xaml`.
Only the shelf meta style stays here, repeated from the sources panel so a source row reads alike in both.
The rows wear `Theme.Catalog.Row` directly, and the item fills mark the chosen row.

## `<Rectangle ... Style="{StaticResource Theme.Panel.SeamRow}" />`

The seams that part the roll of authors, the sources crediting the chosen author, and the author itself.
The row seam runs under the ordering bar and the command row, bled past the panel margin as elsewhere.
The column seams sit in the gutters and reach the foot of the window.

## `<Border x:Name="PEchelon" ... Style="{StaticResource Theme.Search.Bar}">`

The ordering button and the search field are one control over the roll column.
The roll column takes 300, because a row carries a name and two counts and no second line.
The dropdowns, icons and field events are wired by the panel, so every part carries a name.

## `<Popup x:Name="PEchelonDropdown" ...>`

The orderings the roll may be listed in, one radio row each, carrying its choice in `Tag`.
Name both ways, then the two counts busiest first, because that is all an Author can be ordered by.
The panel ties it to its dropper and anchors it under the whole bar.

## `<Border x:Name="PLouver" ... Style="{StaticResource Theme.Search.Bar}">`

The search field and the kind filter over the middle column, shaped as the sources panel's `PTrellis`.
`PComb` narrows the sources crediting the chosen Author by typed text.
`PLouverDropper` opens the menu of source kinds, and `PLouverMark` shows while any is hidden.
The kinds are drawn in code, because their words come from the sources panel's own keys.

## `<local:PRail Grid.Row="0" Grid.Column="2" ...>`

The action row of the panel.
`PGuildFresh` opens the edit area on an Author nothing credits yet.
`PGuildStore` saves the held Author, and stands in the rail at all times.
One slot sits between save and print, and it holds whichever pair the mode asks for.
Reading shows `PGuildVoyage`, whose `PGuildEarlier` and `PGuildLater` walk the window's trail of records.
Writing shows `PGuildChronicle`, whose `PGuildBackward` and `PGuildForward` walk the chronicle of the held Author.
`PGuildChronicle` starts collapsed, because the panel opens reading.
Each carries no label, only the arrow and a tooltip, and is lit only while a step is there.
`PGuildPress` puts the Source read in the colophon on paper, and is dead while an Author is read instead.
Export is not offered, because it belongs to an Entry and no Entry is read here.
Every button is named, and the panel sets its icon, command and click from code.

## `<ItemsControl x:Name="PRoll" ...>`

Every Author the workspace holds, including one nothing credits.
The uncredited row stands first while no text is typed, and lists the Sources crediting nobody.
The panel fills each row's named parts and takes each row's click.
`PRollEmpty` covers both an empty workspace and a search that matches nothing.

## `<ItemsControl x:Name="POeuvre" ...>`

The middle column: every Source crediting the chosen Author, one row per Source.
While no Author is chosen, every Source is listed, as the sources panel lists every Entry under no Source.
The row is the sources panel's own shelf card, so a Source reads alike wherever it is browsed.
Choosing a row shows that Source in the colophon, in place of the Author reading, without leaving the tab.

## `<local:PColophon x:Name="PColophon" Visibility="Collapsed" />`

The Source reading the sources panel shares, drawn in the same cell as the Author reading.
It shows an Author or a Source, never both, so it stays collapsed until a Source row is chosen.

## `<local:PVita x:Name="PVita" />`

The reading of one Author, a control of its own and described in `PVita.comment.md`.
It stands in the same cell as the colophon and is shown while an Author is read.

## `<local:PAutograph x:Name="PAutograph" Margin="21,18,0,0" Visibility="Collapsed" />`

The edit area, a control of its own and described in `PAutograph.comment.md`.
It sits over the read area in the same cell and at the same inset.
The panel decides which is shown, so the mode toggle stays in the action row.

## `<Button x:Name="PGuildBin" ...>`

Deletes the shown Author, from the reading side or the editing side alike.
It is disabled while nothing is selected, while a Source is shown, and while the edited Author is unsaved.
Its icon is the named `PGuildBinIcon`, set by the panel.
