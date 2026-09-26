# PEditor.xaml

## `UserControl`

The shared editor as markup alone.
The Deportment class of the same name loads it and drives every part.
Every icon, click, command and popup link is set by that class, so the markup holds no hook.

## `<Grid UseLayoutRounding="False">`

Keep Auto row sizes unrounded and round each section's contents like PDisplay.

## `<ResourceDictionary.MergedDictionaries>`

The form's shape stays here and the rules its parts are drawn by stand beside it.
Fields, the card the lists end with, and the dropdown shells are each a dictionary of their own.
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

## `<TextBox x:Name="PMarkerField" Style="{StaticResource Theme.Marker.Text}" Tag="{DynamicResource Speech.Title}" />`

Editable, not a chooser.
The switch beside it opens the language's presets, and the box takes anything typed.
So a part of speech no pack declares can still be written down.
Each one written becomes a chip in the list before the field.

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

## `<local:PScript x:Name="PEditorScript" Margin="0,14,0,0" PScriptFolded="True" />`

The character styles of the entry, the same box the reading view shows, folded under its head here.
The editor never writes them, so they sit closed as reference until the switch opens them.

## `<local:PFanqie x:Name="PEditorFanqie" Margin="0,14,0,0" PFanqieFolded="True" />`

The rime-book placements of the entry, the same box the reading view shows, folded under its head here.

## `<Border x:Name="PPronunciation" Style="{StaticResource Theme.Pronunciation.Surface}">`

The primary pronunciation: its variety as a flag or a label, then the bracketed field, then its buttons.
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
It holds the rows, the fetching line and the fold switch.
It stays collapsed until a draft in a language with reflex rules or reflex rows is rendered.
`PReflexTable` is the stack of rows, sized by hand while a fetch runs.
`PReflexLoading` is the fetching line under the rows, shown only while a fetch runs.
`PReflexFold` shows or hides the folded rows, and stays hidden while no row is folded.
`PAnchor` is the anchor dropdown, one popup for every row, targeted at the label that opened it.
`PAnchorList` holds its tick rows and `PAnchorEmpty` the notice shown when the character has no placement.

## `<local:PReflexList x:Name="PReflex" ItemTemplate="{StaticResource Theme.Reflex.Row}" />`

The reflexes, one editable row each, drawn by the shared reflex template.

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
The editor shows it with the block and sets its tag to turn the icon while a fetch runs.

## `<Popup x:Name="PNotation" ...>`

The pronunciation menu, declared once beside the other popups the editor owns.
It has no placement target of its own, because the row button that opens it becomes the target.
Closing it, by a pick or a click elsewhere, calls the search off.

## `<Popup x:Name="PClip" ...>`

The audio menu, declared once for the same reason and opened the same way.
