# LVistaFacade.cs
Hash: `c3d5c4901896ca7f`

## `public sealed class LVistaFacade : LVistaPort`

Where a panel asks for a vista, the engine's view state for one catalog tab.
It is also where the panel asks for the rows that vista lists.
No browse panel keeps its own order and filter fields or applies them itself.
The engine holds the choices and returns rows already filtered, sorted, numbered and marked.
The facade keeps the vista registry and delegates stored data to the engine's other facades and staff.
It implements the vista port itself, so Host hands it to Conduct with no outlet between.
Favorites live in `LCatalogFacade`, beside Tags and Registers.

## `private readonly LEngineHearth _lVistaFacadeHearth`

The hearth this facade shares, for the gate and the observer calls.
The sibling facades and the row builder are held beside it.

## `private readonly object _lVistaFacadeGate`

The engine gate shared with every other facade.

## `private long _lVistaFacadeCount`

The ids handed out so far, so each vista's bulletin names one vista.

## `private readonly Dictionary<string, LVista> _lVistaFacadeTabs = []`

The vista now standing for each tab, so a restart can detach the one it replaces.

## `internal LVistaFacade(LEngineHearth hearth, LAuthorFacade author, LCatalogFacade catalog, LEntryFacade entry, LExampleFacade example, LReferenceFacade reference, LSettingsFacade settings, LSituationFacade situation, LWorkspaceFacade workspace, LVistaRowFacade row)`

Stores the hearth, its gate and the sibling facades it calls, all built by `LEngine` before this one.
The gate, the staff and the shared state are read through the hearth.
It takes its siblings rather than the engine, so it names only the facades it uses.

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
The entry facade probes the parent's chosen record by subject and id.
A missing vista, or a parent subject that reaches no entries, answers no rows.

## `public IReadOnlyList<string> LEngineNameResolve(IReadOnlyList<string> labels)`

The labels made distinct in their given order, numbered where two share a name, for the compass rows.

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
