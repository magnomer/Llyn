# LEditor.cs

## `public sealed class LEditor`

The entry editor's spine as a deportment: the desk it sits at, and every entry-level fact and command.
It opens an entry or a fresh draft, stores, resets, and forwards the headword, reading, note and language.
The favourite and grasp marks and the sound facts sit on its `LEditorEsteem` and `LEditorTimbre`.
The card lookups go through the card deportment, and the sound rows ask the atelier's catalog for pack facts.
The origin of a draft is the tab of the vista handed in, so the deportment holds no origin string.

## `private bool _lEditorFresh;`

Whether the draft being stored stood on no entry, written once before the store and read through its verdict.
A fresh store on the input tab reopens a blank draft, while every other store reopens what was stored.

## `private bool _lEditorHalted;`

Whether the tenure had stopped at the last state notice, so the stop is announced once.

## `public event Action? LEditorStateChanged;`

The desk's state came or went, so the store, reset and chronicle buttons are read again.

## `public event Action? LEditorStopped;`

The tenure stopped taking requests, announced once so the window can say so.

## `public event Action<string, Exception>? LEditorFailed;`

A favourite, grasp or rime-book write was refused, announced under its own notice key.
The display and the sound sheet pass their refusals on here, so the window listens once.

## `public LDisplay LEditorDisplay { get; }`

The Conduct display of the editor's entry, which the editor view wraps in its own lectern.
The editor holds no lectern, since a lectern names WPF and a sealed controller names none.
It holds the favourite and grasp rules the esteem forwards to.

## `public CCard LEditorCard { get; }`

The card deportment the editor owns, handed to the card views for the lookups their fields make.

## `public CSentence LEditorSentence { get; }`

The sentence gates over the editor's desk, which the sentence rows call for every edit.
It reads the word order through the same map the catalog gates use.

## `public LClip LEditorClip { get; }`

The recording menu deportment, holding the foray the menu searches recordings through.

## `public LNotation LEditorNotation { get; }`

The pronunciation menu deportment, holding the foray the menu searches readings through.

## `public LSounding LEditorSounding { get; }`

The sound sheet of the held entry, which the editor hands to the rime, script and paradigm boxes.
Its refusals are announced through `LEditorFailed`, so one notice reaches the window.

## `public QEsteem LEditorEsteem { get; }`

The favourite, grasp and frequency of the held entry, built over the editor's desk.
It forwards each mark to the editor's display, whose refusals reach `LEditorFailed`.

## `public QTimbre LEditorTimbre { get; }`

The pack sound facts and sound-sheet commands of the held entry, built over the desk, display and sheet.

## `public event Action<CEntryDraft>? LEditorDraftChanged;`

The held entry was read again, so the editor view writes its controls from it.
It carries the prepared content the desk handed this controller, never a second read.

## `public bool LEditorClipStart(string word, long target, Action<CHarvestStep> sink)`

Starts a recording search over the desk, which keeps it, and reports whether one started.
The clip deportment reads the running search from the same desk.
`LEditorNotationStart` does the same for a reading search and the notation deportment.

## `public bool LEditorOwned`

Whether this editor is the input tab's, which alone shows the command rail.

## `public long? LEditorEntry`

The stored entry the held draft stands on, or null for a fresh draft or an empty desk.
It forwards to `CDeskStoredRead`, so the stored id has one home.

## `public bool LEditorRunning`

Whether a tenure is held and taking requests, so the controls are live.

## `public void LEditorOpen(long? id)`

Opens the entry, or a fresh draft when the id is null.
An entry that cannot be opened falls back to a fresh draft, after the failure has been announced.

## `public CEntryDraft? LEditorDraftRead()`

The held draft's content as the tenure last read it, applying nothing first.

## `public string LEditorPronunciationRead()`

The primary reading as the field shows it, respelled when the pack respells and one exists.

## `public void LEditorPronunciationSet(string text)`

Hands the typed reading to the tenure as a respelling or as a phonetic one, whichever the pack shows.

## `public void LEditorNoteSet(string text)`

Hands the typed note to the tenure, which trims it.

## `public void LEditorSave()`

Stores the held draft when it changed, and reopens what was stored once the desk announces it.

## `public bool LEditorFinish(bool store)`

The window's leave: stores or drops the held draft and reports whether the tenure ended.
A stored draft is reopened the way a save reopens it.
A hidden tab then comes back showing what it stored.

## `public void LEditorReset()`

Drops the typing by reopening the entry the draft stood on, or a fresh draft.

## `private void LEditorStateUpdate()`

Announces the state, and the stop once when the tenure has just halted.

## `private void LEditorFailureShow(string key, Exception exception)`

Passes a sound sheet refusal on under the editor's own notice, so the window listens once.

## `private LEntryDraft? LEditorContent`

The held entry as the tenure reads it, without persisting a deferred edit.
The draft, pronunciation and etymon reads share it, so the tenure is named once.

## `internal LTenure? LEditorTenure`

The held tenure for a field edit, or null while none is held or the desk fills its controls.
The editor's surfaces edit through it, so no surface builds a request.

## `public IReadOnlyList<CTranslationTarget> LEditorEtymonRead()`

The etymons of the held entry, as the etymology field lists them.
The display resolves them, since the etymology field no longer reaches a lectern.

## `public IReadOnlyDictionary<long, CTranslationTarget> LEditorTargetRead()`

The link targets of the held draft, keyed by entry, for the cards' translation chips.
A refused read answers empty, so the cards still draw.
