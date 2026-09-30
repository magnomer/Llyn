# CCorpus.cs

## `public sealed class CCorpus`

The corpus panel's session: the example list, the quotation list, the transcript desk and the entry editor.
The two vistas' chosen rows and editing flags are the panel's mode, and the drivers only follow.
Every gate holds only the interaction and reaches the engine through a panel, the desk or the editor.
It asks the user and reports failures through the envoy, never through a seam a driver hands up.
It restores both vistas itself and chooses which side prints, exports or deletes.
It attaches its lists to the engine's notices itself, so no driver wires a subject.

## `private CCorpus(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the desk under the `Example` scope, the anthology over it and the quotation panel over the editor's desk.
The session defers to the entry editor while the quotation list edits.
A stored Example is shown again outside the scribe through `LCorpusStoredShow`.
The quotation panel opens the editor on the entry it edits, and a clear cancels the editor's desk.
The anthology's row notices reach the quotation panel, whose rows follow the chosen Example.
It registers its close with the workspace, so the window's exit gate stops the entry editor.
It wires the transcript desk's notices through `marshal`, which only the medium knows.
A draft notice raises `CCorpusDraftChanged` with the Example read afresh, so no driver wires the desk.
It restores its vistas last, so a built area already stands on started vistas.

## `public static CCorpus CCorpusCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the corpus over the atelier, with the panel's own entry editor.
Building it is no user action, so it is no gate on the atelier.
`shownSeam` answers whether the tab is in front, which only the surface knows until `CNavigation` owns it.

## `public event Action? CCorpusChanged`

Raised when the desk or the entry editor changes state, so a driver repaints the mode.

## `public event Action<CExample>? CCorpusTranscriptChanged`

Raised with the Example the desk holds after a start.
After a cancel, a refused start or a failed read it carries the blank Example, never null.

## `public event Action<CExample>? CCorpusDraftChanged`

Raised with the held Example after an edit to its draft, through the marshal the corpus was built with.
`CCorpusTranscriptChanged` fills the transcript anew, while this one only refreshes what the edit changed.

## `public event Action<CExample>? CCorpusExampleChanged`

Raised with the Example a loaded anthology draft carries, so the excerpt shows it.

## `public event Action? CCorpusQueryCleared`

Raised once an arrival drops the query and the language filter, so a driver empties both controls.

## `public event Action? CCorpusWorkspaceChanged`

Raised after a workspace notice closed the shown Example, so a driver reloads what belongs to the folder.

## `private readonly Action<Action> _cCorpusMarshal`

The medium's marshal the corpus was built with.
Every engine notice the corpus answers runs through it, since only the medium knows its thread.

## `private readonly CAtelier _cCorpusAtelier`

The atelier the two vistas start through on every restore.

## `public CQuotation CCorpusQuotation { get; }`

The quotation list, whose panel asks the entry editor's desk whether the entry holds unsaved changes.
It is built over the atelier's entry and portrait ports, so no driver holds either.

## `public bool CCorpusTranscriptShown`

True while the example side is in front and in edit mode, so the transcript shows.
The other mode flags follow the same two facts: which side is in front and whether it edits.
`CCorpusExcerptShown` holds for the example side outside edit mode.
`CCorpusDisplayShown` holds for the quotation side outside edit mode, as `CQuotationShown` says.
`CCorpusEditorShown` holds while the quotation list is in edit mode.
`CCorpusScribeChecked` holds while either editor shows, and `CCorpusViewerChecked` holds otherwise.
`CCorpusModeEnabled` holds on the quotation side, and on the example side follows its panel.
`CCorpusBinEnabled` holds only on the example side, where it follows its panel.
`CCorpusExcerptHeld` and `CCorpusExcerptBlank` say whether the example list has a chosen row.
`CCorpusStoreEnabled` follows the entry editor while it shows, and the transcript desk otherwise.

## `public bool CCorpusPressAllowed`

Print takes an entry on display or a chosen Example on the excerpt.
Export, as `CCorpusPortraitAllowed` says, takes only an entry on display.

## `private bool LCorpusQuotationSide`

The quotation side is in front exactly when the quotation list has a chosen row or is in edit mode.
No driver reads it, since the mode flags it feeds answer every question a driver asks.

## `private bool LCorpusRowShown`

True while the excerpt shows a chosen Example, the only case a missing row clears.

## `private bool LCorpusRowHeld`

True while either list has a chosen row, so a fresh start opens a quotation.

## `internal CExample? LCorpusTranscriptRead()`

Reads the Example the desk holds, which persists the tenure first and so can throw.
A throw reports `Example.HoldFailed` through the envoy and reads as null.
`CCorpusDraftChanged` hands it on each draft bulletin, so the desk never hands a driver a draft.

## `private void LCorpusDraftResonate()`

Answers the desk's draft notice with the held Example read afresh.
A notice with no Example to show raises nothing, so the driver never redraws from nothing.

## `public void CCorpusMentionOpen(string word)`

The gate that opens the transcript's mention picker, for the selected word as the user gave it.
It searches in the held Example's own language, through `CMention.LMentionProspectRead`.
The offer is raised as `CCorpusMentionOffered`, which the corpus editor's picker paints.

## `public event Action<CProspect>? CCorpusMentionOffered;`

The transcript's mention offer, ready to show.
The picker belongs to the corpus editor, so its Refine is wired to this event rather than handed a record.

## `public void CCorpusMentionAdd(string text, int start, int length, long entryId)`

The gate for an Entry picked for the transcript's selection, with the box's raw text and selection.
The span rule and the request are ShellEngine's.
Entry 0 marks the selection as standing for nothing, which is the silence command.

## `public void CCorpusSenseSet(string text, int start, int length, long senseId)`

The gate for a Meaning picked in the sense menu, with the field's raw text and selection at the pick.
The Mention under the selection is found and narrowed below Conduct, and a selection in no Mention changes nothing.

## `public void CCorpusMentionRemove(long mentionId)`

The gate for a chip's unlink, with the chip's own Mention id.

## `public void CCorpusMentionRemove(string text, int start, int length)`

The gate for an unlink from the field, with its raw text and selection.
The Mention under the selection is found below Conduct, and a selection in no Mention changes nothing.

## `public bool CCorpusMentionCheck(string text, int start, int length)`

Whether a Mention lies under the field's selection, so the unlink command may run.
It answers from the raw selection and never persists.

## `public bool CCorpusSenseCheck(string text, int start, int length)`

Whether a Mention linked to an Entry lies under the field's selection, so the sense command may run.
It answers from the raw selection and never persists.

## `public CMentionSense? CCorpusSenseRead(string text, int start, int length)`

The sense menu, ready to show, for the Entry the selection's Mention links to.
It answers null when no linked Mention lies there, so no menu opens.
A failed read shows `Mention.FindFailed`, through the shared read in `CMention`.

## `public IReadOnlyList<CMentionLabel> CCorpusMentionRead()`

The chip line under the transcript, ready to paint, from the draft the transcript's desk holds.
A silent chip carries its own word key, and a failed read shows `Mention.FindFailed` and answers no chips.

## `public CMentionOffer? CCorpusMentionFind(int offset)`

The gate for a click at `offset` on a word of the excerpt.
It asks the leave question first, since the click may open an Entry in another panel.
The anthology then finds the word, opens what opens at once and reports a failure.
The answer is what the menu offers under the found word, null when the user stays.

## `private void LCorpusExampleUpdate(CExample? example)`

Announces the Example a loaded draft carries and ignores drafts of other subjects.

## `internal void LCorpusExampleOpen(long id)`

The navigation's arrival on the corpus tab.
It shows an Example that comes from outside the list, keeping the scribe open when either editor was open.
When the query or the filter hides it, the list clears it at once.
Both are then dropped and the Example is shown again, so an arrival never lands on a blank excerpt.
A row that vanished stays cleared, and without a query or filter nothing is dropped.

## `private void LCorpusExampleShow(long id, bool editing)`

Clears the quotation side and shows the Example in the mode the caller read.
The anthology panel loads the row, clears a vanished one and restarts the transcript through its edit event.

## `public void CCorpusExampleSelect(long? id)`

Shows the clicked Example once the user agrees to leave unsaved changes.
The mode is read before the question, because a save from the dialog must not drop the scribe.
The voyage is recorded only after the user agreed, so a refused leave keeps the Forward history.
A save from the dialog finishes through `CSessionClose`, so the Example loads once.

## `private void LCorpusQuotationOpen(long id, bool editing)`

Loads the entry on the quotation side first, and touches nothing else when it fails or has vanished.
Only a held entry cancels the transcript and closes the example scribe.
The scribe then reopens on the entry when either editor was open.

## `public void CCorpusQuotationSelect(long? id)`

Shows the chosen entry once the user agrees to leave unsaved changes.
The mode is read before the question, as `CCorpusExampleSelect` does.

## `public void CCorpusExampleCreate()`

Starts a quotation for the chosen row when a row is chosen, else a blank Example transcript.

## `public void CCorpusScribeToggle(bool editing)`

Toggles the scribe on the side in front.
Closing the quotation side falls back to the chosen Example, and closing the transcript cancels the desk.

## `private void LCorpusExampleRestore(bool editing)`

Shows the chosen Example again in the given mode, or clears both lists when none is chosen.

## `private void LCorpusExampleClose()`

Clears both lists, also when the workspace changes under the panel.

## `private void LCorpusClose()`

The corpus's part of the window's exit gate `CAtelier.CAtelierClose`, registered with the workspace at build.
It closes the entry editor and cancels its display's playback.
It leaves the transcript desk alone, as the panel's close always did.

## `private void LCorpusQueryClear()`

Drops the query and the language filter on the example vista and raises `CCorpusQueryCleared`.

## `private void LCorpusStoredShow(long id)`

Shows the stored Example outside the scribe after the desk stores it.
The save button, a tab leave and the window's close finish through it, and a row click does not.

## `private void LCorpusQuotationCreate()`

Opens a fresh entry in the editor, already citing the chosen Example when there is one.
The engine starts and cites it in one call, so the first paint shows the citation.
The fresh open clears the quotation panel, which closes the editor before the blank opens.

## `internal bool LCorpusLeaveConfirm(bool shown)`

True when nothing is unsaved or the user agrees to leave.
No driver asks it, since the navigation's tab and every corpus gate ask it inside.
Asks the envoy only over unsaved changes.
A store finishes the session, and a discard leaves without touching it.
The finish shows the stored Example only when `shown` holds.

## `internal void LCorpusEntryResonate()`

Answers the chosen entry's notice by refreshing the quotation draft.
The quotation list's observers call it through the marshal, so no driver hands the notice over.
It falls back to the chosen Example once the quotation side has closed.
It does nothing without a chosen quotation, so a whole-set bulletin never clears a transcript or a new entry.
The mode is read before the refresh, so a vanished entry under edit reopens the transcript.

## `public IReadOnlyList<CCatalogExample> CCorpusRowsRead()`

Reads the example list's rows, ready to paint and worded by the engine.
A failed read is reported by the anthology and answers no rows, and then nothing is cleared.
When the excerpt shows a chosen Example the rows no longer list, both lists clear before the rows return.
The clear announces fresh rows, which carry the same Examples, so a driver's nested refill is harmless.


## `public Task<CEnsignSheet<IReadOnlyList<CCatalogExample>>> CCorpusRowsLoad(`

Runs the flag fill into the driver's `store`, then answers `CCorpusRowsRead` beside the loaded languages.
The shared rule `CCatalog.LCatalogEnsignLoad` orders the two, so the driver makes one request.

## `public void CCorpusExampleDelete()`

Deletes the chosen Example through the anthology panel, which asks the envoy first.
It does nothing on the quotation side, whose panel has no delete scope.
A failure reads `Example.DeleteFailed`, the delete key `CAnthology` hands its panel.

## `public Task CCorpusPortraitPrint()`

Prints the entry on display, else the chosen Example on the excerpt, else nothing.
The entry prints with the label, and the Example list with the Example realm's legend.
The reader is asked for the printer through the panel's envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the panel's envoy.

## `internal void LCorpusVistaRestore()`

Starts the `corpus` vista over Examples by text and the `quotation` vista over entries by headword.
The anthology takes the first, and the quotation list takes both, since its rows follow the chosen Example.
The entry editor takes the quotation vista, so an entry it stores lands in that list.
Each list carries its held query into its fresh vista, so a switched workspace keeps both searches.
Each list then attaches its observers on its fresh vista through `_cCorpusMarshal`.
The anthology hands a workspace notice to `LCorpusWorkspaceResonate`.
The quotation list refreshes the example rows on an entry notice and hands its chosen entry to `LCorpusEntryResonate`.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.
It paints nothing, since the drivers paint the first rows once their flags are loaded.

## `private void LCorpusWorkspaceResonate()`

Answers the workspace notice: it clears both lists, which cancels the transcript desk.
It then raises `CCorpusWorkspaceChanged`.
