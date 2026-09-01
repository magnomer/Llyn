# PList.xaml

## `<UserControl.Resources>`

The read-only card shapes. A Meaning card and a Collocation card are one card (LCardDraft), so the two templates below differ in one element: the Collocation's Expression. Every member the loaded draft carries is drawn, and a field the card left empty collapses rather than leaving a blank line, which is what these styles are for.

## `<Style x:Key="Display.Order.Row" TargetType="RadioButton">`

A sorting choice reads as one row in a single menu, not as another stack of cards. The checked row keeps the current order visible when the menu is opened again.

## `<Grid x:Name="POrder" Grid.Row="0" Grid.Column="0" Margin="0,0,20,18">`

The two controls share the index column, so their combined edge is the same as the catalog below. The viewer deliberately has no control in this row: that clear band keeps its card visually separate from the browsing tools that act on the catalog.

## `<Canvas Width="16" Height="14">`

Three descending bars: the sorting mark, drawn rather than fetched.

## `<RadioButton GroupName="POrderChoice" IsChecked="True" Click="POrderHandle" Style="{StaticResource Display.Order.Row}" Tag="Headword" Content="{DynamicResource Order.Headword}" />`

Radio state makes the current ordering legible without repeating it on the compact toolbar button. The tag remains the stable value the handler reads.

## `<StackPanel Grid.Row="0" Margin="20,16,20,10" HorizontalAlignment="Right" Orientation="Horizontal">`

Entry actions stay above both the reader and editor. Only Edit is wired today; New, Export and Print are present as the requested mock-up controls.

## `<Button x:Name="PDisplayPlayback" Width="42" Height="42" Margin="10,0,0,0" Background="{StaticResource Theme.Surface}" BorderBrush="{StaticResource Theme.Line}" Foreground="{StaticResource Theme.Accent}" Click="PDisplayPlaybackHandle" Style="{StaticResource Theme.Input.Action}" Visibility="Collapsed">`

Shown only when the entry has an audio file on disk; see PDisplayShow.

## `<Rectangle Width="11" Height="13" Fill="{Binding Foreground, RelativeSource={RelativeSource AncestorType=Button}}">`

Phosphor play.svg, tinted through an opacity mask (see PPlayback).

## `<StackPanel x:Name="PDisplaySenseSection" Margin="0,34,0,0">`

The editor's tabs become document sections here, so reading never hides one kind of content behind another.

## `<local:PEditor x:Name="PEditor" Grid.Row="1" Padding="20,4,20,20" Visibility="Collapsed" />`

The same editor the input panel mounts, over the entry this panel has selected: what differs is not the editing structure but that this one always stands on an entry, so a store modifies that entry and a discard puts it back as it is stored.
