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
