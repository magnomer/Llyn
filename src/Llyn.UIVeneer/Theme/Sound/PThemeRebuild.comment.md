# PThemeRebuild.xaml
Hash: `0967ab6bc83fe474`

## `<Style x:Key="Theme.Sound.Rebuild" TargetType="Button">`

The one regenerate button of every sound part: rime books, character forms and readings.
It carries the rebuild mark alone and names itself through its tooltip, since the parts label their own heads already.
The mark and its turn are `QLook` rows in `QLookSound.cs`.
The mark turns while the button carries the `Pending` cue, so a running fetch shows where it was asked.

## `<DoubleAnimation x:Key="Theme.Sound.Rebuild.Spin"`

The rebuild icon's spin under its contract ID, one turn a second for as long as the rebuild runs.
