# PGuild.xaml

## `<UserControl.Resources>`

The row templates the panel draws with live in `PGuildTemplate.xaml`, and the source row in `PReferenceShelf.xaml`.
Only the styles built on another style stay here, because a dictionary standing on its own cannot resolve one.
The shelf row style is repeated from the sources panel, so a source row reads alike in both.
The vita styles repeat the colophon's, because an Author is read as a Source is read.

## `<Rectangle ... Style="{StaticResource Theme.Panel.SeamRow}" />`

The seams that part the roll of authors, the sources crediting the chosen author, and the author itself.
The row seam runs under the ordering bar and the command row, bled past the panel margin as elsewhere.
The column seams sit in the gutters and reach the foot of the window.

## `<Border x:Name="PEchelon" ... Style="{StaticResource Theme.Search.Bar}">`

The ordering button and the search field are one control over the roll column.
The roll column takes 300, because a row carries a name and two counts and no second line.

## `<Popup x:Name="PEchelonDropdown" ...>`

The orderings the roll may be listed in, one radio row each, carrying its choice in `Tag`.
Name both ways, then the two counts busiest first, because that is all an Author can be ordered by.

## `<Border x:Name="PLouver" ... Style="{StaticResource Theme.Search.Bar}">`

The search field and the kind filter over the middle column, shaped as the sources panel's `PTrellis`.
`PComb` narrows the sources crediting the chosen Author by typed text.
`PLouverDropper` opens the menu of source kinds, and `PLouverMark` shows while any is hidden.
The kinds are drawn in code, because their words come from the sources panel's own keys.

## `<local:PRail Grid.Row="0" Grid.Column="2" ...>`

The action row of the panel.
`PGuildFresh` opens the edit area on an Author nothing credits yet.
`PGuildStore` saves the held Author, and stands in the rail at all times.
One slot sits between save and export, and it holds whichever pair the mode asks for.
Reading shows `PGuildEarlier` and `PGuildLater`, which walk the window's trail of records.
Writing shows `PGuildBackward` and `PGuildForward`, which walk the chronicle of the held Author.
Each carries no label, only the arrow and a tooltip, and is lit only while a step is there.
Print puts the Source read in the colophon on paper, and is dead while an Author is read instead.
Export is not offered, because it belongs to an Entry and no Entry is read here.

## `<ItemsControl x:Name="PRoll" Button.Click="PRollHandle" ...>`

Every Author the workspace holds, including one nothing credits.
The uncredited row stands first while no text is typed, and lists the Sources crediting nobody.
The click of every row is taken here, because a template in a dictionary can name no handler.
`PRollEmpty` covers both an empty workspace and a search that matches nothing.

## `<ItemsControl x:Name="POeuvre" Button.Click="POeuvreHandle" ...>`

The middle column: every Source crediting the chosen Author, one row per Source.
While no Author is chosen, every Source is listed, as the sources panel lists every Entry under no Source.
The row is the sources panel's own shelf card, so a Source reads alike wherever it is browsed.
Choosing a row shows that Source in the colophon, in place of the Author reading, without leaving the tab.

## `<local:PColophon x:Name="PColophon" Visibility="Collapsed" />`

The Source reading the sources panel shares, drawn in the same cell as the Author reading.
It shows an Author or a Source, never both, so it stays collapsed until a Source row is chosen.

## `<Grid x:Name="PVita">`

The reading of one Author, laid out as the colophon lays out a Source.
The name stands at the head of the page.
The source count and the citation count stand as chips on the row beneath it.
The co-authors and the places citing the Author's sources are headings over their rows.
A heading with no rows is not drawn at all, as a source with no note draws none.
A never-written name reads the unnamed text in the muted colour, because the head of the page cannot be empty.

## `<ItemsControl x:Name="PFellow" Button.Click="PFellowHandle" ...>`

The Authors credited beside the read one, each with how many Sources credit the two together.
A row leads to that Author in this same panel.

## `<ItemsControl x:Name="PVitaCitation" Button.Click="PVitaCitationHandle" ...>`

The places citing the Author's sources, one row per card or lone Example.
A card row leads to its Entry in the library, and an Example row to the corpus.

## `<Grid x:Name="PAutograph" ...>`

The edit area, the reading with bare fields in place of its values.
The name is a bare field at the head of the page, and the chips beneath it stay as read.
The union section folds this Author into another: a typed name lists the Authors it matches, and one click folds.
The notice stands in its place while the Author is unsaved, because nothing can be folded into an unstored row.
It carries no buttons of its own, saving through the rail and deleting through the panel bin.

## `<Button x:Name="PGuildBin" ...>`

Deletes the shown Author, from the reading side or the editing side alike.
It is disabled while nothing is selected, while a Source is shown, and while the edited Author is unsaved.
