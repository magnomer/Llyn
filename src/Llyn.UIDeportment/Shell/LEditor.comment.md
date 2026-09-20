# LEditor.cs

## `public sealed class LEditor`

The entry editor's spine as a deportment: the desk it sits at, and every entry-level fact and command.
It opens an entry or a fresh draft, stores, resets, and forwards the headword, reading, note and language.
The favourite mark, the grasp, the frequency, and the read-only sections are read from it as facts.
The card lookups go through the card deportment, and the sound rows ask the window deportment for pack facts.
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

## `public LDisplay LEditorDisplay { get; }`

The reflex fold of the editor's own reflex block, kept the way the reading view keeps its own.
The editor's reflex rows also start, check and rebuild their fetch through it, as the reading view does.

## `public LCard LEditorCard { get; }`

The card deportment the editor owns, handed to the card views for the lookups their fields make.

## `public LClip LEditorClip { get; }`

The recording menu deportment, holding the foray the menu searches recordings through.

## `public LNotation LEditorNotation { get; }`

The pronunciation menu deportment, holding the foray the menu searches readings through.

## `public void LEditorClipStart(string word, long target, Action<LHarvestStep> sink)`

Starts a recording search over the desk's tenure and hands the foray to the clip deportment.
`LEditorNotationStart` does the same for a reading search and the notation deportment.

## `public bool LEditorOwned`

Whether this editor is the input tab's, which alone shows the command rail.

## `public long? LEditorEntry`

The stored entry the held draft stands on, or null for a fresh draft or an empty desk.

## `public bool LEditorRunning`

Whether a tenure is held and taking requests, so the controls are live.

## `public void LEditorOpen(long? id)`

Opens the entry, or a fresh draft when the id is null.
An entry that cannot be opened falls back to a fresh draft, after the failure has been announced.

## `public LEntryDraft? LEditorDraftRead()`

The held draft's content as the tenure last read it, applying nothing first.

## `public string LEditorPronunciationRead()`

The primary reading as the field shows it, respelled when the pack respells and one exists.

## `public void LEditorPronunciationSet(string text)`

Defers the typed reading as a respelling or as a phonetic one, whichever the pack shows.

## `public void LEditorNoteSet(string text)`

Defers the note without the trailing line breaks the box carries.

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

## `public void LEditorFavoriteSet(bool marked)`

Marks or clears the favourite on the stored entry, announcing a refusal and a re-read on failure.

## `public void LEditorGraspSet(int step)`

Writes the grasp step on the stored entry, announcing a refusal and a re-read on failure.

## `public int LEditorGraspStep => _lEntryPort.LEngineGraspStep;`

The last grasp step, which the star control takes as its limit so it names no Core constant.

## `public string LEditorGraspFormat(int step)`

The wording of a grasp step, or empty for a fresh draft that has no grasp to word.
The engine words it, so the deportment names no localization and no grasp key.

## `public IReadOnlyList<LFanqieGroup> LEditorFanqieRead()`

The stored entry's rime-book groups, started when missing, or nothing for a fresh draft or a refusal.

## `public IReadOnlyList<LFanqieRow> LEditorAnchorRead()`

The same rows flattened, for the reflex rows to anchor on.

## `public void LEditorFanqieRebuild()`

Fetches the rime-book rows again and announces the change, or the refusal.
