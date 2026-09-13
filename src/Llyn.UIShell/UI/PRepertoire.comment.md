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
`PRepertoireStore` saves the Entry open in `PEditor`, and is shown only while that editor is in front.
A Situation saves from its own editor, so the rail button never stands for both.
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

The reading of one Situation.
`PVignette` here stands on a Situation rather than on an Entry.
What it shares with the entry panels is the read-and-edit mechanism and the `PRepertoireScribe` toggle, not the object.
The three fields stand as labeled rows for title, kind and description.
Each is drawn whether or not it holds anything.
A field is stored data, so a hidden row would hide the difference between never written and written-but-unknown.
A row therefore reads its value, the unknown mark, or the unrecorded mark in the muted colour.

## `<ItemsControl x:Name="POccurrence">`

The middle column: every Entry whose cards reference the selected Situation, one row per Entry.
An Entry referencing it from several cards is still one row.
While no Situation is chosen, every Entry is listed, as the tenor panel lists every Entry under no register.
A row reads the flag, the headword and the language, as a cohort row does.
Choosing a row shows that Entry in `PDisplay`, in place of the Situation reading, without leaving the tab.
The list is read-only: a reference is added or dropped on the card holding it, never here.

## `<Grid x:Name="PScenario" Visibility="Collapsed">`

The editable view of the selected Situation.
The three fields are three-state, so an empty field says which kind of empty it is.
`PCitation` is the single Source the Situation cites, a pointer that is cleared without touching the Source itself.

## `<Button x:Name="PScenarioRemoval" ...>`

Deletes the shown Situation.
It stands with discard and save because it acts on the record rather than on the browsing beside it.
It is disabled while the editor stands on a Situation nothing has stored yet.

## Catalog spacing

The catalog uses the shared Theme.Catalog.Frame and Theme.Catalog.Scroll styles.
Rows keep 6 device-independent pixels on both sides, with a reserved scrollbar lane so their width stays stable.
The shared Theme.Catalog.Row preserves the same rounded shape, internal padding and row spacing across browse panels.
