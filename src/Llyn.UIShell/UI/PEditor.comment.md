# PEditor.xaml

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

## `<Rectangle Width="13" Height="15" Fill="{Binding Foreground, RelativeSource={RelativeSource AncestorType=Button}}">`

Phosphor play.svg.
SvgViewbox has no foreground of its own, so the glyph is used as an opacity mask over the button's foreground.

## `<Rectangle Width="15" Height="15" Fill="{Binding Foreground, RelativeSource={RelativeSource AncestorType=ToggleButton}}">`

Phosphor magnifying-glass.svg, tinted through an opacity mask (see PPlayback).

## `<Rectangle Width="15" Height="15" Fill="{Binding Foreground, RelativeSource={RelativeSource AncestorType=ToggleButton}}">`

Phosphor box-arrow-down.svg, tinted through an opacity mask (see PPlayback).

## `<TextBlock`

The part of speech as it stands, beside the field that sets it.
The label is bound straight to the box.
So it says what is typed while it is being typed, rather than after the entry is saved.

## `<TextBox x:Name="PMarkerField" Width="212" MinHeight="40" Padding="12,0" Background="Transparent" BorderThickness="0" FontSize="15" Foreground="{StaticResource Theme.Ink}" Style="{StaticResource Theme.Input.Field}" Tag="Part of speech" VerticalContentAlignment="Center" />`

Editable, not a chooser.
The dropdown offers the language's presets and this box takes anything.
So a part of speech no pack declares can still be written down.

## `<StackPanel Grid.Row="1" Margin="0,12,0,0" HorizontalAlignment="Left" Orientation="Horizontal">`

The pronunciation row, laid out as the reading view lays it out.
Pronunciation and playback tray carry the theme's shared styles, so switching mode moves neither of them.
The row starts at the headword's own margin, because an indent here read as a different position.

## `<Grid Margin="3,0,1,0">`

The field and the unseen twin that measures it, stacked on the same cell.
The twin decides the cell's width, so the brackets close on the text instead of on a fixed box.

## `<Border x:Name="PPlayback" Height="37" Margin="10,0,0,0" Background="Transparent" BorderThickness="0" ...>`

The playback row: the play button and the volume it is played at, the same bare row the reading view draws.
It appears with a recording and goes with it, so the volume is offered only while there is something to hear.
The volume it shows is the workspace's own, so a level set here is the level the reading view opens at.

## `<Border Height="41" Margin="10,0,0,0" Style="{StaticResource Theme.Command.Group}">`

Lookup and audio download both fetch what the chip beside them holds, so one tray holds the pair.
It is the tray discard and store sit in, at the height of the chip and the playback tray.
Only the editor draws it, because nothing is fetched into a view that cannot be typed into.
