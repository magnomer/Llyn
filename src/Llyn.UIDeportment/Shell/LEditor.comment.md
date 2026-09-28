# LEditor.cs

## `public sealed class LEditor`

The entry editor's holder until its remaining pieces reach Conduct.
Every entry gate, read and session flag lives on `CEditor`, which the atelier builds.
It still builds the clip, notation, sound sheet, esteem and timbre, whose own jobs dismantle them.
The two ports it takes serve only the sound sheet and the timbre.

## `public CEditor LEditorStudio { get; }`

The Conduct editor every view calls for an entry gate or read.

## `public LSounding LEditorSounding { get; }`

The sound sheet of the held entry, which the editor hands to the rime, script and paradigm boxes.

## `public QEsteem LEditorEsteem { get; }`

The favourite, grasp and frequency of the held entry, built over the editor's desk and display.

## `public QTimbre LEditorTimbre { get; }`

The pack sound facts and sound-sheet commands of the held entry, built over the desk, display and sheet.

## `public long? LEditorEntry`

The stored entry the held draft stands on, or null for a fresh draft or an empty desk.

## `public bool LEditorMorphology`

Whether the paradigm box shows morphology, as the display reads the settings.

## `internal LTenure? LEditorTenure`

The held tenure for a field edit, or null while none is held or the desk fills its controls.
