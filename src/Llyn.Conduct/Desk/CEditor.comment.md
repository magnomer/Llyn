# CEditor.cs
Hash: `1442444288029fac`

## `public sealed class CEditor`

The entry editor's session holds the desk it sits at and every entry-level gate and read.
It opens an entry or a fresh draft, stores, drops typing, and hands each field edit to the tenure.
The atelier builds it over its ports, so no driver holds a port for the editor.
Every driver edits an entry through the same gates.

## `private LVista? _cEditorVista;`

The vista of the tab the editor serves, which says whether it is the input tab's.

## `private bool _cEditorFresh;`

Whether the draft being stored stood on no entry, written just before each finish.
A fresh store on the input tab reopens a blank draft, while every other store reopens what was stored.

## `internal CEditor(LDraftPort drafts, LEntryPort entries, LPhonologyPort phonology, LSettingsPort settings, LMediaPort media, CEnvoy envoy, CLedgerNoticed noticed)`

Every editor's desk takes the `Input` scope, so its failure notices use the input wording on any tab.
It takes the atelier's repaint memory, which nothing else here can reach, and widens by that one argument.
The display, the sentence gates and the sound sheet show their repaint read failures through it.
The sound facts take `envoy` too, so a failed flag load shows its notice.

## `public static CEditor CEditorCreate(CAtelier atelier, CEnvoy envoy)`

Builds an entry editor over the atelier's ports and repaint memory, asking its questions through `envoy`.
Each tab that edits entries holds its own, so no driver ever holds a port for it.
Building a session is composition, not a user action, so it is no gate.
The editor's display and sounding open what they are asked to through the atelier's navigation.
Every opened workspace opens a fresh draft in the editor, so no editor keeps the former workspace's draft.

## `public CCardSpeech CEditorSpeech { get; }`

The draft's parts of speech as the user builds them, kept across the editor's drafts.

## `public CCardField CEditorField`

The typed fields of the held draft's cards, built fresh over the desk since it keeps no state.

## `public CCardList CEditorList`

The gates that add, remove and move the held draft's cards, built fresh over the desk.

## `public CImage CEditorImage`

The image rows of the cards, built fresh over the desk.

## `public CVideo CEditorVideo`

The video rows of the cards, built fresh over the desk.

## `public string CEditorLanguage`

The held draft's language, or empty while no draft is held.

## `public void CEditorObserverAttach(Action<Action> marshal)`

Hands every area of the editor the driver's marshal once.
Each area then raises its own change event on the driver's thread when the engine announces its subject.
The folds hear the settings port's own fold event rather than a subject.
A settings change reads the held draft again, since the draft's shown form depends on the settings.

## `public void CEditorClose()`

The editor closes with its view.
The folds' handler leaves the engine event first, so a closed editor hears no other editor's fold.
The held draft is let go, then every running search.
Closing is one user action, and the order of the cancels is the editor's.

## `public event Action<CEntryDraft>? CEditorDraftChanged;`

The held entry was read again, so a driver writes its controls from it.
It carries the content the desk prepared, never a second read of the draft.
The tenure resolves the cards' link targets for that same content, so every card paints its links ready.

## `public CDisplay CEditorDisplay { get; }`

The display area of the editor's entry, holding the favourite, grasp and sound facts.
It is the C area, so a lectern over the editor never names the rules under it.
The esteem, the sound sheet and the sound facts take those rules from it.

## `public CCard CEditorCard { get; }`

The card gates over the editor's desk.

## `public CSentence CEditorSentence { get; }`

The sentence gates over the editor's desk.

## `public CSounding CEditorSounding { get; }`

The sound sheet of the entry the editor's desk holds.
It shares the editor's envoy, so a refused rebuild shows the same notice as the desk's own failures.

## `public CFold CEditorFold { get; }`

The open state of the rime-book and script boxes, which the user keeps across entries.
It shares the editor's settings port and envoy, so a failed save shows like the desk's own failures.
The editor puts its engine handler back on open and takes it off on close.

## `public CEsteem CEditorEsteem { get; }`

The favourite, grasp and frequency of the stored entry the editor's desk holds.

## `public CTimbre CEditorTimbre { get; }`

The pack sound facts and waiting sections of the entry the editor's desk holds.

## `public CPlayback CEditorPlayback { get; }`

The recordings of the entry the editor's desk holds, which its play buttons and tray open.

## `public CTranscription CEditorTranscription`

The transcription rows of the entry the editor's desk holds, with their gates.
It holds nothing but the desk, so each read builds a fresh one over it.

## `public bool CEditorOwned`

Whether this editor is the input tab's, which alone shows the command rail.

## `private LTenure? CEditorTenure`

The held tenure for a field edit, or null while none is held or the desk fills its controls.

## `internal void LEditorVistaRestore(LVista vista)`

Binds the desk and the display to the tab's vista.
Only the Conduct areas call it as they restore their vistas, so it is a helper.

## `public void CEditorEntryOpen(long? id)`

Opens the entry, or a fresh draft when the id is null.
It puts the folds' handler back first, so an editor reopened after a close hears the folds again.
An entry that cannot be opened falls back to a fresh draft, after the failure has been announced.

## `public void CEditorEntryUndo()`

Drops the typing by reopening the entry the draft stood on, or a fresh draft.

## `public void CEditorEntrySave()`

Stores the held draft when it changed, and reopens what was stored once the desk announces it.

## `internal bool LEditorFinish(bool store)`

The quit's and the leave's finish.
It stores or drops the held draft and reports whether the tenure ended.
A stored draft is reopened the way a save reopens it.
A hidden tab then comes back showing what it stored.

## `private void CEditorStoredShow(long id)`

Chooses what opens after a store.
A fresh input draft opens a blank draft, and any other store opens the stored entry.

## `public string CEditorPronunciationRead()`

The primary reading as the field shows it, respelled when the pack respells.

## `public IReadOnlyList<CTranslationTarget> CEditorEtymonRead()`

The etymons of the held entry, as the etymology field lists them.

## `public void CEditorHeadwordSet(string text)`

Hands the typed headword to the tenure.
A desk that holds no tenure or fills its view takes no edit.

## `public void CEditorNoteSet(string text)`

Hands the typed note to the tenure.
A desk that holds no tenure or fills its view takes no edit.

## `public void CEditorLanguageSet(string language)`

Hands the chosen language to the tenure.
A desk that holds no tenure or fills its view takes no edit.

## `public void CEditorUnitSet(string key)`

The user picked a unit from the menu.
Picking the unit already held clears it, a rule the tenure keeps.
The menu itself is read with the part-of-speech field, through `CMarkerUnits`.
A desk that holds no tenure or fills its view takes no edit.
