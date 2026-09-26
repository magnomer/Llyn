# PVita.xaml

## `<UserControl.Resources>`

The vita styles repeat the colophon's, because an Author is read as a Source is read.
The co-author and citation rows come from `PGuildTemplate.xaml`, merged here.
The markup carries no class, and the vita control loads it and fills its named parts.

## `<StackPanel x:Name="PVitaBody" ...>`

The reading of one Author, laid out as the colophon lays out a Source.
The name stands at the head of the page.
The source count and the citation count stand as chips on the row beneath it.
The co-authors and the places citing the Author's sources are headings over their rows.
A heading with no rows is not drawn at all, as a source with no note draws none.
A never-written name reads the unnamed text in the muted colour, because the head of the page cannot be empty.

## `<ItemsControl x:Name="PFellow" ...>`

The Authors credited beside the read one, each with how many Sources credit the two together.
A row leads to that Author in this same panel.

## `<ItemsControl x:Name="PVitaCitation" ...>`

The places citing the Author's sources, one row per card or lone Example.
A card row leads to its Entry in the library, and an Example row to the corpus.

The vita control fills each row's named parts and takes each list's clicks.

## `<TextBlock x:Name="PVitaUnselected" ...>`

The prompt shown while no Author is chosen, centred where the reading would stand.
