# TEngineRig.cs
Hash: `3fd868a63b28c5a2`

## `public sealed class TEngineRig`

Covers the engine standing on a rig of fakes, with no file on disk.
A start reads the settings and the rescue through the fake ports and names the rig's root.
An entry created through the engine lands in the fake entry vault and reads back from it.
A rig apply moves the engine onto the second rig.
The next read then hits the second fake and not the first.
The settings the second rig never held are inherited, as a fresh folder inherits them.
A second rig whose default settings cannot be written still opens with the inherited settings.
The write's vault fault is recorded once and goes no further.
