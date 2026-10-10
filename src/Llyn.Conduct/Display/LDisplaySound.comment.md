# LDisplaySound.cs
Hash: `3b183f53f5877bd0`

## `internal sealed class LDisplaySound`

The reading view's sound state holds the shown draft, its entry and the latest play ticket.
The gates and reads over it stand on [CDisplaySound](CDisplaySound.comment.md), which holds no state of its own.
The pronunciation reads stand on [CDisplayAccent](CDisplayAccent.comment.md), and the play gates on [CDisplayPlayback](CDisplayPlayback.comment.md).
Caught list-read and check failures answer empty or false, so their sections can still draw.
`LDisplayReflexRead` does not catch failures itself.
Every refused read it catches raises `LDisplaySoundFailed` with its text key, so none passes unseen.
Every refused send raises `LDisplayMarkFailed` instead, since a send follows a show the user caused.
The engine owns the player, so the view holds no player of its own.
It keeps its latest play's ticket, so it never stops another view's sound.
The editor's [CKindred](../Sound/CKindred.comment.md) reads the opened state and the reflex check here, and asks for a rebuild.
It holds no opened state, since the engine stores it per entry.
The editor's sound sheet, [CSounding](../Sound/CSounding.comment.md), reads the morphology verdict here too.
Only Conduct reads it, so the drivers are offered no Core draft.

## `internal LDisplaySound(LEntryPort entries, LLanguagePort languages, LReflexPort reflexes, LFanqiePort fanqies, LScriptPort scripts, LParadigmPort paradigms, LMediaPort media, LSettingsPort settings)`

Holds the ports the state's own reads, fetch starts and play stops go through.
Each check or rebuild goes through the one narrow port that owns its rows.

## `internal LEntryDraft? LDisplayShown`

The shown draft, which the lectern's areas and its compass read.

## `internal long? LDisplayEntry`

The id of the shown entry, which every sound read and fetch request is made for.

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
A present entry id starts background sound fetches through one engine call, leaving their order to the engine.
The start runs on every show of a draft.
A refused start raises `LDisplayMarkFailed` with `Sound.StartFailed`.

## `internal void LDisplaySoundClear()`

Drops the shown draft, its id and the loaded reflexes.
It stops its own playback, and a sound another view started plays on.
A display that shows nothing has no playback, so clearing it again stops nothing.

## `internal bool LDisplaySpreadCheck(long? id)`

Whether the entry's "More readings" is opened, as the engine stores it for that entry.
No entry answers false, and so does a refusal.
A refusal raises `LDisplaySoundFailed` with `Reflex.SpreadReadFailed`.
The reading view and the editor both read here, each for its own entry.

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
