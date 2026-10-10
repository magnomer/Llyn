# CEditor.cs
Hash: `9ebd50b80b7ba6f1`

## `public sealed class CEditor`

An editor session composes every area over one desk, keeping drivers behind shared Conduct gates.
It opens, stores and drops drafts without exposing its ports to drivers.

## `private LVista? _cEditorVista;`

The restored vista identifies whether this editor belongs to the input tab.

## `private bool _cEditorFresh;`

The pre-finish verdict distinguishes a fresh input save from saving an existing entry.
That distinction determines what reopens after storage.

## `internal CEditor(LDraftPort drafts, CEntryBundle entries, CPhonologyBundle phonology, LSettingsPort settings, LMediaPort media, CEnvoy envoy, CLedgerNoticed noticed, Action<Action> marshal)`

Every desk uses the `Input` notice scope, regardless of its tab.
Each child receives only its required ports, while repaint-read failures share the supplied notice memory.
The desk's finish handler precedes the entry area's draft subscription.
Listening areas receive the marshal, so engine notifications need not depend on driver-owned thread dispatch.
Settings notifications resonate the held draft because its presentation depends on settings.

## `public static CEditor CEditorCreate(CAtelier atelier, CEnvoy envoy)`

Each editor receives its own session over the atelier's shared dependencies.
Display navigation and sounding navigation reach the atelier rather than the driver.
Workspace opening replaces the held entry with a fresh draft.

## `public CCardSpeech CEditorSpeech { get; }`

One speech area follows the shared desk across drafts.

## `public CCardField CEditorField`

A stateless card-field area can be built afresh over the same desk.

## `public CCardList CEditorList { get; }`

One card-list area owns add, remove, move and fold gates over the shared desk.

## `public CImage CEditorImage`

A stateless image area can be built afresh over the same desk.

## `public CVideo CEditorVideo`

A stateless video area can be built afresh over the same desk.

## `public void CEditorClose()`

Closing cancels the held desk session before cancelling its errands.

## `public CDesk CEditorDesk { get; }`

Every editor area shares one desk and therefore one held draft.

## `public CEntry CEditorEntry { get; }`

The entry area supplies head-field gates and ready draft notifications.

## `public CDisplay CEditorDisplay { get; }`

The display area owns shared display rules used by the editor's esteem, sounding and kindred areas.

## `public CCard CEditorCard { get; }`

Card gates operate on the editor's shared desk.

## `public CSentence CEditorSentence { get; }`

Sentence gates share the desk, envoy and repaint-read notice memory.

## `public CSounding CEditorSounding { get; }`

Sounding shares the display rules and envoy, keeping navigation and failure presentation within the session.

## `public CFold CEditorFold { get; }`

Rime-book and script box states are stored per entry through the reflex port.
It is built over the desk's stored entry, which it asks on every read and write.
The fold area listens to nothing itself.
`CEntry` supplies draft notifications when that entry's fold bulletin arrives.

## `public CEsteem CEditorEsteem { get; }`

Esteem reads the shared desk's stored entry through the display rules.

## `public CTimbre CEditorTimbre { get; }`

Timbre supplies pack sound facts for the desk's held entry.

## `public CKindred CEditorKindred { get; }`

Kindred shares the desk and display rules for reflex lookup and row edits.

## `public CPlayback CEditorPlayback { get; }`

Playback associates media actions with the same held entry.

## `public CTranscription CEditorTranscription`

A stateless transcription area can be built afresh over the same desk.

## `public bool CEditorOwned`

Only an input vista makes this editor owned.
An unrestored vista answers false.

## `internal void LEditorVistaRestore(LVista vista)`

The desk and display receive the same restored vista, keeping their tab context aligned.

## `public void CEditorEntryOpen(long? id)`

Null opens a fresh draft.
A requested entry that leaves no held draft falls back to a fresh draft.

## `public void CEditorEntryUndo()`

Undo reopens the draft's stored entry, or a fresh draft when none exists.

## `public void CEditorEntrySave()`

An unchanged draft is not finished for storage.
A changed draft uses the same finish path as leaving.

## `internal bool LEditorFinish(bool store)`

The fresh-entry verdict is captured before finishing, so post-store reopening uses the draft's former identity.
The desk's finish verdict is returned unchanged.

## `private void CEditorStoredShow(long id)`

A freshly stored input draft reopens blank.
Every other successful store reopens the stored entry.
