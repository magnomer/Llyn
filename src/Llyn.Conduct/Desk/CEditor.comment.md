# CEditor.cs
Hash: `f6d414aa75e09f0c`

## `public sealed class CEditor`

The entry editor's session holds the desk it sits at and every area over that desk.
It opens an entry or a fresh draft, stores and drops typing.
Its entry area hands each head field edit to a quill over the tenure.
The atelier builds it over its ports, so no driver holds a port for the editor.
Every driver edits an entry through the same gates.

## `private LVista? _cEditorVista;`

The vista of the tab the editor serves, which says whether it is the input tab's.

## `private bool _cEditorFresh;`

Whether the draft being stored stood on no entry, written just before each finish.
A fresh store on the input tab reopens a blank draft, while every other store reopens what was stored.

## `internal CEditor(LDraftPort drafts, CEntryBundle entries, CPhonologyBundle phonology, LSettingsPort settings, LMediaPort media, CEnvoy envoy, CLedgerNoticed noticed, Action<Action> marshal)`

Every editor's desk takes the `Input` scope, so its failure notices use the input wording on any tab.
It takes the atelier's repaint memory, which nothing else here can reach, and widens by that one argument.
The display and the sentence gates show their repaint read failures through it.
The sound facts take `envoy` too, so a failed flag load shows its notice.
The reflex block takes it as well, so a refused anchor read shows its notice.
It reads the entry and phonology bundles and hands each child only the ports it calls.
It hands the marshal once, last, to the desk and each area that listens, so no driver attaches one.
Each area then raises its own change event on the UI thread when the engine announces its subject.
The folds hear the settings port's own fold event rather than a subject.
A settings change reads the held draft again, since the draft's shown form depends on the settings.

## `public static CEditor CEditorCreate(CAtelier atelier, CEnvoy envoy)`

Builds an entry editor over the atelier's ports, repaint memory and marshal, asking its questions through `envoy`.
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

## `public void CEditorClose()`

The editor closes with its view.
The folds' handler leaves the engine event first, so a closed editor hears no other editor's fold.
The held draft is let go, then every running errand is cancelled.
Closing is one user action, and the order of the cancels is the editor's.

## `public CDesk CEditorDesk { get; }`

The desk this editor sits at, built over the `Input` scope.
Every area of the editor shares it, so all of them edit the same held draft.

## `public CEntry CEditorEntry { get; }`

The head fields of the entry the editor's desk holds, with their gates and the draft's change event.
It is built right after the desk's finish handler, so it hears each prepared draft in the former order.

## `public CDisplay CEditorDisplay { get; }`

The display area of the editor's entry, holding the favourite, grasp and sound facts.
It is the C area, so a lectern over the editor never names the rules under it.
The esteem, the sound sheet and the reflex block take those rules from it.

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

## `public CKindred CEditorKindred { get; }`

The reflex block of the entry the editor's desk holds, with its lookup and row edits.
It is built right after the sound facts, so its lookup starts on the same draft prepare.

## `public CPlayback CEditorPlayback { get; }`

The recordings of the entry the editor's desk holds, which its play buttons and tray open.

## `public CTranscription CEditorTranscription`

The transcription rows of the entry the editor's desk holds, with their gates.
It holds nothing but the desk, so each read builds a fresh one over it.

## `public bool CEditorOwned`

Whether this editor is the input tab's, which alone shows the command rail.

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
