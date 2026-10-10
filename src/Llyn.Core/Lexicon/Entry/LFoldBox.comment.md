# LFoldBox.cs
Hash: `1cc4cf7e5a366082`

## `public enum LFoldBox`

Which folding box of the entry editor a fold request means.
Each box keeps its opened state per entry, and no entry shares it with another.
The two boxes share one shape, so one member pair serves both through this kind.
The "More readings" fold of a reflex list is not a box and keeps its own members.

## `LFoldBoxFanqie,`

The Fanqie box of the entry editor.

## `LFoldBoxScript,`

The Script box of the entry editor.
