# Repertoire

The repertoire panel lists situations, the entries referencing the chosen one, and the situation itself.
It stands apart from the tenor and taxonomy panels.
Its catalog holds Situations, which are written here rather than only read.
Its markup carries no hook.
The Deportment driver `QRepertoire` and its helpers set icons, commands, clicks, text changes and row fills.
The rail and the two pickers are driven by their own shared drivers, which the panel wires.

## `PRepertoire`

The shell behind `PRepertoire.xaml`, placed by the main window.
Its constructor only calls `InitializeComponent()`.

## Seams

The seams part the situation catalog, the entries referencing the chosen situation, and the situation itself.
The row seam runs under the ordering bar and the command row.
It is bled past the panel margin so it meets the navigation's own edge.
The two column seams sit in the middle of the gutters and reach the foot of the window.
Neither seam encloses anything, because a line marks a division and a box would claim an object.

## `PTier` and `PMesh`

`PTier` joins the ordering button and the search field into one control over the catalog column.
The ordering picker is the shared `PChoiceOrder`, placed as `PRepertoireOrder`.
The panel anchors the picker's menu under the whole bar.
`PMesh` holds the search field and language filter over the middle column, copied from the tenor panel's `PGrille`.
`PSortie` narrows the entries referencing the chosen Situation by typed text.
The language filter is the shared `PChoiceFilter`, placed as `PRepertoireFilter`.

## `PRepertoireRail`

The panel's action row, the shared `PPanelRail`.
While no Situation is held, Fresh opens the editor on a Situation nothing references yet.
While one is held, Fresh opens a new Entry that already references it.
This panel is the only place such a Situation can arise.
Elsewhere one is written from the card that carries it.
Save stores whichever editor is in front, the Entry in `PEditor` or the Situation in `PScenario`.
It is enabled only while one of them is.
Reading walks the window's trail of records, and writing walks the chronicle of the editor in front.
Export takes the Entry shown, and print takes what is read.

## `PAtlas`

The catalog of every Situation the workspace holds.
A row reads its title beside its kind, with the number of places referencing it at the far end.
That number is shown here because it decides which delete the panel offers.
The catalog uses the shared `Theme.Catalog.Frame`, `Theme.Catalog.Scroll` and `Theme.Catalog.Row` styles.
The scroll bar takes its lane only when the list overflows, and the rows then narrow.

## `POccurrence`

The middle column lists every Entry whose cards reference the selected Situation, one row per Entry.
An Entry referencing it from several cards is still one row.
While no Situation is chosen, every Entry is listed, as the tenor panel lists every Entry under no register.
A row reads the flag, the headword and the language.
Choosing a row shows that Entry in `PDisplay` in place of the Situation reading, without leaving the tab.
The list is read-only.
A reference is added or dropped on the card holding it, never here.

## `PDisplay` and `PEditor`

Both are the entry display and editor the library and tenor panels share.
They are drawn in the same cell as the Situation reading.
`PDisplay` shows an Entry or a Situation, never both, so it stays collapsed until an Entry row is chosen.
The mode toggle swaps it for `PEditor`, so an Entry is read and written here as in the tenor panel.

## `PVignette`

The reading of one Situation, laid out as `PDisplay` lays out an Entry.
It shares the read-and-edit mechanism and the rail's mode toggle with the entry panels.
The title is drawn as a headword.
The kind and the reference count stand as chips on the row beneath it.
That row is where an Entry shows its parts of speech, so a classification stands where a classification stands.
The description is a heading over a paragraph, rendered from Markdown without the note's box.
A box claims an object the reader can act on, and a description is only read.
No field is labeled, because the entry display labels nothing.
Position and dress say what a value is.
An unknown value reads the unknown mark where the value would stand.
A never-written kind or description is not drawn at all, as an entry with no note draws no note.
A never-written title reads the untitled text in the muted colour, because the head of the page cannot be empty.
The editing side still draws every slot, so the two kinds of empty are one toggle apart.
Under the description stand the pictures and then the videos, in the shapes a card draws them.
`PVignettePicture` and `PVignetteVideo` share the card's line templates, so the same picture reads the same everywhere.
Either list hides itself while the Situation has none of that kind.

## `PScenario`

The editable view of the selected Situation, laid out as `PEditor` lays out an Entry.
Every slot the reading side draws is drawn here in the same place.
A bare field stands where the text stood.
The title is written in the headword box, the kind inside its chip, and the description inside its card.
The three fields are three-state, and the placeholder says which kind of empty it is.
A never-written field asks for its value, and an unknown one reads the unknown mark until typing clears it.
The head row is fixed and the description scrolls beneath it.
The editor carries no buttons of its own.
Save is the rail's save, delete is `PRepertoireBin`, and there is no discard.
Leaving the editor asks about the draft as the library panel does.
Under the description stand `PScenarioImage` and `PScenarioVideo`, the card's row templates over the draft's rows.
A gutter column keeps room for each row's remove.
Each list hears the text changes bubbling from its rows.
So a typed location reaches the panel without a row copy.
The add buttons `PScenarioPicture` and `PScenarioFilm` are the card's and are the only buttons the editor carries.

## `PRepertoireBin`

Deletes the shown Situation, from the top right corner of the surface where the library panel's bin stands.
It is enabled while a stored Situation is shown, read or written.
It is disabled while an Entry or an unsaved Situation is shown.

## Merged dictionaries

The card display's picture and video dictionaries are merged so the reading side draws media with the card's own templates.
The picture and video row templates of the editing side are merged too.
The panel's own shapes follow, in the Vignette folder's two dictionaries.
All six dictionaries come from the Veneer.

## Stamp

Commit: `c2f335c331fe`
- `PRepertoire.xaml` `6960d537e086e69a`
- `PRepertoire.xaml.cs` `22324c28074bf190`
