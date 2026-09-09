# PThemeVolume.xaml

## `<Style x:Key="Theme.Volume.Rail" TargetType="RepeatButton">`

The two halves of the volume track, the taken one filled in the accent and the remaining one left clear.
They are repeat buttons because that is what a WPF track is built from.
A click on either walks the volume toward it.

## `<Style x:Key="Theme.Volume.Thumb" TargetType="Thumb">`

The grip the volume is carried by is a ring of the accent around the surface.
It fills as it is pointed at and is solid while it is dragged.
It is a ring rather than a dot, so the track it sits on stays readable underneath it.

## `<Style x:Key="Theme.Volume.Slider" TargetType="Slider">`

The volume of a played recording, from silence to full over its own width.
It runs zero to one, the range a media player takes.
Nothing between the grip and the sound rescales it.
A click anywhere on the track moves the grip there.
A volume is chosen by where it should be rather than nudged toward it.
