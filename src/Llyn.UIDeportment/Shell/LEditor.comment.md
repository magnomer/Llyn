# LEditor.cs

## `public sealed class LEditor`

The entry editor's holder until its remaining pieces reach Conduct.
Every entry gate, read and session flag lives on `CEditor`, which the atelier builds.
The marks, the sound facts and the sound sheet are `CEditor`'s too, so it holds no port.
The recording and reading searches are the desk's `CErrand`, so it builds nothing of its own.

## `public CEditor LEditorStudio { get; }`

The Conduct editor every view calls for an entry gate or read.

## `public long? LEditorEntry`

The stored entry the held draft stands on, or null for a fresh draft or an empty desk.

## `public bool LEditorMorphology`

Whether the paradigm box shows morphology, as the display reads the settings.

## `internal LTenure? LEditorTenure`

The held tenure for a field edit, or null while none is held or the desk fills its controls.
