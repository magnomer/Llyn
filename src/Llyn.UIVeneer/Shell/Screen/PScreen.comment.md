# PScreen.xaml

The static frame one film plays in: a black stage above a play switch.
The markup carries no hook.
`PScreen` in Deportment loads it, subscribes every event and places each player inside the stage.

## `<Border x:Name="PScreenStage"`

The black stage with rounded corners, never shorter than 180 pixels.
Its height follows its width in Deportment, so the markup sets none.

## `<MediaElement x:Name="PScreenMedia"`

The machine's own player for a file, collapsed until a file is played.
Its loading is manual, so nothing plays until Deportment asks.

## `<Grid x:Name="PScreenPage"`

The empty slot the embedded browser is put into, collapsed until a web address is played.
The browser is built by logic, so only the slot is markup.

## `<TextBlock x:Name="PScreenNotice"`

The muted line shown in place of a film that failed, collapsed until then.

## `<ToggleButton x:Name="PScreenSwitch"`

The play switch beneath the stage, in the accent colour of the toggle action style.
