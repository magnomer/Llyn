# PEditor.xaml

## `<ResourceDictionary.MergedDictionaries>`

The form's shape stays here and the rules its parts are drawn by stand beside it.
Fields, the card the lists end with, and the dropdown shells are each a dictionary of their own.
A reader of this file sees where things sit, not how each of them is painted.

## `<Border Margin="10" Style="{StaticResource Theme.Popup.Surface}">`

The same floating ground the other pickers stand on.
It is not a white box of its own that no theme reaches.

## `<Border x:Name="PEditorCommand"`

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
Every row, the primary one included, wears the same play, lookup and download buttons after its brackets.
Pronunciation and volume tray carry the theme's shared styles, so switching mode moves neither of them.
The rows start at the headword's own margin, because an indent here read as a different position.

## `<Border x:Name="PPronunciation" Style="{StaticResource Theme.Pronunciation.Surface}">`

The primary pronunciation: its variety as a flag or a label, then the bracketed field, then its buttons.
The chip is empty for a pronunciation without a variety, so the brackets then open at the margin.
The buttons are the accent tool style, so the primary row reads as the further rows do.
Play shows only while the row has a recording.
Lookup and download open the editor's menus under the button pressed, for this row.

## `<ItemsControl x:Name="PAccent" ItemTemplate="{StaticResource Theme.Accent.Row}">`

The further pronunciations, one editable row each, drawn by the shared accent template.
The row commands are bound here, so a row's buttons reach the editor that owns the draft and the menus.

## `<Grid Margin="3,0,1,0">`

The field and the unseen twin that measures it, stacked on the same cell.
The twin decides the cell's width, so the brackets close on the text instead of on a fixed box.

## `<Border x:Name="PPlayback" Height="37" Margin="10,0,0,0" Background="Transparent" BorderThickness="0" ...>`

The volume tray holds the level every row's recording is played at.
It is the same bare tray the reading view draws.
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
