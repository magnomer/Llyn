# PEditor.xaml

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
A bin of one size and a diskette of another, beside labels of two lengths, left the pair looking lopsided.
Both take the width of their label, so a longer language does not clip.

## `<TextBlock`

The part of speech as it stands, beside the field that sets it.
The label is bound straight to the box.
So it says what is typed while it is being typed, rather than after the entry is saved.

## `<TextBox x:Name="PMarkerField" Width="212" MinHeight="40" Padding="12,0" Background="Transparent" BorderThickness="0" FontSize="15" Foreground="{StaticResource Theme.Ink}" Style="{StaticResource Theme.Input.Field}" Tag="Part of speech" VerticalContentAlignment="Center" />`

Editable, not a chooser.
The dropdown offers the language's presets and this box takes anything.
So a part of speech no pack declares can still be written down.

## `<StackPanel Grid.Row="1" Margin="12,12,0,0" HorizontalAlignment="Left">`

The pronunciation rows, laid out as the reading view lays them out.
The primary pronunciation stands on the first row with the volume tray beside it.
Every further pronunciation stands on a row of its own beneath, in the accent list.
The transcription rows follow beneath those, one per scheme, only for a language whose pack declares schemes.
Every row, the primary one included, wears the same play, lookup and download buttons after its brackets.
Every row ends with the plus and minus pair the example rows carry, shown on hover.
The plus adds a row beneath and the minus drops the row.
The row commands are bound here, above the primary row and the accent list.
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
The brackets are named so the code behind can turn them into slashes for a phonemic respelling.
The chip is empty for a pronunciation without a variety, so the brackets then open at the margin.
The buttons are the accent tool style, so the primary row reads as the further rows do.
Play shows only while the row has a recording.
Lookup and download open the editor's menus under the button pressed, for this row.
The plus and minus carry no row, which the handlers read as the primary.

## `<local:PContour x:Name="PContour" Style="{StaticResource Theme.Contour.Box}" ...>`

The tone contour of the primary pronunciation, drawn beneath the chip when the chosen language is tonal.
Its IPA is bound to the pronunciation field, so the picture follows every keystroke.
The tonal flag is pushed by `PEditorContourApply` whenever the language changes.

## `<Grid x:Name="PReflexBlock" Visibility="Collapsed">`

The reflex block at the head of the reading stack.
It holds the rows and the fetch-again button at their top right corner.
It stays collapsed until a draft in a language with reflex rules or reflex rows is rendered.
`PReflexTable` is the stack of rows the button sits beside, sized by hand while a fetch runs.
`PReflexLoading` is the fetching line under the rows, shown only while a fetch runs.
`PReflexRenewal` is the button itself, whose tag turns its icon while a fetch runs.
`PAnchor` is the anchor dropdown, one popup for every row, targeted at the label that opened it.
`PAnchorList` holds its tick rows and `PAnchorEmpty` the notice shown when the character has no placement.

## `<ItemsControl x:Name="PReflex" ItemTemplate="{StaticResource Theme.Reflex.Row}" />`

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

## `<Grid Margin="3,0,1,0">`

The field and the unseen twin that measures it, stacked on the same cell.
The twin decides the cell's width, so the brackets close on the text instead of on a fixed box.

## `<Border x:Name="PPlayback" Style="{StaticResource Theme.Volume.Tray}">`

The volume tray holds the level every row's recording is played at.
It is the same bare tray the reading view draws, from the same theme style.
It appears while any row has a recording and goes when none does.
The volume is offered only while there is something to hear.
The volume it shows is the workspace's own.
A level set here is the level the reading view opens at.

## `<Popup x:Name="PNotation" ...>`

The pronunciation menu, declared once beside the other popups the editor owns.
It has no placement target of its own, because the row button that opens it becomes the target.
Closing it, by a pick or a click elsewhere, calls the search off.

## `<Popup x:Name="PClip" ...>`

The audio menu, declared once for the same reason and opened the same way.
