# PThemePlayback.xaml
Hash: `d516781fe3e24f8b`

## `<Style x:Key="Theme.Playback.Action" TargetType="Button">`

The play button, drawn as a command inside the playback tray rather than as a control of its own.
It is the icon toggle's treatment without the held state, because playing is done the moment it is asked for.
Both views draw it, because a recording is played where it is heard and where it is chosen.

## `<ScaleTransform x:Key="Theme.Playback.Action.PressScale" ScaleX="0.88" ScaleY="0.88" />`

The shrink a pressed playback action takes, set by key so the deportment holds no size.
