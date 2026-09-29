# LDisplaySound.cs

## `public sealed class LDisplaySound`

The reading view's sound state: the shown draft, its entry, the reflex fold and the latest play.
The gates and reads over it stand on [CDisplaySound](CDisplaySound.comment.md), which holds no state of its own.
A read the engine refuses answers empty, so a section draws nothing rather than failing the page.
The engine owns the player, so the view holds no player of its own.
It keeps its latest play's ticket, so it never stops another view's sound.
The editor's reflex block still calls its reflex members and its fold, until job75 takes that block apart.
The editor's cadence still reads the morphology verdict here.

## `internal LDisplaySound(LEntryPort entries, LPhonologyPort phonology, LMediaPort media, LSettingsPort settings)`

Holds the ports the state's own reads, fetch starts and plays go through.

## `public LEntryDraft? LDisplayShown => _lDisplaySoundDraft;`

The shown draft, which the Conduct reads hand the engine unread.
The lectern's card half still reads its cards here, until job66.

## `internal long? LDisplayEntry => _lDisplaySoundEntry;`

The id of the shown entry, which every phonology read and fetch request is made for.

## `public bool LDisplayFoldOpened => _lDisplaySoundOpened;`

Whether the folded reflexes are shown, shared by the reading view and the editor.

## `internal int LDisplayTicket { get; set; }`

The ticket of this view's latest play, which the playback gate sets.
A stop sends it, so the engine stops this play alone.

## `internal event Action<string, Exception>? LDisplaySoundFailed;`

Raised with the text key and the exception when a read the view cannot do without is refused.
[LDisplay](LDisplay.comment.md) passes it on to the panel's envoy.

## `internal void LDisplaySoundShow(long? id, LEntryDraft draft)`

Holds `draft` and its entry `id` as the entry the verdicts and reads work on.
Reflexes loaded for an earlier entry are dropped, so the rows follow the new draft.
It starts every background fetch the view shows through one engine call, whose order is the engine's.
The start runs once per opened entry, however many lecterns show it.

## `internal void LDisplaySoundClear()`

Drops the shown draft, its id and the loaded reflexes.
It stops its own playback, and a sound another view started plays on.

## `internal void LDisplayFoldSet(bool opened)`

Sets whether the folded reflexes are shown, behind the gate `CDisplaySound.CDisplayReflexToggle`.

## `internal void LDisplayReflexLoad()`

Reloads the shown entry after a reflex fill, and keeps the stored draft for the reflex rows alone.
Every other read still works on the draft on screen.
A refused or empty load keeps the rows as they were, and a refusal raises `LDisplaySoundFailed`.

## `internal IReadOnlyList<LReflexDraft> LDisplayReflexRead()`

The written reflexes, as the engine filters them.
A draft reloaded after a fill wins over the shown draft.

## `internal void LDisplayReflexStart(long? id)`

Asks the engine to fill the entry's reflex rows, for the editor's reflex block.

## `internal bool LDisplayReflexCheck(long? id)`

Whether a reflex fill runs for the entry, false for no entry or when the engine refuses to say.

## `internal void LDisplayReflexRebuild(long? id)`

Asks the engine to drop the entry's reflex rows and fetch them again.

## `internal bool LDisplayFanqieCheck(long? id)`

Whether the entry's rime-book rows are still being fetched, false for no entry or a refusal.

## `internal bool LDisplayScriptCheck(long? id)`

Whether the entry's script images are still being fetched, false for no entry or a refusal.

## `internal bool LDisplayParadigmCheck(long? id)`

Whether the entry's inflections are still being fetched, false for no entry or a refusal.

## `public bool LDisplayMorphologyRead()`

The engine's morphology verdict, false when the settings cannot be read.

## `internal void LDisplayPlaybackStop()`

Stops this view's play, and leaves a later play from another view running.

## `internal IReadOnlyList<LDisplayItem> LDisplayListRead<LDisplayItem>(Func<long, IReadOnlyList<LDisplayItem>> read)`

Runs `read` for the shown entry, answering no rows for no entry or a refusal.

## `private static void LDisplayMarkSend(Action<long> mark, long? id)`

Sends `mark` for the entry, and swallows a refusal, since a fetch request answers nothing.

## `private static bool LDisplayPendingRead(Func<long, bool> check, long? id)`

Runs `check` for the entry, answering false for no entry or a refusal.
