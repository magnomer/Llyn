# Corpus

The Corpus panel's owner driver, its catalog, its reading page and its edit area's sentence and speaker.
The citation, gloss and mention parts of the edit area live in Citation.
It stands apart because the panel is the view of the shared stock of sentences, not of one Entry's sentences.
Each sub-driver subscribes what it paints itself, so the owner only builds and introduces it.

## `QCorpus`

An Example is independent data owned by nothing.
The panel answers two questions rather than one.
They are what the sentence is, and where it is quoted.
An Example is quoted by any number of cards, so rewriting it here rewrites what every one of them quotes.
The panel offers no way to fork an Example while editing it.
A sentence meant for one card alone is a new Example on that card.
The driver keeps no draft id, halted flag or timer of its own, since the engine owns each of those.
Each sub-driver holds its own Conduct reference, so the owner's `_cCorpus` serves only the mode, rail, vista and chronicle.
It answers the window's undo and redo keys as `QChronicleHost`, attached to its surface since the surface is no driver.

### `_cCorpus`

It is null until the window hands one over.
The driver keeps no copy of the order, the query or the hidden languages and asks the Conduct for each.

### `QCorpus`

The page merges the excerpt and transcript dictionaries itself, so their local styles reach the look sheet here.
The order picker hangs its menu under the whole `PRank` bar.

### `QCorpusIntroduce`

Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
It keeps only the atelier, which a dropped inquest reads the languages from.
The desk and the transcript show their own failures through the envoy.
The speaker chip takes the atelier and the envoy, never the window, for its workspace reload.
The reading page's word offers go to the window's mention menu, with no unit of their own.
The linking gesture takes the menu itself and hears each sense pick on the ask it opened.
So neither this driver nor any sub-driver holds the window.
`QTranscriptMention` takes the editor's picker after the editor is introduced.
The engine's change notices drive the mode, and the panel is current whether or not its tab is in front.

### `QCorpusVistaRefine`

It answers `CWorkspaceOpened`, after the corpus restored its vistas and attached its observers.
The flags are loaded before any row is built, since a row keeps the flag it was built with.
The same read answers the languages for the filter menu and the menu `QTranscriptSpeaker` fills.
Its one request is `CCorpusRowsLoad`, which runs the flag fill and then answers the rows `QAnthology` paints.

### `QCorpusExitRefine`

It releases the editor's player.
It closes the citation drawer, the speaker chip and both pickers, so none outlives the window.
It calls no gate, because `CAtelierClose` stops the editor and its playback in Conduct.

### `QChronicleUndoObserve`

The step runs inside `QChronicle.QChronicleCaretRefine`, so the caret stays at the end of the focused box.

### `QCorpusShownCheck`

The engine uses it to know when a notice needs painting.

### `QCorpusModeUpdate`

The transcript and the reading page each light their own parts.
It ends by refreshing the rail's undo and redo.

### `QCorpusChronicleRefine`

The session reads the entry editor's chronicle while `PEditor` is in front, and the held sentence's otherwise.

### `QCorpusFreshObserve`

`CDiptychEntryCreate` decides between a fresh Example and a new quotation.

### `QCorpusBinObserve`

`CDiptychEntryDelete` acts only while an Example and not an Entry is shown.

### `QCorpusScribeObserve`

The gate asks before leaving an editor, so unsaved wording is never lost silently.

## `QAnthology`

The left column, with the Example search field over it.
It lists every Example the workspace holds, including one nothing quotes.
A chosen row is read back and shown, and the middle column narrows to the entries quoting it.

### `QQueryClear`

It empties the search box after an arrival dropped the query on the vista.
The owner's `QCorpusClearRefine` rebuilds the filter menu on the same notice.

### `QAnthologyRefine`

The driver decides nothing about the ordering, the query or the wording.
The read reports its own failure, and it drops a shown selection whose row no longer stands.
The paint of ready rows asks Conduct nothing, and the owner's flag-fill Refine hands in what its load answered.

### `QAnthologyObserve`

`CDiptychParentSelect` asks about unsaved work first, and records the station only when the move goes ahead.

### `QAnthologyApply`

The row carries the `Chosen` cue on the chosen item and none otherwise.
The click is removed first, so a refill never doubles it.

## `QAnthologyItem`

One Example row in `PAnthology`, carrying the sentence, its language and flag, and the usage count.
The cited Source is not carried, because a row says what the sentence is and not where it came from.
The id is identity and never displayed.
Two Examples may carry the same sentence, so a row is never found by what it reads.
An empty sentence is not one thing.
It may never have been written, or it may have been written and be unknown now.
The row gets the mark for the second and the unwritten text for the first, so no row stands blank.
The map applied those wordings, so nothing is decided here.

### `QAnthologyItemCount`

The text is blank while the Example is unused.

### `QAnthologyItemChosen`

It is the only value of the row that changes after the row is built.
`QSplice` moves the mark in place, so the list keeps its scroll position.

### `QAnthologyItemName`

The engine numbers repeated sentences as `(1)`, `(2)` on the row it returns.
A repeat is only visible across rows.
`QAnthologyItemText` keeps the plain sentence for everything that is not display.

### `QAnthologyItemMatch`

The chosen mark is left aside.
`QSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## `QCorpusPortrait`

The print and export commands of the Corpus panel, which the rail's buttons reach.
Both act on what the display reads, so neither needs any other part of the panel.

### `_cCorpus`

It is null until the owner hands one over, so the command checks answer false before that.

### `QCorpusPressCheck`

An editor on screen prints nothing, because what is printed is what is read.

### `QCorpusPressObserve`

`CCorpusPortraitPrint` picks the page from the side in front and asks for the printer.
The engine builds the page from stored rows.

### `QCorpusPortraitCheck`

Print may act on the other page this panel reads, but export acts on entries alone.

### `QCorpusPortraitObserve`

`CQuotationPortraitExport` asks for the file and the format through the envoy.
The engine writes the document from stored rows.

## `QExcerpt`

The reading side, the page one chosen Example is shown on.
The sentence stands at its head, its Gloss rows under it, and the cited Source last.
A click on a word asks the corpus's gate what it names.

### `QExcerptMentionOffered`

The owner hands the offer to the window's mention menu, so this driver never holds the window.

### `QExcerptIntroduce`

The language list is `QTranscriptSpeakerLanguage`, which each read Gloss row lists.

### `QExcerptTallyRefine`

A quotation added elsewhere refills the catalog, so its count shows at once.

### `QExcerptSentenceRefine`

It looks up `CExampleWording` when Conduct chose a word, and shows the text itself otherwise.
`CExampleMuted` picks the muted colour, because the head of the page cannot be empty.
A worded key draws as one plain piece made by `QMentionPieceCreate`, since the pieces belong to the text it replaces.
The click hands the corpus gate only raw click values, so the mention text holds no Mentions here.

### `QExcerptGlossShow`

The whole section hides when there is no Gloss, as a never-written field is not drawn.

### `QExcerptCitationRefine`

The heading hides when no citation was ever written.

### `QExcerptMentionObserve`

The raw click values go unchanged to `CCorpusMentionFind`, which asks, converts, finds, opens and reports failures.

## `QTranscript`

The sentence field of the edit area, with its placeholder and its tally chip.
What the field holds is handed raw to `CAnthologyTextSet`.
The desk on the corpus Conduct holds the tenure from start to commit or cancel.

### `QTranscriptTeardown`

The typing handler is unsubscribed, so a fill from the held sentence writes nothing back.
Filling a field raises the same change the user typing raises, and only typing is a request.

### `QTranscriptTextObserve`

The placeholder is painted from the key the gate answers.

### `QTranscriptDraftRefine`

Whether the text differs is the Example's `CExampleTextKept` verdict from Conduct, so a bulletin never moves the caret.

### `QTranscriptTallyRefine`

A quotation added elsewhere refills the catalog, so its count shows at once.

## `QTranscriptSpeaker`

The speaker chip of the edit area, with the dropper that picks the held Example's language.
It fills the language menu that every Gloss row of the panel lists.

### `QTranscriptSpeakerLanguage`

The owner hands the same list to `QExcerpt` and `QTranscriptGloss`, so every Gloss row offers the same languages.

### `QTranscriptSpeakerIntroduce`

It takes the atelier and the envoy rather than the window, since the workspace reload needs only those two.

### `QSpeakerWorkspaceRefine`

The flags and the language menu belong to the old folder, so both are loaded again.
One ensign read loads the flags and answers the languages.
The envoy goes with the load, so the catalog reports a failed load and answers no languages.

### `QSpeakerRefine`

The chip is not redrawn, since a fresh workspace holds no draft.
No default is chosen, because an Example's language is optional and the store accepts none.

### `QSpeakerObserve`

The chip changes when the draft bulletin returns.

### `QSpeakerShow`

It runs on every transcript and draft notice, since painting the same language changes nothing.

## Stamp

Commit: `cb8ab7e8feb4`
- `QAnthology.cs` `a770dbcc55e7ffd8`
- `QAnthologyItem.cs` `b1316675b6391a64`
- `QCorpus.cs` `b478d30964aabaed`
- `QCorpusPortrait.cs` `cfe29836ed70f4cf`
- `QExcerpt.cs` `7f4c70cd37f6e033`
- `QTranscript.cs` `045f9f0e592d2d31`
- `QTranscriptSpeaker.cs` `106962427c901ebd`
