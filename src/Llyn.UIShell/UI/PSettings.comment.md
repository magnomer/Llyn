# PSettings.xaml

## `<StackPanel Margin="32" HorizontalAlignment="Left" VerticalAlignment="Top">`

The stored choices, one heading and one control each, read top to bottom.
Every choice takes effect in the engine, so the panel holds no state of its own.

## `<CheckBox x:Name="PRespelling" Margin="0,12,0,0" Content="{DynamicResource Respelling.Switch}" ...>`

Whether looked-up transcriptions are recast into the pack's symbol convention.
The next lookup reads it, so nothing on screen changes when it flips.

## `<CheckBox x:Name="PFrequency" Margin="0,12,0,0" Content="{DynamicResource Frequency.Switch}" ...>`

Whether an entry's frequency is fetched from the pack's web source.
It sits below respelling in the same shape, since both govern what a lookup fetches.
A pack that declares no frequency source fetches nothing either way, so the switch costs nothing there.
