# PEditor.xaml

## `<Grid Margin="{Binding Padding, RelativeSource={RelativeSource AncestorType=UserControl}}">`

The margin is the host panel's.
The input panel gives the editor the whole tab.
A browse-style panel gives it the card beside its catalog.

## `<Border Margin="10" Style="{StaticResource Theme.Popup.Surface}">`

The same floating ground the other pickers stand on.
It is not a white box of its own that no theme reaches.

## `<Rectangle Width="14" Height="16" Margin="0,0,9,0" Fill="{Binding Foreground, RelativeSource={RelativeSource AncestorType=Button}}">`

Phosphor trash.svg.
SvgViewbox has no foreground of its own, so the glyph is used as an opacity mask over the button's foreground.

## `<Rectangle Width="15" Height="15" Margin="0,0,9,0" Fill="{Binding Foreground, RelativeSource={RelativeSource AncestorType=Button}}">`

Phosphor floppy-disk.svg, tinted through an opacity mask (see PDiscard).

## `<Rectangle Width="13" Height="15" Fill="{Binding Foreground, RelativeSource={RelativeSource AncestorType=Button}}">`

Phosphor play.svg, tinted through an opacity mask (see PDiscard).

## `<Rectangle Width="15" Height="15" Fill="{Binding Foreground, RelativeSource={RelativeSource AncestorType=ToggleButton}}">`

Phosphor magnifying-glass.svg, tinted through an opacity mask (see PDiscard).

## `<Rectangle Width="15" Height="15" Fill="{Binding Foreground, RelativeSource={RelativeSource AncestorType=ToggleButton}}">`

Phosphor box-arrow-down.svg, tinted through an opacity mask (see PDiscard).

## `<TextBlock`

The part of speech as it stands, beside the field that sets it.
The label is bound straight to the box.
So it says what is typed while it is being typed, rather than after the entry is saved.

## `<TextBox x:Name="PSpeechContents" Width="212" MinHeight="40" Padding="12,0" Background="Transparent" BorderThickness="0" FontSize="15" Foreground="{StaticResource Theme.Ink}" Style="{StaticResource Theme.Input.Field}" Tag="Part of speech" VerticalContentAlignment="Center" />`

Editable, not a chooser.
The dropdown offers the language's presets and this box takes anything.
So a part of speech no pack declares can still be written down.
