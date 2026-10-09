# LEngine.cs
Hash: `49a79517ceed7ee9`

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

The engine is serialised behind `LEngineHearth.LEngineGate`.
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
`LEngineHearth` keeps the gate, the staff, the shared state and the observers.
The engine keeps one property per facade and forwards the observer calls to the hearth.
Each sealed facade reads the gate, the staff and the shared state through the hearth.
Each facade but the tenure facade takes the hearth and its sibling facades, never the engine.
`LEngineStaff` keeps the clerks in dependency order and swaps them together when the rig changes.

## `public LEngine(LRig rig, Func<string, LRig> factory, Action<string> pointer)`

Binds the engine to the ports of `rig`, already built over the workspace they stand on.
It neither reads nor rewrites the recorded workspace pointer.
Host hands `factory`, which builds a rig over a folder, and `pointer`, which records the chosen folder.
Both stay outside, since the engine names no infrastructure, and pass on to the workspace facade.
The hearth comes first, building the clerks and reading the settings.
The row facade follows it, so every facade that builds vista rows finds it.
The facades come last, so none of them can ever see an engine without its staff.
Each facade is built after the siblings it takes, so its constructor needs no engine.
The tenure facade comes last and alone takes the engine, since its tenures reach every facade.
The workspace opens at the very end, after the workspace facade has created the database.

## `public LVistaFacade LEngineVista { get; }`

Owns the vista registry and serves the entry rows shown by each catalog tab.

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

## `public LScriptFacade LEngineScript { get; }`

The facade for script images, built just before the language facade, which starts their fetch.

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

The facade for cards, over meanings, collocations and translations.

## `public LCatalogFacade LEngineCatalog { get; }`

The facade for the catalog shelves: tags, registers and favorites.
It is built right after the row facade, since it takes no other sibling.

## `public LEntryFacade LEngineEntry { get; }`

The facade for entries, built once with the others.

## `public LDraftFacade LEngineDraft { get; }`

The draft facade, built once with the others like every facade.

## `public void LEngineObserverAttach(Action<LBulletin> observer)`

Subscribes `observer` to future announcements, through the hearth's roster.

## `public void LEngineObserverDetach(Action<LBulletin> observer)`

Stops announcing to `observer`, through the hearth's roster.

## `public void Dispose()`

Cancels every pending fetch, so a closed engine leaves no task writing into the workspace.
`LEngineHearth.LEngineStaffClear` does it, since the hearth owns the gate and the staff.

## `internal LEngineHearth LEngineHearth { get; }`

The shared state every facade reads, built before any facade so none sees an engine without staff.

## `internal LVistaRowFacade LEngineVistaRow { get; }`

The row builder every catalog of entries uses, so no facade reaches the vista facade only to build rows.
