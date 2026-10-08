# QCorpus.cs
Hash: `627d781c19c24a28`

## `internal sealed class QCorpus : QChronicleHost`

The Corpus panel's driver is the view of the shared stock of sentences itself.
An Example is independent data owned by nothing, so this panel is not a view of one Entry's sentences.
It holds the surface, the editor, the display, the rail, the two pickers, the atelier and the corpus Conduct.
The catalog, the quotation column and the reading page are the sub-drivers `QAnthology`, `QQuotation` and `QExcerpt`.
The sentence field, the speaker chip and the edit Gloss rows are `QTranscript`, `QTranscriptSpeaker` and `QTranscriptGloss`.
The citation field and the linking gesture are `QTranscriptCitation` and `QTranscriptMention`.
The print and export commands are `QCorpusPortrait`.
Each sub-driver holds its own Conduct reference, so the owner's `_cCorpus` serves only the mode, rail, vista and chronicle.
The panel answers two questions rather than one.
They are what this sentence is, and where it is quoted.
An Example is quoted by any number of cards, so rewriting it here rewrites what every one of them quotes.
The panel offers no way to fork an Example while editing it.
A sentence meant for one card alone is a new Example on that card.
It answers the window's undo and redo keys as `QChronicleHost`, attached to its surface since the surface is no driver.
The driver keeps no draft id, halted flag or timer of its own, since the engine owns each of those.

## `private CCorpus _cCorpus`

The corpus Conduct, holding the anthology, the quotation list, the transcript, the session and the panel's mode.
The anthology's vista carries the order, the query, and the languages hidden from the entry column.
The driver keeps no copy of any of them and asks the Conduct for each where it needs it.
It is null until the window hands one over.

## `internal QCorpus(UserControl surface)`

Takes the veneer's page as its surface, which the window pulls by contract ID.
The page merges the excerpt and transcript dictionaries itself, so their local styles reach the look sheet here.
The driver attaches itself to the page as the host of the undo and redo keys.
It builds the rail with Fresh and Portrait shown, and the two pickers over their placed controls.
It builds the nine sub-drivers over the same page.
The order picker hangs its menu under the whole `PRank` bar.
It subscribes the rail's four notices.

## `private Border QRank`

Each named part of the page is pulled through `QContract.QContractFind` by its contract ID.

## `internal void QCorpusIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)`

Builds the corpus Conduct with the window's envoy, and the corpus builds its editor and its desk.
It keeps only the atelier, which a dropped inquest reads the languages from.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
The desk and the transcript show their own failures through the envoy.
The display view builds the lectern over the editor's display, which Conduct attached to the quotation panel.
It introduces the nine sub-drivers, which subscribe what they paint.
The speaker chip takes the atelier and the envoy, never the window, for its workspace reload.
Both Gloss lists offer the languages of `QTranscriptSpeakerLanguage`.
The reading page's word offers go to the window's mention menu through `+=`.
The linking gesture takes the menu itself, and hears each sense pick on the ask it opened.
So neither this driver nor any sub-driver holds the window.
It introduces the entry display and editor with the same atelier, envoy and volume.
So an Entry is read and written.
`QTranscriptMention` takes the editor's picker after the editor is introduced.
It hands the rail the window's navigation and itself as the chronicle host.
It hands both pickers the anthology's aperture, and the order picker the orderings `CAnthology.CAnthologyOrderRead` offers.
The engine's change notices drive the mode.
Its failures reach the window through the envoy, which the Conduct asks directly.
A dropped search rebuilds the language menu, while the catalog empties its own search box.
`QCorpusVistaRefine` fills the example catalog when the workspace opens.
Every change after that arrives as an announcement.
The panel is current whether or not its tab is in front.

## `internal async void QCorpusVistaRefine()`

Answers `CWorkspaceOpened`, after the corpus restored its vistas, carried the queries and attached its observers.
The two pickers draw the ordering and the filter mark from the anthology first.
The flags are loaded before any row is built, since a row keeps the flag it was built with.
The same read answers the languages for the filter menu and the menu `QTranscriptSpeaker` fills.
`QAnthology` then paints the catalog, which is the panel's first paint in the workspace.
Its one request is `CCorpusRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `internal void QCorpusExitRefine()`

Releases the editor's player and has the citation drawer, the speaker chip and both pickers close their popups.
So none outlives the window.
It calls no gate.
The window's exit gate `CAtelierClose` stops the editor and its playback in Conduct.

## `public void QChronicleUndoObserve()`

Steps whichever draft is in front one snapshot back, through the corpus session.
The step runs inside `QChronicle.QChronicleCaretRefine`, so the caret stays at the end of the focused box.

## `public void QChronicleRedoObserve()`

Steps whichever draft is in front one snapshot forward again.

## `private bool QCorpusShownCheck()`

Whether the page is on screen, so the engine knows when a notice needs painting.

## `private void QCorpusModeUpdate()`

Paints the mode the engine decides.
The transcript and the reading page each light their own parts.
The rail takes the scribe verdict, the mode and bin enablement, and the store verdict.
It ends by refreshing the rail's undo and redo.

## `private void QCorpusChronicleRefine()`

Lights the rail's two chronicle buttons only when the draft in front has a step to walk.
The session reads the entry editor's chronicle while `PEditor` is in front, and the held sentence's otherwise.

## `private void QCorpusClearRefine()`

Has the filter picker rebuild its language menu after an arrival dropped the query on the vista.
The picker then redraws its mark.
`QAnthology` empties its own search box on the same notice.

## `private void QCorpusFreshObserve()`

The rail's Fresh asks the diptych's gate `CDiptychEntryCreate`, which decides between a fresh Example and a new quotation.

## `private void QCorpusBinObserve()`

Hands the rail's delete to the diptych's gate `CDiptychEntryDelete`.
That gate acts only while an Example and not an Entry is shown.

## `private void QCorpusStoreObserve()`

The rail's save, standing for whichever editor is in front.

## `private void QCorpusScribeObserve(bool scribe)`

Asks the diptych's `CDiptychScribeToggle` for the side the rail's toggle names, on whichever side the corpus stands.
The gate asks before leaving an editor, so unsaved wording is never lost silently.
