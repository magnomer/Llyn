# Core

This folder holds the shared editor that every panel editing an entry places, with its dropdowns and input panel.
It stands apart from the other editor folders because it is the form's shape.
The field templates beside it supply the parts.
The markup declares no event handlers.
The Deportment driver `QEditor` and its subordinate drivers supply event and command wiring.
Each C# shell only calls `InitializeComponent()`.

## `PEditor`

The form's shape stays here and the rules its parts are drawn by stand beside it.
Fields, the closing card, the dropdown shells and the other templates are each a dictionary of their own.
The templates cover sentence, picture, video, meaning, collocation and language.
A reader of the file sees where things sit, not how each is painted.
Auto row sizes stay unrounded and each section's contents round like `PDisplay`.
The popup ground is the same floating surface the other pickers stand on, so a theme reaches it.

## `PEditorCard`

`Editor.Card.Add` is the button that ends a meaning list and a collocation list, so the ending is written once.
A dashed outline says the card is not there yet and the plus says it can be.
Only the wording and the list it adds to differ, and both stay in the markup.
The plus is the named part `PSurfaceIcon`, which a look row fills with its icon.
Each add button is named, so the editor subscribes its click.

## `PEditorField`

`Editor.Field.Ghost` is the unseen twin that measures the headword, standing in the same `QGauge` as the box.
It alone is measured, so the cell closes on the text rather than on the box.
A box measures wider than its text, by its caret and its scroll host.
An empty box is measured by the word it asks for, so the row never closes on nothing.
The editor writes that text, so the style holds only how the twin stands.

## `PEditorPopup`

It holds the shells of the editor's dropdowns, apart from the layout that opens them.
A row that lights on hover and fills when picked is one behaviour, so it is declared once.
`Editor.Popup.Stretch` gives a row the width of the picker, not of its own text.
`Editor.Popup.Frame` is the surface of a caret's dropdown with the gutter its shadow needs.
The editor sets its width to the caret it was opened from each time it places the dropdown.
`Editor.Popup.List` is read and clicked, never tabbed into or scrolled sideways.
The caret keeps the focus while it is open.
`Editor.Popup.Row` is the translation row, with the height and corner a reader of that panel expects.
Its hover and pick fills are look rows on the part `PSurface`, not triggers.
`Editor.Popup.Caret` is cut down from that row for the short caret of the Situation and slate lists.
It inherits the look rows, since its surface part has the same name.

## `PEditorDropdown`

It holds the five dropdowns a caret or a row button opens.
So `PEditor` holds nothing that floats over the form.
The editor places it once, and the drivers find each popup by its fixed name through the logical tree.
It merges the shells of `PEditorPopup.xaml`, and its row templates come from the editor around it.
`QNotation`, `QClip`, `QProspect`, `QProffer` and `QSlate` open, fill and place the popups.

### `PNotation`

The pronunciation menu has no placement target of its own, because the row button that opens it becomes the target.
Closing it, by a pick or a click elsewhere, calls the search off.

### `PClip`

The audio menu is declared once for the same reason and opened the same way.

## `PInput`

The input panel is the shared editor standing on no entry, a form that creates one.
The browse panels mount the same control over an entry they loaded.
`QInput` drives the editor inside.
The padding is this panel's own, since the editor fills the whole tab here.

## Inline notes

### PEditor.xaml: `PEditorCommand`

Undo and redo stand first, lit only while the engine has a step to walk.
They carry no label.
The keys of the window are the main road, and the buttons only show it exists.
Discard and store answer one unsaved edit, so they share one group and differ by their fill alone.
The group is the command bar the browsing panels carry, so the two surfaces read as one language.
The buttons stand at 38 pixels beside the headword they act on rather than over it.
Each carries the icon its panel twin carries, and the editor sets it, since an icon lookup is code.
Both take the width of their label, so a longer language does not clip.

### PEditor.xaml: `PMarkerField`

It is editable, not a chooser, and the switch beside it opens the language's presets.
So a part of speech no pack declares can still be written down.
Each one written becomes a chip in the list before the field.

### PEditor.xaml: `PUnitDropper`

The lexical unit leads the part-of-speech row, since it says what kind of piece carries the roles after it.
It is a chooser, because every language offers only three units.
Its dropdown and the category dropdown show at once with no fade, so a pick never looks slow to answer.

### PEditor.xaml: `PFreedomDropper`

The freedom follows the unit, so the row reads as unit, freedom, then the parts of speech.
It is a chooser of two, drawn and timed as the unit chooser is.

### PEditor.xaml: `PEditorSound`

The pronunciation rows are laid out as the reading view lays them out.
The reflex block leads, shown only for a language that has one.
The primary pronunciation follows with the volume tray beside it, then the accent list, then one transcription row per scheme.
Transcription rows show only for a language whose pack declares schemes.
Every row wears the same play, lookup and download buttons after its brackets.
Every row ends with the plus and minus pair, shown on hover.
The plus adds a row beneath and the minus drops it.
The editor binds the row commands on this panel.
So both the primary row and the accent list reach the editor that owns the draft.
The shared styles keep switching mode from moving either of them.
The rows start at the headword's own margin, because an indent read as a different position.

### PEditor.xaml: `PEditorScript`

It is the character-style box the reading view shows, folded under its head here.
The pictures are reference rather than manually editable fields, and rebuilding can replace them independently of the stored opening.
Each entry keeps its own open state in the database.

### PEditor.xaml: `PEditorFanqie`

It is the rime-book placement box the reading view shows, folded under its head here.
Each entry keeps its own open state, as the script box does.

### PEditor.xaml: `PEtymology`

The etymology card is the veneer's own control, which its folder's markup draws.
It stays collapsed until its tab is chosen, as the meaning, collocation and note cards do.
The deportment finds it by this name, both for the tab and for its driver.

### PEditor.xaml: `PPronunciation`

The primary pronunciation places its variety before the bracketed field and its buttons after it.
The brackets are named so the editor can turn them into slashes for a phonemic respelling.
The chip is empty for a pronunciation without a variety, so the brackets then open at the margin.
Play shows only while the row has a recording.
Lookup and download open the editor's menus under the button pressed, for this row.
The plus and minus carry no row, which the handlers read as the primary.

### PEditor.xaml: `PContour`

It draws the tone contour beneath the chip when the chosen language is tonal.
The editor copies the pronunciation field into its IPA on every keystroke, so the picture follows the typing.
The editor sets the tonal flag whenever the language changes.

### PEditor.xaml: `PReflexBlock`

It sits at the head of the reading stack.
It stays collapsed until a draft with reflex rules or rows is rendered.
`PReflexTable` is sized by hand while a fetch runs.
`PReflexLoading` is the fetching line, shown only while a fetch runs.
`PReflexHinge` folds the rows, stays hidden while no row is folded, and stores its state per entry.
`PAnchor` is one popup for every row, targeted at the label that opened it.
`PAnchorEmpty` is the notice shown when the character has no placement.
The reflex list sits in a `QReflexFrame`, which reports only its fields and buttons to accessibility.

### PEditor.xaml: `PTranscription`

It stays collapsed until a draft in a language with schemes is rendered, so English shows no such line.

### PEditor.xaml: `PGlyph`

It holds the traditional form of a Han-script headword.
It stays collapsed until a draft in a language with a glyph section is rendered.
Its tag says whether the section declares sources, and the template hides the lookup button when not.

### PEditor.xaml: `Theme.Pronunciation.Cell`

The field and its unseen twin are stacked on the same cell.
The twin decides the cell's width, so the brackets close on the text instead of on a fixed box.
The editor writes the twin's text from the field, or the field's hint while it is empty.

### PEditor.xaml: `PPlayback`

The volume tray is the same bare tray the reading view draws, from the same theme style.
It appears while any row has a recording and goes when none does.
The volume it shows is the workspace's own.
So a level set here is the level the reading view opens at.

### PEditor.xaml: `PReflexRenewal`

The fetch-again button is pinned to the top right of the sound rows.
It sits outside the reflex block so the rows never push it down.
The editor shows it with the block and sets its `Pending` cue to turn the icon while a fetch runs.

## Stamp

Commit: `c6e60141a172`
- `PEditor.xaml` `6faba677bb261c18`
- `PEditor.xaml.cs` `29ffbc2c15f8f19d`
- `PEditorCard.xaml` `ac6c941e70bc9e7e`
- `PEditorDropdown.xaml` `1fb3025a80aa0ef9`
- `PEditorDropdown.xaml.cs` `6557c92af97153fd`
- `PEditorField.xaml` `f3c21eda21dc7150`
- `PEditorPopup.xaml` `8f407343120ec80d`
- `PInput.xaml` `ad6146b3ac256c44`
- `PInput.xaml.cs` `b00205fc017bccbb`
