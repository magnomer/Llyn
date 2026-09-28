# LEditor.cs

## `public sealed class LEditor`

The entry editor's holder until its remaining pieces reach Conduct.
Every entry gate, read and session flag lives on `CEditor`, which the atelier builds.
It still builds the clip, notation, esteem and timbre, whose own jobs dismantle them.
The phonology port it takes serves only the timbre.
The sound sheet is `CEditor.CEditorSounding`, which the timbre is handed.

## `public CEditor LEditorStudio { get; }`

The Conduct editor every view calls for an entry gate or read.

## `public QEsteem LEditorEsteem { get; }`

The favourite, grasp and frequency of the held entry, built over the editor's desk and display.

## `public QTimbre LEditorTimbre { get; }`

The pack sound facts and sound-sheet commands of the held entry, built over the desk, display and Conduct sheet.

## `public long? LEditorEntry`

The stored entry the held draft stands on, or null for a fresh draft or an empty desk.

## `public bool LEditorMorphology`

Whether the paradigm box shows morphology, as the display reads the settings.

## `internal LTenure? LEditorTenure`

The held tenure for a field edit, or null while none is held or the desk fills its controls.
