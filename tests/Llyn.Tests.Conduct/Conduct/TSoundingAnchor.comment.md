# TSoundingAnchor.cs
Hash: `d81e0fda8c148553`

## `public sealed class TSoundingAnchor`

Covers the anchor menu gates of the editor end to end, over a real workspace and no window.
The menu reads the chosen row's anchors off the draft, so the test hands only the row's id.

## `private const string TSoundingAnchorPack`

A pack naming one fanqie book, so the stored rows of its language are read.

## `public void SoundingAnchorOpen_AnchoredReflex_MarksTheHeldReading()`

The open menu offers the stored reading and marks it held, since the row anchors to it.
An id the draft does not hold answers the empty menu.

## `public void SoundingAnchorOpen_FreshDraft_AnswersTheEmptyMenu()`

A draft never stored has no readings, so the menu is flagged empty.

## `public void SoundingAnchorSet_OpenedReflex_TiesAndUntiesTheReadingInTheDraft()`

A tick ties the chosen row to the reading in the draft, and a second tick unties it.

## `public void SoundingAnchorClose_ClosedMenu_LeavesTheDraftAlone()`

After the menu closes no row is chosen, so a late tick changes nothing.

## `private static (long, long) TSoundingAnchorPlace(LEngine engine, TWorkspace workspace, string language)`

Saves a one-character entry with one reflex row, stores one reading of the character and anchors the row to it.
It answers the entry id and the reading id.

## `private static long TSoundingAnchorSave(LEngine engine)`

Saves an English entry with one reflex row and no stored reading.

## `private static TEditorFixture TSoundingAnchorPrepare(LEngine engine, long? entry)`

An input editor on the library vista with `entry` opened on its desk.
The anchor menu is built over the editor's kindred, so a test hands it the fixture's kindred.
