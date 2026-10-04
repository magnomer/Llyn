# LDisplaySound.cs
Hash: `c5d7f66185665506`

## `internal sealed class LDisplaySound`

The reading view's sound state: the shown draft, its entry, the reflex fold and the latest play.
The gates and reads over it stand on [CDisplaySound](CDisplaySound.comment.md), which holds no state of its own.
A list read or check the engine refuses answers empty or false, so a section draws nothing.
Every refused read it catches raises `LDisplaySoundFailed` with its text key, so none passes unseen.
Every refused send raises `LDisplayMarkFailed` instead, since a send follows a show the user caused.
The engine owns the player, so the view holds no player of its own.
It keeps its latest play's ticket, so it never stops another view's sound.
The editor's [CTimbre](../Sound/CTimbre.comment.md) reads the fold and the reflex check here, and asks for a rebuild.
So the fold is one state, shared by the reading view and the editor.
The editor's sound sheet, [CSounding](../Sound/CSounding.comment.md), reads the morphology verdict here too.
Only Conduct reads it, so the drivers are offered no Core draft.

## `internal LDisplaySound(LEntryPort entries, LPhonologyPort phonology, LMediaPort media, LSettingsPort settings)`

Holds the ports the state's own reads, fetch starts and play stops go through.

## `internal LEntryDraft? LDisplayShown`

The shown draft, which the lectern's area, its sound area and its compass read.

## `internal long? LDisplayEntry`

The id of the shown entry, which every phonology read and fetch request is made for.

## `internal bool LDisplayFoldOpened`

Whether the folded reflexes are shown, shared by the reading view and the editor.

## `internal int LDisplayTicket { get; set; }`

The ticket of this view's latest play, which the playback gate sets.
A stop sends it, so the engine stops this play alone.

## `internal event Action<string, Exception>? LDisplaySoundFailed;`

Raised with the text key and the exception when a read the view cannot do without is refused.
[LDisplay](LDisplay.comment.md) passes it on through the atelier's repaint memory.
A read repeats on every repaint, so a lasting refusal shows once until the user acts.

## `internal event Action<string, Exception>? LDisplayMarkFailed;`

Raised with the text key and the exception when a fetch start or a reflex rebuild is refused.
[LDisplay](LDisplay.comment.md) shows it through the panel's envoy every time, since each send answers a user act.

## `internal void LDisplaySoundShow(long? id, LEntryDraft draft)`

Holds `draft` and its entry `id` as the entry the verdicts and reads work on.
Reflexes loaded for an earlier entry are dropped, so the rows follow the new draft.
It starts every background fetch the view shows through one engine call, whose order is the engine's.
The start runs on every show of a draft.
A refused start raises `LDisplayMarkFailed` with `Sound.StartFailed`.

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

## `internal bool LDisplayReflexCheck(long? id)`

Whether a reflex fill runs for the entry, false for no entry or when the engine refuses to say.
A refusal raises `LDisplaySoundFailed` with `Sound.PendingFailed`, as for the fanqie, script and paradigm checks.

## `internal void LDisplayReflexRebuild(long? id)`

Asks the engine to drop the entry's reflex rows and fetch them again.
A refusal raises `LDisplayMarkFailed` with `Display.ReflexRebuildFailed`.

## `internal bool LDisplayFanqieCheck(long? id)`

Whether the entry's rime-book rows are still being fetched, false for no entry or a refusal.

## `internal bool LDisplayScriptCheck(long? id)`

Whether the entry's script images are still being fetched, false for no entry or a refusal.

## `internal bool LDisplayParadigmCheck(long? id)`

Whether the entry's inflections are still being fetched, false for no entry or a refusal.

## `internal bool LDisplayMorphologyRead()`

The engine's morphology verdict, false when the settings cannot be read.
That refusal raises `LDisplaySoundFailed` with `Sound.MorphologyFailed`.

## `internal void LDisplayPlaybackStop()`

Stops this view's play, and leaves a later play from another view running.

## `internal IReadOnlyList<LDisplayItem> LDisplayListRead<LDisplayItem>(Func<long, IReadOnlyList<LDisplayItem>> read, string key)`

Runs `read` for the shown entry, answering no rows for no entry or a refusal.
A refusal raises `LDisplaySoundFailed` with the caller's `key`, so each section names its own failure.

## `private void LDisplayMarkSend(Action<long> mark, long? id, string key)`

Sends `mark` for the entry.
A fetch request answers nothing, so a refusal only raises `LDisplayMarkFailed` with `key`.

## `private bool LDisplayPendingRead(Func<long, bool> check, long? id)`

Runs `check` for the entry, answering false for no entry or a refusal.
A refusal raises `LDisplaySoundFailed` with `Sound.PendingFailed`.
