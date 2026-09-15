# PRepertoire.xaml

## `<Rectangle ... Style="{StaticResource Theme.Panel.SeamRow}" />`

The seams that part the situation catalog, the entries referencing the chosen situation, and the situation itself.
The row seam runs under the ordering bar and the command row.
It is bled past the panel margin so it meets the navigation's own edge.
The column seam sits in the middle of the gutter and reaches the foot of the window.
Neither seam encloses anything, which is the whole rule: a line marks a division, a box would claim an object.

## `<Border x:Name="PTier" ... Style="{StaticResource Theme.Search.Bar}">`

The ordering button and the search field are one control over the catalog column.
The action row over the broader column stands in the same top row, as the tenor panel arranges it.

## `<Border x:Name="PMesh" ... Style="{StaticResource Theme.Search.Bar}">`

The search field and the language filter over the middle column, copied from the tenor panel's `PGrille`.
`PSortie` narrows the entries referencing the chosen Situation by typed text.
`PMeshDropper` opens the menu of loaded languages, and `PMeshMark` shows while any is hidden.

## `<StackPanel Grid.Row="0" Grid.Column="1" ...>`

The action row of the panel.
`PRepertoireFresh` opens the editor on a Situation nothing references yet.
This panel is the only place such a Situation can arise.
Elsewhere one is written from the card that carries it.
`PRepertoireStore` saves whichever editor is in front, the Entry in `PEditor` or the Situation in `PScenario`.
It is shown only while one of them is, as the library panel shows its save beside its new.
Export and print are mock-up controls and are not wired.

## `<ItemsControl x:Name="PAtlas">`

The catalog of every Situation the workspace holds.
A row reads its title over its kind, with the number of places referencing it at the far end.
That number is shown here and not only in the display, because it decides which delete the panel offers.

## `<local:PDisplay x:Name="PDisplay" Visibility="Collapsed" />`

The entry display the library and tenor panels share, drawn in the same cell as the Situation reading.
It shows an Entry or a Situation, never both, so it stays collapsed until an Entry row is chosen.
The mode toggle swaps it for `PEditor`, so an Entry is read and written here as in the tenor panel.

## `<local:PEditor x:Name="PEditor" Padding="30,18,30,30" Visibility="Collapsed" />`

The entry editor the library and tenor panels share, drawn in the same cell as the display.
It stands in front only while an Entry is shown and the toggle is on the editing side.

## `<Grid x:Name="PVignette">`

The reading of one Situation, laid out as `PDisplay` lays out an Entry.
`PVignette` here stands on a Situation rather than on an Entry.
What it shares with the entry panels is the read-and-edit mechanism, the `PRepertoireScribe` toggle, and the shape of the page.
The title is drawn as a headword.
The kind and the reference count stand as chips on the row beneath it.
That row is where an Entry shows its parts of speech, so a classification stands where a classification stands.
The description is a heading over a paragraph.
It is rendered from Markdown as the entry note is but without the note's box.
A box claims an object the reader can act on, and a description is only read.
No field is labeled, because the entry display labels nothing: position and dress say what a value is.
A field that holds nothing follows the entry display's rule rather than a rule of its own.
An unknown value reads the unknown mark where the value would stand.
A never-written kind or description is not drawn at all, as an entry with no note draws no note.
A never-written title reads the untitled text in the muted colour, because the head of the page cannot be empty.
The editing side still draws every slot, so the two kinds of empty are one toggle apart rather than lost.
Under the description stand the pictures and then the videos, in the shapes a card draws them.
`PVignettePicture` and `PVignetteVideo` share the card's line templates and converters, so the same picture reads the same everywhere.
Either list hides itself while the Situation has none of that kind, as a card's does.

## `<ResourceDictionary.MergedDictionaries>`

The card display's picture and video dictionaries, merged so the reading side draws media with the card's own templates.
The panel's own shapes follow, the reading side in [PRepertoireVignette.xaml](PRepertoireVignette.comment.md) and the editing side in [PRepertoireScenario.xaml](PRepertoireScenario.comment.md).
The two converters stay, because both sides of the panel read media through them.
The editing side's row templates are merged from code instead, because they carry handlers this panel answers.

## `<ItemsControl x:Name="POccurrence">`

The middle column: every Entry whose cards reference the selected Situation, one row per Entry.
An Entry referencing it from several cards is still one row.
While no Situation is chosen, every Entry is listed, as the tenor panel lists every Entry under no register.
A row reads the flag, the headword and the language, as a cohort row does.
Choosing a row shows that Entry in `PDisplay`, in place of the Situation reading, without leaving the tab.
The list is read-only: a reference is added or dropped on the card holding it, never here.

## `<Grid x:Name="PScenario" ... Visibility="Collapsed">`

The editable view of the selected Situation, laid out as `PEditor` lays out an Entry.
Every slot the reading side draws is drawn here in the same place.
A bare field stands where the text stood.
The title is written in the headword box, the kind inside its chip, and the description inside its card.
The three fields are three-state, and an empty field says which kind of empty it is through its placeholder.
A never-written field asks for its value, and an unknown one reads the unknown mark until typing clears it.
The head row is fixed and the description scrolls beneath it, as the editor's head row stands over its cards.
The editor carries no buttons of its own.
Save is the rail's `PRepertoireStore`, delete is `PRepertoireBin`, and there is no discard.
That is how the library panel arranges an Entry.
Leaving the editor asks about the draft as it does there.
Under the description stand `PScenarioImage` and `PScenarioVideo`, the card's own row templates over the draft's rows.
They sit where the reading side draws the same media, and a gutter column keeps room for each row's remove.
The two add buttons after them are the card's, in the same icon group, the only buttons the editor carries.

## `<Button x:Name="PRepertoireBin" ...>`

Deletes the shown Situation, from the top right corner of the surface where the library panel's bin stands.
It is enabled while a stored Situation is shown, read or written.
It is disabled while an Entry or an unsaved Situation is.

## Catalog spacing

The catalog uses the shared Theme.Catalog.Frame and Theme.Catalog.Scroll styles.
Rows keep 6 device-independent pixels on both sides, with a reserved scrollbar lane so their width stays stable.
The shared Theme.Catalog.Row preserves the same rounded shape, internal padding and row spacing across browse panels.
