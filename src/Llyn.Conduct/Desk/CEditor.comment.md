# CEditor.cs

## `public sealed class CEditor`

The entry editor's session: the desk it sits at and every entry-level gate and read.
It opens an entry or a fresh draft, stores, drops typing, and hands each field edit to the tenure.
The atelier builds it over its ports, so no driver holds a port for the editor.
Every driver edits an entry through the same gates.

## `private LVista? _cEditorVista;`

The vista of the tab the editor serves, which says whether it is the input tab's.

## `private bool _cEditorFresh;`

Whether the draft being stored stood on no entry, written just before each finish.
A fresh store on the input tab reopens a blank draft, while every other store reopens what was stored.

## `public static CEditor CEditorCreate(CAtelier atelier, CEnvoy envoy)`

Builds an entry editor over the atelier's ports, asking its questions through `envoy`.
Each tab that edits entries holds its own, so no driver ever holds a port for it.
Building a session is composition, not a user action, so it is no gate.

## `public event Action<CEntryDraft>? CEditorDraftChanged;`

The held entry was read again, so a driver writes its controls from it.
It carries the content the desk prepared, never a second read.

## `public LDisplay CEditorDisplay { get; }`

The display of the editor's entry, holding the favourite, grasp and sound facts.

## `public CCard CEditorCard { get; }`

The card gates over the editor's desk.

## `public CSentence CEditorSentence { get; }`

The sentence gates over the editor's desk.

## `public CSounding CEditorSounding { get; }`

The sound sheet of the entry the editor's desk holds.
It shares the editor's envoy, so a refused rebuild shows the same notice as the desk's own failures.

## `public CEsteem CEditorEsteem { get; }`

The favourite, grasp and frequency of the stored entry the editor's desk holds.

## `public CTimbre CEditorTimbre { get; }`

The pack sound facts and waiting sections of the entry the editor's desk holds.

## `public bool CEditorOwned`

Whether this editor is the input tab's, which alone shows the command rail.

## `public string CEditorOrigin`

The tab the editor serves, which a link court records as its origin.

## `private LTenure? CEditorTenure`

The held tenure for a field edit, or null while none is held or the desk fills its controls.

## `internal void CEditorVistaRestore(LVista vista)`

Binds the desk and the display to the tab's vista.

## `public void CEditorEntryOpen(long? id)`

Opens the entry, or a fresh draft when the id is null.
An entry that cannot be opened falls back to a fresh draft, after the failure has been announced.

## `public void CEditorEntryUndo()`

Drops the typing by reopening the entry the draft stood on, or a fresh draft.

## `public void CEditorEntrySave()`

Stores the held draft when it changed, and reopens what was stored once the desk announces it.

## `public bool CEditorFinish(bool store)`

The window's leave: stores or drops the held draft and reports whether the tenure ended.
A stored draft is reopened the way a save reopens it.
A hidden tab then comes back showing what it stored.

## `private void CEditorStoredShow(long id)`

Chooses what opens after a store: a blank draft for a fresh input draft, else the stored entry.

## `public CEntryDraft? CEditorDraftRead()`

The held draft's content as the tenure last read it, applying nothing first.

## `public IReadOnlyList<CReflexHead> CEditorLeadRead(long reflex, string language)`

Every reflex row's lead, while `language` is typed into row `reflex`.
The held draft's rows are read in order, with the typed language standing in for the row's stored one.
The lead rule is `CReflex.LReflexLeadRead`, the one the scan marks its rows by.
Only the edit in hand is overlaid, so another row's edit still deferred reads as stored.

## `public string CEditorPronunciationRead()`

The primary reading as the field shows it, respelled when the pack respells.

## `public IReadOnlyList<CTranslationTarget> CEditorEtymonRead()`

The etymons of the held entry, as the etymology field lists them.

## `public IReadOnlyDictionary<long, CTranslationTarget> CEditorTargetRead()`

The link targets of the held draft, keyed by entry, for the cards' translation chips.

## `public void CEditorPronunciationSet(string text)`

Hands the typed reading to the tenure, which writes it the way the pack shows it.

## `public void CEditorVarietySet(bool primary, long pronunciation, string variety)`

Names the variety of the reading a menu filled, after the reading itself was written.
A menu opened on the primary reading targets whatever primary the draft now holds.
Any other menu targets the accent row it was opened on.
