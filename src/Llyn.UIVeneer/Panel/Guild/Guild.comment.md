# Guild

This folder holds the guild panel, which browses Authors, the Sources crediting them, and the Author's own page.
It splits into the panel, the Author reading `PVita`, the edit area `PAutograph` and the shared row templates.
The C# shells only call `InitializeComponent()`.
`QVita` and `QAutograph` load `PVita` and `PAutograph` and wire their named parts.

## `PGuild`

The roll row lives in `PGuildTemplate.xaml`, and the source row in `PReferenceShelf.xaml`.
Only the shelf meta style stays here, repeated from the sources panel so a source row reads alike in both.
The rows wear `Theme.Catalog.Row` directly, and the item fills mark the chosen row.

### Seams

The seams part the roll of authors, the sources crediting the chosen author, and the author itself.
The row seam runs under the ordering bar and the command row, bled past the panel margin as elsewhere.
The column seams sit in the gutters and reach the foot of the window.

### `PEchelon`

The ordering button and the search field are one control over the roll column.
The roll column takes 300, because a row carries a name and two counts and no second line.
The ordering picker is the shared `PChoiceOrder`, placed as `PGuildOrder`, and the panel anchors its menu under the whole bar.
It offers name both ways, then the two counts busiest first.
That is all an Author can be ordered by.

### `PLouver`

It holds the search field and the kind filter over the middle column, shaped as the sources panel's `PTrellis`.
`PComb` narrows the sources crediting the chosen Author by typed text.
The kind filter is the shared `PChoiceFilter`, placed as `PGuildFilter`.
The kinds are drawn in code, because the engine lists them and each carries its own word key.

### `PGuildRail`

It is the shared `PPanelRail` as the panel's action row.
Fresh opens the edit area on an Author nothing credits yet.
Save is enabled only while the edit area holds a storable draft.
The undo pair walks the chronicle of the held Author.
Print puts the Source read in the colophon on paper, and is dead while an Author is read instead.
Export is hidden, because it belongs to an Entry and no Entry is read here.

### `PRoll`

It lists every Author the workspace holds, including one nothing credits.
The uncredited row stands first while no text is typed, and lists the Sources crediting nobody.
The panel fills each row's named parts and takes each row's click.
`PRollEmpty` covers both an empty workspace and a search that matches nothing.

### `POeuvre`

The middle column lists every Source crediting the chosen Author, one row per Source.
While no Author is chosen, every Source is listed, as the sources panel lists every Entry under no Source.
The row is the sources panel's own shelf card, so a Source reads alike wherever it is browsed.
Choosing a row shows that Source in the colophon, in place of the Author reading, without leaving the tab.

### `PColophon`

It is the Source reading the sources panel shares, drawn in the same cell as the Author reading.
It shows an Author or a Source, never both, so it stays collapsed until a Source row is chosen.

### `PVita` and `PAutograph` cells

The guild driver gives each its own `QVita` and `QAutograph`.
`PVita` stands in the same cell as the colophon and is shown while an Author is read.
`PAutograph` sits over the read area in the same cell and at the same top inset.
The panel decides which is shown, so the mode toggle stays in the action row.

### `PGuildBin`

It deletes the shown Author, from the reading side or the editing side alike.
It is disabled while nothing is selected, while a Source is shown, and while the edited Author is unsaved.
Its icon is the named `PGuildBinIcon`, set by the rail driver, which also takes its click.

## `PVita`

Its title style repeats the colophon's, because an Author is read as a Source is read.
Its section heading and its tally chips come from the shared `Theme.Page` styles, which the statistics page uses too.
The co-author and citation rows come from `PGuildTemplate.xaml`, merged here.
The vita control fills each row's named parts and takes each list's clicks.

### `PVitaBody`

It reads one Author, laid out as the colophon lays out a Source.
The name stands at the head of the page.
The source count and citation count stand as chips beneath it.
The co-authors and the places citing the Author's sources are headings over their rows.
A heading with no rows is not drawn at all, as a source with no note draws none.
A never-written name reads the unnamed text in the muted colour, because the head of the page cannot be empty.

### `PFellow`

It lists the Authors credited beside the read one, each with how many Sources credit the two together.
A row leads to that Author in this same panel.

### `PVitaCitation`

It lists the places citing the Author's sources, one row per card or lone Example.
A card row leads to its Entry in the library, and an Example row to the corpus.

### `PVitaUnselected`

It is the prompt shown while no Author is chosen, centred where the reading would stand.

## `PAutograph`

Its title, field, heading and chip styles serve the edit area.
The chips are built on the speech chip, so the control merges the speech theme to reach it.
The union rows come from `PGuildTemplate.xaml`, merged here.
The edit area is the reading with bare fields in place of its values.
The name is a bare field at the head of the page, and the chips beneath it stay as read.
It carries no buttons of its own, saving through the rail and deleting through the panel bin.

### Union section

The union section folds this Author into another.
A typed name lists the Authors it matches, and one click folds.
The notice stands in its place while the Author is unsaved, because nothing can be folded into an unstored row.
`PAutographUnionList` draws the matches with the union row, and the edit area control fills each row and takes its clicks.

## `PGuildTemplate`

It holds the roll row, the co-author row, the citation row and the union row.
It is a dictionary because three controls share it.
The panel, the vita and the edit area each merge it for the rows they list.
It carries no code, and every part a row fill writes is named.

### `Guild.Roll.Card`

It shows one Author in the roll.
It holds the mark, the name, the source count, and a pellet counting the places citing those.
The row hands in the mark, because the uncredited row wears a different one.
The roll row fill marks `PRollRow` when it is the chosen Author.

### `Guild.Fellow.Card`

It shows one co-author in the vita: the name and a pellet counting the Sources crediting both.

### `Guild.Citation.Card`

It shows one place citing the Author's sources.
It shows the flag, the headword or sentence, what names the card, and its kind.

### `Guild.Union.Card`

It shows one Author the union field matched.
It shows the name and its source count, so the kept row is chosen knowingly.
Its parts share the roll row's names, so the roll row fill paints it too.

### `PRollRow`

Every row wears the shared catalog row, reached by name at runtime.
A style built on another cannot be resolved in a dictionary that stands on its own.

## Stamp

Commit: `121691f60459`
- `PAutograph.xaml` `be61330295ea5ba8`
- `PAutograph.xaml.cs` `eb524f605c7f73ce`
- `PGuild.xaml` `8a0db9acdb8c1c9d`
- `PGuild.xaml.cs` `45cccae55b8a3f38`
- `PGuildTemplate.xaml` `de72aca78db37007`
- `PVita.xaml` `892cbb9126191665`
- `PVita.xaml.cs` `59b2c434aa6269a3`
