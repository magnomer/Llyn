# Engine

The engine proper, its hearth, the bulletin roster and the staff groups of clerks it builds for one rig.
It stands apart from Port because the engine owns state, gates and bulletins, while a port only narrows a facade.
The staff record groups the clerks by concern, so a facade finds each clerk through the group that owns it.

## `LBulletinRoster`

One ordered list of bulletin observers, shared by the engine, a tenure and a vista.
Each entry may name a subject and an id, and a missing one matches every bulletin.
The roster takes no lock, so each owner keeps its own gate around every call.
Owners copy the list under their gate and dispatch outside it.
So an observer can attach or detach while a dispatch runs without disturbing it.
The optional filter is asked again for every entry a bulletin would reach.
So state it reads counts as of that call.
A vista passes a filter to reach its chosen observers only for the chosen row.
Attach appends, so dispatch reaches observers in the order they were attached.
Check lets the engine refuse a second attach of one observer.
Detach removes the first matching entry and does nothing when none matches.
Clear drops every entry, so nothing attached before is reached again.
Dispatch calls entries in snapshot order, so an earlier observer may change state a later one reads.

## `LCatalogStaff`

The group of clerks for the catalog rows a card, a mention or a citation names.
It is built first, since it reads no other group.
The translation clerk stamps through the shared revision clerk, so history stays one list.
The example clerk reads through the reference clerk built beside it.

## `LClaimStaff`

The group of clerks for held drafts, their history and their claims.
It is built after the catalog group.
The court reads the catalog's translation clerk, so a pending link resolves through translations.
The court manages links from a held draft to a target not yet stored.
The draft, court and claim clerks share the one identity issuer, so no id is issued twice.

## `LEngineStaff`

The record keeps every clerk and engine helper that belongs to the current rig.
The clerks sit in five group records, each one concern a reader finds by name.
The engine swaps one record when the workspace changes.
Build makes the staff in dependency order over one rig.
It makes the identity issuer, the language cache and the revision clerk the groups share.
Each group is built after every group it reads a clerk from.
The identity issuer receives the stale ids, so the new workspace never issues one a tenure still holds.
One revision clerk stamps for the translation, entry, citation and intake clerks, so history stays one list.

## `LEntryStaff`

The entry lifecycle, search, commit, citation and fold clerks share this group.
It is built after the catalog, claim and language groups.
The card and inflection clerks are built here and kept only inside the meaning and entry clerks.
The entry and citation clerks stamp through the shared revision clerk, so history stays one list.
The fold clerk keeps card folds, per-entry "More readings" state and editor-box state.
The ancestor clerk keeps the relation and tongue lists, and the lineage clerk reads an entry's etymology tree.

## `LLanguageStaff`

The group of clerks for language data, pronunciations and every background fetch.
All six fetching clerks sit here, so one call cancels them together.
It is built after the claim group.
Fetch clerks receive the gate and the bulletin raiser supplied by the engine.
The frequency and lacuna fetches also receive the settings reader.
The fetch clerks also receive one fault owner, which records a background fault in the audit log.
The paradigm clerk receives the language cache, so it reads the same rule books as the lacuna fetch.
The recording, reflex and lacuna clerks read the claim clerk, so they see which drafts this engine holds.
The stem clerk receives the reflex clerk, so a series member orders its reflex rows by the pack's rule.
It also receives the language clerk, so a series member reads its guises from their one owner.
Apply derives the 音韻地位 categories again from the stored fanqie rows and the hypothesis on disk.
Then it builds again the phonetic series of every pack that declares a series source.
A bound workspace needs both before the first read.
Clear cancels every pending fetch of the six fetching clerks, on a rig apply and on dispose.
The reflex fetch is reached through the reflex clerk that holds it.

## `LWorkspaceStaff`

The group of clerks for workspace state, markup, portraits, print settings and the Joplin push.
It is built last, since it reads every other group.
The markup entry clerk receives the reflex clerk, so an exported entry lists its reflexes in the pack's order.
One markup ancestor clerk serves both the entry and the draft clerk, so both translate an etymology map alike.
The portrait clerk receives the entry group's fold clerk, so a portrait reads each card's stored fold.
It also receives the lineage and ancestor clerks, so a portrait draws the etymology band.
The markup link clerk is built here and shared by the intake, its draft clerk and the ancestor clerk.
The courier records its failures through the workspace clerk, so a failed push leaves a trace.
The courier takes no portrait clerk, since `LCourierFacade` hands it each Livery page at send time.
The courier searches through the query clerk, never through the entry lifecycle.
The deck, cue and stencil clerks are built here for `LDeckFacade`.
The deck clerk receives the trail clerk, which normalizes a name before the deck code is derived.
The cue clerk receives the entry clerk, which loads each entry an add scans.
It receives the translation, reference and trail clerks too, which the listing line reads.
The stencil clerk receives the entry, translation, reference and trail clerks its fields read.
The depot latch is built once per workspace and handed to the depot clerk.
The depot clerk shares the stencil clerk, so an export reads the faces the deck view shows.
It records its failures through the workspace clerk, as the courier does.
The depot latch is built here once per workspace, so a busy export blocks only its own workspace.
A workspace switch rebuilds the staff, so the new workspace starts with an idle latch.

## `LEngine`

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
Each facade but the tenure facade takes the hearth and its sibling facades, never the engine.
`LEngineStaff` keeps the clerks in dependency order and swaps them together when the rig changes.

## `LEngine(LRig rig, Func<string, LRig> factory, Action<string> pointer)`

Binds the engine to the ports of `rig`, already built over the workspace they stand on.
It neither reads nor rewrites the recorded workspace pointer.
Host hands `factory`, which builds a rig over a folder, and `pointer`, which records the chosen folder.
Both stay outside, since the engine names no infrastructure, and pass on to the workspace facade.
The hearth comes first, building the clerks and reading the settings.
The row facade follows it, so every facade that builds vista rows finds it.
The facades come last, so none of them can ever see an engine without its staff.
Each facade is built after the siblings it takes, so its constructor needs no engine.
The deck facade takes the entry and row facades, so its accession find lists catalog rows.
The stem and reflex facades take no settings facade, since the language clerk owns every row's guise.
The tenure facade comes last and alone takes the engine, since its tenures reach every facade.
The workspace opens at the very end, after the workspace facade has created the database.

## `LEngineVista`

It owns the vista registry and serves the entry rows shown by each catalog tab.

## `LEngineCatalog`

The facade for the catalog shelves: tags, registers and favorites.
It is built right after the row facade, since it takes no other sibling.

## `LEngineVistaRow`

The row builder every catalog of entries uses, so no facade reaches the vista facade only to build rows.

## `LEngineTenure`

The facade that starts and commits draft holds, each driven by an `LTenure`.

## `LEngineLivery`

The facade for the Joplin page read.
`LCourierFacade.LEngineCourierSend` hands its page read to the Joplin push.

## `LEnginePortrait`

The facade for printing and exporting portraits.
It is built after the Livery facade, since an entry export reads the Livery page.

## `LEngineScript`

The facade for script images, built just before the language facade, which starts their fetch.

## `LEngineRequest`

The facade that applies one edit request at a time to a held draft.

## `LEngineReflex`

The reflex, stem, fanqie and vocabulary facades are public, since Host hands them to Conduct as ports.
Vocabulary serves the paradigm and sentence ports, and fanqie serves the fanqie and diwei ports.

## `LEngineHearth`

The shared state every facade reads, built before any facade so none sees an engine without staff.

## `Dispose`

Cancels every pending fetch, so a closed engine leaves no task writing into the workspace.
`LEngineHearth.LEngineStaffClear` does it, since the hearth owns the gate and the staff.

## `LEngineHearth.cs`

The engine's shared state: the gate, the staff, the settings, the trove, the stale drafts and the observers.
Every facade reads it, so no facade needs the engine itself for state.
It swaps the staff as one unit when the engine moves onto another workspace.
It builds the clerks over the rig, then reads the settings through them.
It leaves the workspace closed, since the workspace facade creates the database first.
The gate is the shared lock every facade and fetching clerk uses for engine state.
Callers hold it while reading or changing the state.
The trove is the engine session cache, shared with the facades that read and clear it.
The stale draft ids were claimed in a former workspace, so a surviving tenure cannot write into the new one.
The revision is a count that moves on every announced change.
So a tenure knows when its kept state went stale.
A facade that writes without announcing moves it by hand, so no kept state outlives the write.
The staff record changes as one unit when `LEngineRigApply` moves to another workspace.
It is read under the gate, so a caller outside the gate still sees one whole record.
The settings reader exists before any facade does, since the fetch clerks hold it.

## `LEngineWorkspaceOpen`

The steps a bound workspace needs before the first read.
The controlled vocabularies come from the language packs on disk and are written into the workspace.
The vocabulary clerk imports them directly, so the hearth needs no facade.
`LEngine` calls it once its facades exist, and `LEngineRigApply` calls it after the swap.
The 音韻地位 categories are derived again from the stored fanqie rows and the hypothesis on disk.
The phonetic series of every pack that declares a series source are built again.
`LLanguageStaff.LLanguageStaffApply` runs those two steps on the language group.
Stored respellings derived by older pack rules are then derived again through the workspace clerk.
A workspace rebuilt from an older schema has its derived strings filled once through the workspace clerk.
A failure on one entry is recorded in the audit log, and the scan goes on with the next entry.

## `LEngineRigApply`

It moves the engine's clerks, caches and settings onto the workspace `rig` was built over.
`LWorkspaceFacade.LEngineRigApply` calls it under the gate, keeps the folder and the rescue, then raises the bulletin.
The new rig's settings are read through the clerk's static open step before anything here changes.
A folder that cannot be opened therefore leaves the old clerks and caches untouched.
A workspace that already holds a settings file is opened on its own settings.
One without any receives the current settings and has them written.
That write goes through the clerk's `LWorkspaceFallbackSave`, which records a vault fault and hands it back.
The engine needs no use for the fault.
So the switch never stops halfway, and the next settings save tries again.
The drafts this engine claimed are forgotten with the old folder and each id is marked stale.
The old recording clerk stops its playback, since the shared phonograph outlives it.
The pending fetches of the old clerks are cancelled, and the flag cache and the trove are cleared.

## `LEngineStaffClear`

It cancels every pending fetch of the staff held now.
So a closed engine leaves no task writing into the workspace.
`LLanguageStaff.LLanguageStaffClear` does the cancelling, since the language group holds every fetching clerk.
It runs under the gate, so a workspace switch cannot swap the staff halfway.

## `LEngineObserverAttach`

An already attached delegate is not added a second time.
It names no subject or id, so it hears every announcement.
Detach stops announcing, so a closed surface is never called again.

## `LEngineBulletinRaise`

It announces one stored change to every subscriber.
A bulletin may carry a refusal, which only a refused queued draft request sets.
The revision moves first, even with nobody listening, because a tenure reads it whether or not it subscribed.
The `LBulletinRoster` is copied under the gate, then dispatched outside it.
So a subscriber can detach without disturbing iteration.

## Stamp

Commit: `2f9a06b8eb95`
- `LBulletinRoster.cs` `089b003a16e92876`
- `LCatalogStaff.cs` `23816a2bb79aced1`
- `LClaimStaff.cs` `fc2ed893e2af6438`
- `LEngine.cs` `70e0ba46ed5cc982`
- `LEngineHearth.cs` `17ccd1f0c9342b91`
- `LEngineStaff.cs` `1282514bb49cde5e`
- `LEntryStaff.cs` `769d85bf0ae2afca`
- `LLanguageStaff.cs` `dcce9d9280b1adad`
- `LWorkspaceStaff.cs` `06182aadb64ca9b8`
