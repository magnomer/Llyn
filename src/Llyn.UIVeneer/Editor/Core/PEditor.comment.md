# PEditor.xaml
Hash: `57a6e3de77dc4eea`

## `UserControl`

The shared editor as markup alone.
Its `x:Class` shell only builds it, and the Deportment driver `QEditor` drives every part.
`QEditor` and its subordinate drivers supply event and command wiring.
This markup declares no event handlers.

## `<Grid UseLayoutRounding="False">`

Keep Auto row sizes unrounded and round each section's contents like PDisplay.

## `<ResourceDictionary.MergedDictionaries>`

The form's shape stays here and the rules its parts are drawn by stand beside it.
Fields, the card the lists end with, and the dropdown shells are each a dictionary of their own.
So are the sentence, picture, video, meaning, collocation and language templates, and the rest the form uses.
A reader of this file sees where things sit, not how each of them is painted.

## `<Border Margin="10" Style="{StaticResource Theme.Popup.Surface}">`

The same floating ground the other pickers stand on.
It is not a white box of its own that no theme reaches.

## `<Border x:Name="PEditorCommand"`

Undo and redo stand first, lit only while the engine has a step to walk.
A divider parts them from the pair that ends the edit.
They carry no label, only the arrow and a tooltip.
The keys of the window are the main road, and the buttons only show it exists.
Discard and store answer one unsaved edit, so they share one group and are separated by their fill alone.
The group is the command bar the browsing panels carry, so the two surfaces read as one language.
A borderless tint is what this window gives a language pill and an open tab, not a button.
They stand at 38 pixels inside the group, beside the headword they act on rather than over it.
Each carries the icon its panel twin carries, a plus for a fresh form and a disk for a store.
The editor sets those icons, since an icon lookup is code.
A bin of one size and a diskette of another, beside labels of two lengths, left the pair looking lopsided.
Both take the width of their label, so a longer language does not clip.

## `<TextBox x:Name="PMarkerField" Style="{StaticResource Theme.Marker.Text}" />`

Editable, not a chooser.
The switch beside it opens the language's presets, and the box takes anything typed.
So a part of speech no pack declares can still be written down.
Each one written becomes a chip in the list before the field.

## `<ToggleButton x:Name="PUnitDropper" Style="{StaticResource Theme.Unit.Dropper}">`

The lexical unit leads the part-of-speech row, since it says what kind of piece carries the roles after it.
It is a chooser, not a field, because a language offers only two or three units.
Its dropdown and the category dropdown show at once, with no fade, so a pick never looks slow to answer.

## `<StackPanel x:Name="PEditorSound"`

The pronunciation rows, laid out as the reading view lays them out.
The reflex block leads, shown only for a language that has one.
The primary pronunciation follows on a row of its own with the volume tray beside it.
Every further pronunciation stands on a row of its own beneath, in the accent list.
The transcription rows follow beneath those, one per scheme, only for a language whose pack declares schemes.
Every row, the primary one included, wears the same play, lookup and download buttons after its brackets.
Every row ends with the plus and minus pair the example rows carry, shown on hover.
The plus adds a row beneath and the minus drops the row.
The editor binds the row commands on this panel, above the primary row and the accent list.
So both reach the editor that owns the draft and the menus.
Pronunciation and volume tray carry the theme's shared styles, so switching mode moves neither of them.
The rows start at the headword's own margin, because an indent here read as a different position.

## `<local:QScript x:Name="PEditorScript" Margin="0,14,0,0" QScriptFolded="True" />`

The character styles of the entry, the same box the reading view shows, folded under its head here.
Pictures are reference rather than manually editable fields.
Rebuilding can replace them independently of the stored box opening.
Each entry keeps its own open state, stored in the database.

## `<local:QFanqie x:Name="PEditorFanqie" Margin="0,14,0,0" QFanqieFolded="True" />`

The rime-book placements of the entry, the same box the reading view shows, folded under its head here.
Each entry keeps its own open state, as the script box does.

## `<Border x:Name="PPronunciation" Style="{StaticResource Theme.Pronunciation.Surface}">`

The primary pronunciation places its variety before the bracketed field and its buttons after it.
The brackets are named so the editor can turn them into slashes for a phonemic respelling.
The chip is empty for a pronunciation without a variety, so the brackets then open at the margin.
The buttons are the accent tool style, so the primary row reads as the further rows do.
Play shows only while the row has a recording.
Lookup and download open the editor's menus under the button pressed, for this row.
The plus and minus carry no row, which the handlers read as the primary.

## `<local:PContour x:Name="PContour" Style="{StaticResource Theme.Contour.Box}" />`

The tone contour of the primary pronunciation, drawn beneath the chip when the chosen language is tonal.
The editor copies the pronunciation field into its IPA on every keystroke, so the picture follows the typing.
The editor sets the tonal flag whenever the language changes.

## `<Grid x:Name="PReflexBlock" Visibility="Collapsed">`

The reflex block at the head of the reading stack.
It holds the rows, the fetching line and the hinge.
It stays collapsed until a draft in a language with reflex rules or reflex rows is rendered.
`PReflexTable` is the stack of rows, sized by hand while a fetch runs.
`PReflexLoading` is the fetching line under the rows, shown only while a fetch runs.
`PReflexHinge` shows or hides the folded rows, and stays hidden while no row is folded.
Its state is stored per entry, so each entry opens as it was left.
`PAnchor` is the anchor dropdown, one popup for every row, targeted at the label that opened it.
`PAnchorList` holds its tick rows and `PAnchorEmpty` the notice shown when the character has no placement.

## `<ItemsControl x:Name="PReflex" ItemTemplate="{StaticResource Theme.Reflex.Row}" />`

The reflexes, one editable row each, drawn by the shared reflex template.
It sits in a `QReflexFrame`, which reports only its fields and buttons to accessibility.

## `<ItemsControl x:Name="PAccent" ItemTemplate="{StaticResource Theme.Accent.Row}" />`

The further pronunciations, one editable row each, drawn by the shared accent template.

## `<ItemsControl x:Name="PTranscription" ItemTemplate="{StaticResource Theme.Transcription.Row}" Visibility="Collapsed" />`

The transcriptions, one editable row each, drawn by the shared transcription template.
It stays collapsed until a draft in a language with schemes is rendered, so English shows no such line.

## `<ItemsControl x:Name="PGlyph" ItemTemplate="{StaticResource Theme.Glyph.Row}" Visibility="Collapsed" />`

The glyph row, the traditional form of a Han-script headword, drawn by the glyph template.
It stays collapsed until a draft in a language with a glyph section is rendered.
Its tag says whether the section declares sources, and the template hides the lookup button when not.

## `<Grid Style="{StaticResource Theme.Pronunciation.Cell}">`

The field and the unseen twin that measures it, stacked on the same cell.
The twin decides the cell's width, so the brackets close on the text instead of on a fixed box.
The editor writes the twin's text from the field, or the field's hint while it is empty.

## `<Border x:Name="PPlayback" Style="{StaticResource Theme.Volume.Tray}">`

The volume tray holds the level every row's recording is played at.
It is the same bare tray the reading view draws, from the same theme style.
It appears while any row has a recording and goes when none does.
The volume is offered only while there is something to hear.
The volume it shows is the workspace's own.
A level set here is the level the reading view opens at.

## `<Button x:Name="PReflexRenewal" Grid.Row="1" Style="{StaticResource Theme.Reflex.Rebuild}" Visibility="Collapsed" />`

The fetch-again button, pinned to the top right of the sound rows.
It sits outside the reflex block so the rows never push it down.
The editor shows it with the block and sets its `Pending` cue to turn the icon while a fetch runs.

## `<veneer:PEditorDropdown x:Name="PEditorDropdown" />`

The five dropdowns a caret or a row button opens, kept in a control of their own.
So this file holds the form's shape and nothing that floats over it.
The drivers still find each dropdown from the editor, through its logical tree.
