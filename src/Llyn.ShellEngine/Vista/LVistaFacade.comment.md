# LVistaFacade.cs
Hash: `084e072a233f0993`

## `public sealed class LVistaFacade : LFavoritePort, LVistaPort`

Where a panel asks for a vista, the engine's view state for one catalog tab.
It is also where the panel asks for the rows that vista lists.
No browse panel keeps its own order and filter fields or applies them itself.
The engine holds the choices and returns rows already filtered, sorted, numbered and marked.
The facade keeps the vista registry and delegates stored data to the engine's other facades and staff.
It implements the favourite and vista ports itself, so Host hands it to Conduct with no outlet between.

## `private readonly LEngine _lVistaFacadeEngine`

The engine this facade belongs to, for shared state, other facades and observer calls.

## `private readonly object _lVistaFacadeGate`

The engine gate shared with every other facade.

## `private long _lVistaFacadeCount`

The ids handed out so far, so each vista's bulletin names one vista.

## `private readonly Dictionary<string, LVista> _lVistaFacadeTabs = []`

The vista now standing for each tab, so a restart can detach the one it replaces.

## `public LVistaFacade(LEngine engine)`

Binds this facade to its engine and the shared gate.

## `public LVista LEngineVistaStart(string tab, LSubject? subject, LCatalogOrder order, LCatalogFilter filter, bool editing, bool blank = false)`

Starts a vista for `tab` on the order, filter and mode the caller hands it.
The shell's posture resolves those from what it stored, so the engine holds no view state of its own.
`subject` names the stored kind the tab lists, so the engine holds no table of panel names.
`blank` makes an empty query list nothing, which only the duplex wings ask for.
The vista is attached as an observer before it is returned, so it carries bulletins to the panel.
The vista started earlier for the same tab is detached first, so a switched workspace leaves no ghost.

## `public LVista? LEngineVistaRead(long id)`

The vista a bulletin names, or nothing once a restart replaced it.
The posture asks after each vista bulletin, so it stores the order and filter the vista now holds.

## `public IReadOnlyList<LVistaRow> LEngineEntryFind(LVista vista)`

The entry rows the vista lists, ready to show.
The query, order and filter are the vista's, applied by the three-argument find.
A blank vista with nothing typed answers no rows before the lock is taken.
The rows are built by the shared builder under one lock.

## `public IReadOnlyList<LVistaRow> LEngineEntryFind(LVista? parent, LVista? child)`

The entries the record chosen in a catalog vista reaches, listed in the child vista beside it.
The parent's language filter applies, since a catalog's filter hides languages from its chosen record's entries.
The child's query narrows the rows, and the child's chosen row is marked.
The query clerk's match filters, narrows and orders the rows by the child's ordering, as the catalog list does.
So equal headwords go by the entry tie rule, never by the order the store returned them.
A missing vista, or a parent subject that reaches no entries, answers no rows.

## `internal IReadOnlyList<LVistaRow> LEngineVistaBuild(IReadOnlyList<LEntry> entries, long? chosen)`

Turns entries in their listed order into rows ready to show: twin name, epithet and chosen mark.
An entry with no epithet carries an empty one, so no reader of a row falls back on its own.
Twins are numbered by entry id, so the older entry is `(1)` in every view.
Only entries of one language are twins.
The epithets come from one scan, so a long list costs one statement rather than one session per row.
The chosen row is the one whose id equals `chosen`.
Every catalog of entries builds its rows here, so no panel numbers twins or reads epithets itself.
It takes the lock for the epithet scan.

## `public IReadOnlyList<string> LEngineNameResolve(IReadOnlyList<string> labels)`

The labels made distinct in their given order, numbered where two share a name, for the compass rows.

## `private static string[] LEngineTwinRead(IReadOnlyList<LEntry> entries)`

The twin name of each entry, by position, numbered by entry id, through `LEntryClerkTwin`.

## `internal static string[] LEngineTwinRead<LEngineRow>(IReadOnlyList<LEngineRow> rows, Func<LEngineRow, string> name, Func<LEngineRow, long> id)`

The twin name of each row, by position, numbered by row id, through `LEntryClerkTwin`.
Those rows carry no language, so one empty group serves all of them.

## `internal static string LEngineNameRead(LStateValue value, string unknown, string fallback)`

The name a state value shows, through `LEntryClerkTwin`.
An unknown state reads as `unknown`, and a value that shows nothing reads as `fallback`.

## `private IReadOnlyDictionary<long, string> LEngineEpithetScan(IReadOnlyList<LEntry> entries)`

The epithet of every listed entry that has one, keyed by id, or nothing while the workspace hides epithets.
Called under the lock.

## `public IReadOnlyList<LCatalogFavorite> LEngineFavoriteFind(string query, LCatalogOrder order, LCatalogFilter filter)`

The marked entries matching the query in the given order, with those in a hidden language left out.

## `public IReadOnlyList<LVistaRow> LEngineFavoriteFind(LVista vista)`

The rows the favorites panel's vista lists, with the query, order and filter read off the vista.
They come back as vista rows, twins numbered and epithets read in one scan, so the panel only copies them.
The row of the entry the vista stands on comes back marked chosen, so the panel keeps no choice.

## `public bool LEngineFavoriteCheck(long entryId)`

Whether the entry is marked.

## `public void LEngineFavoriteSave(long entryId)`

Marks the entry a favorite and announces the mark.
Marking changes no lexical data and keeps the entry's identity.

## `public void LEngineFavoriteDelete(long entryId)`

Unmarks the entry and announces it.
The entry stands, still reachable through the entry catalog.

## `public LDraft? LEngineVistaLoad(LVista vista)`

Loads the vista's selected record according to its subject under the engine gate.
The result is a snapshot, with no claim file, editing identity, or registered tenure.
Reference snapshots include credits.
Tag and register loads carry no record, only the proof the chosen row still exists.
A stored choice that no longer loads is dropped here, so a panel never branches on the missing answer.

## `public LDraft? LEngineVistaLoad(LVista vista, long? id)`

Selects the row and loads it, restoring the previous choice when the load throws.
So a failed click leaves the panel where it stood, and no caller keeps the prior choice.

## `public string LEngineFileRead(LVista? vista)`

The file name an export of the vista's entry is offered under is the headword with barred characters replaced.
The engine's settings facade says which characters are barred, so the vista reads no file rule itself.
A missing vista, or a blank headword, is offered as `entry`.
A failed load is not caught here.
It travels up to the export gate.

## `public void LEngineSideSave(LVista vista)`

Writes the chosen entry to the left or right slot of the workspace state, whichever side the vista's tab names.
The next run reopens each duplex side on the entry it last showed.
The vista itself says which side it is, so no caller copies that.

## `public int LEngineUsageRead(LVista vista)`

How many places the vista's chosen stored record reaches, which a delete would drop.
An Example, a Situation and a Source count the entries that cite them.
An Author counts the Sources crediting it.
Any other subject, or no stored choice, reaches nothing.

## `public string LEngineTallyRead(LVista vista)`

The citation line of the vista's chosen stored record, ready to show.
An Example, a Situation and a Source have one, and any other vista refuses.
No stored record reads as cited nowhere.

## `public LRevision? LEngineVistaDelete(LVista vista)`

Deletes the vista's selected record of its subject and clears the selection afterwards.
Only an entry delete records a revision, so every other subject answers null after deleting.
A catalog record is detached from every owner first, as the panel already confirmed the usage.
Tag, register and structural vistas delete nothing and answer null.
