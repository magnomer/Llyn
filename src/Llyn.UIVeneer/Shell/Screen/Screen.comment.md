# Screen

The screen is the static frame one film plays in, a black stage above a play switch.
The markup carries no hook.
`QScreen` in Deportment pulls each part by name, subscribes every event and places each player inside the stage.
Each video template places it as `veneer:PScreen`, and `QScreen` drives each realized instance.

## `PScreen`

The shell behind `PScreen.xaml`.
Its constructor only calls `InitializeComponent()`.

## `PScreenStage`

The black stage with rounded corners, never shorter than 180 pixels.
Its height follows its width in Deportment, so the markup sets none.

## `PScreenMedia`

The machine's own player for a file, collapsed until a file is played.
Its loading is manual, so nothing plays until Deportment asks.

## `PScreenPage`

The empty slot the embedded browser is put into, collapsed until a web address is played.
The browser is built by logic, so only the slot is markup.

## `PScreenNotice`

The muted line shown in place of a film that failed, collapsed until then.

## `PScreenSwitch`

The play switch beneath the stage, in the accent colour of the toggle action style.

## Stamp

Commit: `c2f335c331fe`
- `PScreen.xaml` `deedb7ba5fc2437f`
- `PScreen.xaml.cs` `c4ca30bc56a698d2`
