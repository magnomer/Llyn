# LEngine.cs
Hash: `e78be1163a2dc6ff`

## `public sealed class LEngine : IDisposable`

The shell engine is the single boundary the UI shell talks to.
The UI sends a request here.
It hands a delegate over `LLookupStep` for pronunciation or over `LHarvestStep` for audio.
It subscribes a delegate over `LBulletin` to learn that stored data changed.
The first two stream one answer to the caller that asked.
The third announces a change to everyone.

The deportment never holds the engine itself.
It holds the `L*Port` slices under `Port/`, cut by concern so each facade implements the ones it serves.
Four sealed `L*Outlet` classes implement the ports no single facade serves and forward their members to this engine.
`LHost` hands those outlets and the facades in.

The engine is serialised behind `LEngineGate`.
Every public entry point holds a single lock for the whole of its work.
The lock is reentrant, which is what lets an entry point call another one.
The entry points that return a `Task` hold the gate briefly and run the fetch outside it.
A lock held across an await would stall the shell for a whole network call.
The clerks that fetch in the background are handed the same gate object.
So a fetch that lands writes under the lock every other vault call holds.
A held draft is driven by an `LTenure`, made in `LTenureFacade.cs`, so no panel sequences the draft calls itself.

The engine holds no vault of its own.
Every port arrives in one `LRig` and is handed to the staff `LEngineStaffBuild` builds over it.
The composition root, `LHost`, builds the rig through `LRigFactory`, and a test builds one from fakes.
The engine never names the infrastructure and needs no SQLite to start.
All use cases sit in `Llyn.Application` as sealed clerks over the rig, one per concern.
The core keeps the gate, the staff, shared state, observers and one property per facade.
Each sealed facade holds the engine and uses its gate to call a clerk.
`LEngineStaff` keeps the clerks in dependency order and swaps them together when the rig changes.

## `public LEngine(LRig rig, Func<string, LRig> factory, Action<string> pointer)`

Binds the engine to the ports of `rig`, already built over the workspace they stand on.
It neither reads nor rewrites the recorded workspace pointer.
Host hands `factory`, which builds a rig over a folder, and `pointer`, which records the chosen folder.
Both stay outside, since the engine names no infrastructure, and pass on to the workspace facade.
The clerks are built first, then the settings are read through them.
The facades come last, so none of them can ever see an engine without its staff.

## `public LVistaFacade LEngineVista { get; }`

Owns the vista registry and serves the entry and favorite rows shown by each catalog tab.

## `internal LTenureFacade LEngineTenure { get; }`

The facade that starts and commits draft holds, each driven by an `LTenure`.

## `public LMentionFacade LEngineMention { get; }`

The facade for mentions inside sentences and etymologies, built once with the others.

## `internal LMarkupFacade LEngineMarkup { get; }`

The facade for the markup import, built once with the others.

## `internal LCourierFacade LEngineCourier { get; }`

The facade for the Joplin push and its access grant, which the portrait outlet forwards to.

## `internal LLiveryFacade LEngineLivery { get; }`

The facade for the Joplin page read, built once with the others.
`LCourierFacade.LEngineCourierSend` hands its page read to the Joplin push.

## `public LWorkspaceFacade LEngineWorkspace { get; }`

The facade for workspace state and trail operations, built once with the others.

## `public LReflexFacade LEngineReflex { get; }`

The facade for reflexes and the reflex fetch, built once with the others.
It is public, since Host hands it to Conduct as the reflex port.

## `internal LPortraitFacade LEnginePortrait { get; }`

The facade for printing and exporting portraits, built once with the others.

## `public LStemFacade LEngineStem { get; }`

The facade for the phonetic series the xiesheng panel browses, built once with the others.
It is public, since Host hands it to Conduct as the stem port.

## `public LFanqieFacade LEngineFanqie { get; }`

The facade for the fanqie and diwei reads, built once with the others.
It is public, since Host hands it to Conduct as the fanqie and diwei ports.

## `public LLanguageFacade LEngineLanguage { get; }`

The facade for language packs, built once with the others.

## `public LVocabularyFacade LEngineVocabulary { get; }`

The facade for vocabularies, inflections and paradigms, built once with the others.
It is public, since Host hands it to Conduct as the paradigm and sentence ports.

## `public LPronunciationFacade LEnginePronunciation { get; }`

The facade for pronunciations, recordings, lookups, transcriptions and frequencies, built once with the others.

## `internal LSettingsFacade LEngineSettings { get; }`

The facade for settings, built once with the others.

## `internal LRequestFacade LEngineRequest { get; }`

The facade that applies one edit request at a time to a held draft.

## `public LAuthorFacade LEngineAuthor { get; }`

The facade for authors, built once with the others.

## `public LExampleFacade LEngineExample { get; }`

The facade for examples, built once with the others.

## `public LReferenceFacade LEngineReference { get; }`

The facade for references, built once with the others.

## `public LSituationFacade LEngineSituation { get; }`

The facade for situations, built once with the others.

## `public LCardFacade LEngineCard { get; }`

The facade for cards, over meanings, collocations, tags, registers and translations.

## `public LEntryFacade LEngineEntry { get; }`

The facade for entries, built once with the others.

## `internal LEngineStaff LEngineStaffHeld`

Returns the current staff record for engine facades that need a clerk.
The record changes as one unit when `LEngineRigApply` moves to another workspace.
It is read under the gate, so a caller outside the gate still sees one whole record.

## `internal LSettings LEngineSettingsRead()`

The current settings snapshot, read under the gate.
The fetch clerks hold this reader, so it exists before any facade does.

## `public LDraftFacade LEngineDraft { get; }`

The draft facade, built once with the others like every facade.

## `private void LEngineWorkspaceOpen()`

The four steps a bound workspace needs before the first read.
The controlled vocabularies come from the language packs on disk and are written into the workspace.
The 音韻地位 categories are derived again from the stored fanqie rows and the hypothesis on disk.
The phonetic series of every pack that declares a series source are built again.
`LLanguageStaff.LLanguageStaffApply` runs those two steps on the language group.
A workspace rebuilt from an older schema has its derived strings filled once through the workspace clerk.

## `internal object LEngineGate { get; } = new();`

The shared lock every facade and fetching clerk uses for engine state.
Callers hold it while reading or changing the state below.

## `internal LTrove LEngineTrove { get; } = new();`

The engine session cache, shared with the facades that read and clear it.

## `internal LSettings LEngineSettingsHeld { get; set; }`

The current settings snapshot.
Callers read and update it under `LEngineGate`.

## `internal HashSet<long> LEngineDraftStale { get; } = [];`

Draft ids claimed in a former workspace, kept so a surviving tenure cannot write into the new one.

## `internal long LEngineRevision { get; set; }`

A count that moves on every announced change, so a tenure knows when its kept state went stale.
Callers read and move it under `LEngineGate`.
A facade that writes without announcing moves it by hand, so no kept state outlives the write.

## `public void LEngineObserverAttach(Action<LBulletin> observer)`

Subscribes `observer` to future announcements.
An already attached delegate is not added a second time.
It names no subject or id, so it hears every announcement.

## `public void LEngineObserverDetach(Action<LBulletin> observer)`

Stops announcing to `observer`, so a closed surface is never called again.

## `internal void LEngineBulletinRaise(LSubject subject, long id)`

Announces one stored change to every subscriber.
The revision moves first, even with nobody listening, because a tenure reads it whether or not it subscribed.
The `LBulletinRoster` is copied under the gate, then dispatched outside it.
So a subscriber can detach without disturbing iteration.
Announcements happen when a stored record is finished, so an import announces once after all its records are written.

## `internal void LEngineRigApply(LRig rig)`

Moves the engine's clerks, caches and settings onto the workspace `rig` was built over.
`LWorkspaceFacade.LEngineRigApply` calls it under the gate, and keeps the folder, the rescue and the bulletin.
The new rig's settings are read through the clerk's static open step before anything here changes.
A folder that cannot be opened therefore leaves the old clerks and caches untouched.
A workspace that already holds a settings file is opened on its own settings.
One without any receives the current settings and has them written.
That write goes through the clerk's `LWorkspaceFallbackSave`, which records a vault fault and swallows it.
So the switch never stops halfway, and the next settings save tries again.
The drafts this engine claimed are forgotten with the old folder and each id is marked stale.
The old recording clerk stops its playback, since the shared phonograph outlives it.
A shell still holding one is refused instead of writing here.
The pending fetches of the old clerks are cancelled, and the flag cache and the trove are cleared.

## `public void Dispose()`

Cancels every pending fetch, so a closed engine leaves no task writing into the workspace.
`LLanguageStaff.LLanguageStaffClear` does the cancelling, since the language group holds every fetching clerk.
