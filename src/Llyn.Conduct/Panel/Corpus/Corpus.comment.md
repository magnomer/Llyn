# Corpus

This folder holds the corpus panel with its session, its example list, its quotation list and the transcript desk.

## `CAnthology`

The corpus panel's example list, the panel over the example vista.
It finds the rows, while the panel's aperture holds the vista and takes the query, order and language filter.
Its panel loads, edits and deletes the chosen Example, worded under the Example scope.
It also answers the transcript's citation field and prints the chosen Example.
The panel asks whether the desk changed before it leaves an Example.
It builds over the atelier's ports, so the driver hands it no port.
The observers refill the rows on a vista notice.
A stored Example or Source also refills them, as do a reflex fill and a flipped setting.
A Source or reflex change counts because a row cites a Source and shows an epithet beside a headword.
A workspace notice runs the corpus's own answer.
The rows read answers null on failure and shows `Example.LoadFailed`, so the corpus clears nothing on it.
The orderings offered are text, language, Source and usage.

## `LAnthologyMentionFind`

Turns the click into a code-point offset through `CMention.LMentionOffsetRead`.
Then it finds the word fresh in the chosen Example and opens what `CMention.LMentionResultOpen` opens at once.
It hands the open the same text, so the offer places the menu as a surface unit.
It answers null when no Example is chosen or the find fails.
A failure shows `Mention.FindFailed`, and the offset read and the open sit inside the same catch.
No driver calls it, since `CCorpusMentionFind` asks the leave question first.

## `LAnthologyPortraitPrint`

Opens the print window on the chosen Example with the Example realm's legend.
It opens nothing while no Example is chosen.
No driver calls it, since `CCorpusPortraitPrint` chooses which side prints.

## `CAnthologyCitationRead`

The Sources the transcript's citation drawer offers for the typed word, ready to show.
It is the citation find of the example quill over the draft's own Example.
It names no card or sentence.
It offers as the card sentence's field does.
The engine trims, splits, caps and counts.
A failed search shows `Example.FindFailed` once until the user acts, through the repaint memory.
It then answers the typed word with no rows.
No held transcript offers nothing, and typing a citation writes nothing.

## `CAnthologyCitationSet`

The id form points the citation at a Source the drawer offered, and the engine applies it at once.
The title form points it at the Source the engine resolves the typed title to, in one engine call.
The engine reads the Source cited now off the held draft, so an unchanged title keeps it.
A failure shows `Reference.CreateFailed` and leaves the citation as it was.

## `CAnthologyGlossSet`

The transcript's gloss text, deferred like the sentence text.
It is the sentence quill's gloss gate over the Example itself, which names no card or sentence.

## `CAnthologyGlossAdd`

Adds a gloss below the row at that place, where below means the next place, as `CSentenceAdd` puts a sentence.
The engine picks the gloss language.

## `CAnthologyGlossPrepare`

The empty transcript's seed field taking focus calls it.
The engine adds the first gloss only when the Example holds none, and answers whether it did.

## `CAnthologyTextSet`

Typing into the transcript's sentence field hands the raw text to the desk's quill, which defers it.
It answers the placeholder key for typed text, since typing ends an unknown mark.
Without a held tenure it writes nothing and still answers the key.
It keeps the text as the text the field shows, which `LAnthologyTranscriptRead` compares against.

## `CAnthologySpeakerSet`

Picking the speaker language is sent at once through the desk's quill.
The chip changes when the draft bulletin returns, as the corpus gloss gates do.

## `LAnthologyTextCheck`

Whether a field showing a text already shows a value, as the engine would store the field.
It asks the example port, which owns that rule.

## `LAnthologyTextShow`

Notes that the transcript's sentence field now shows the text of an Example, and hands it back.
The corpus calls it when the transcript fills anew.

## `LAnthologyTranscriptRead`

The Example a draft bulletin hands the transcript, with `CExampleTextKept` set when the field already matches.
A field that does not match is repainted, so that text becomes the one shown.
The kept-text verdict lets the driver avoid rewriting a matching sentence field.

## `LAnthologyDraftRead`

Maps the Example a draft carries to its shape, and a draft of another subject to null.
The engine answers the cited Source's line, so the driver never looks a name up.
`CTranscript` and `CCorpus` call it, so neither names an engine record.
It reads the tally from its own panel.

## `LAnthologyCitationRead`

The ready line of the Source the draft's Example cites.
A failed read shows `Reference.LoadFailed` and answers no line.

## `LAnthologyExampleRead`

Maps a stored Example, its citation line and its tally to its shape, and null to null.
The excerpt links only the Mentions the Example lets a reading link, while the transcript keeps them all.
The plain text is divided at those Mentions here, so the excerpt draws ready pieces.

## `CCorpus`

The corpus panel's session over the example list, the quotation list, the transcript desk and the entry editor.
The two vistas' chosen rows and editing flags are the panel's mode, and the drivers only follow.
The routing between the two lists and its mode flags live in `CDiptych`, shared with the repertoire.
Every gate holds only the interaction and reaches the engine through a panel, the desk or the editor.
It asks the user and reports failures through the envoy, never through a seam a driver hands up.
It restores both vistas itself and chooses which side prints.
The quotation list owns export.
It builds `CTranscript`, which holds the transcript's read and Mention gates apart from the leave question.
The desk is built under the `Example` scope.
The anthology stands over it, and the quotation panel over the editor's desk.
The transcript is built right after the anthology, since it reads its Example through it.
The diptych is built right after the session, over the two panels and the desk's start and cancel.
The session defers to the entry editor while the quotation list edits.
The quotation panel opens the editor on the entry it edits, and a clear cancels the editor's desk.
The editor's display follows the quotation panel, so each draft it loads opens the reading view.
The anthology's row notices reach the quotation panel, whose rows follow the chosen Example.
It registers its own editor close with the workspace, so the window's exit gate stops the entry editor.
The leave question is the session's, which the navigation's tab and every corpus gate ask.
It restores its vistas last, so a built area already stands on started vistas.
`shownSeam` answers whether the tab is in front, which only the surface knows until `CNavigation` owns it.
The desk stays internal, so a driver reads the transcript and `CCorpusTranscriptEnabled` instead.

## `CCorpusTranscriptChanged`

Raised with the Example the desk holds after a start.
After a cancel, a refused start or a failed read it carries the blank Example, never null.
The anthology's `LAnthologyTextShow` notes its text as the one the sentence field now shows.
`CCorpusExampleChanged` is raised with the Example a loaded anthology draft carries.
`CCorpusQueryCleared` is raised once an arrival drops the query and the language filter.
`CCorpusWorkspaceChanged` is raised after a workspace notice closed the shown Example.

## `CCorpusTranscript`

The transcript's held Example and its Mention gates, over the corpus desk and anthology.
It is split from the corpus, since editing a Mention never asks the leave question.
The excerpt's word click stays in the corpus as `CCorpusMentionFind`, since it asks that question first.
The corpus owns the desk and the anthology, and lends both to the transcript.

## `CCorpusDiptych`

The routing between the example list and the quotation list.
Its create seam is `LCorpusQuotationCreate`, so a chosen row starts a quotation.
A delete failure shows `Example.DeleteFailed`, worded from the scope `CAnthology` hands its panel.

## `CCorpusStoreEnabled`

Whether the draft being edited can be stored.
It follows the entry editor while the quotation side edits, and the transcript desk otherwise.
`CCorpusDisplayShown` is true while the quotation side is in front outside edit mode.

## `CCorpusPressAllowed`

Print takes an entry on display or a chosen Example on the excerpt.
Export, as `CCorpusPortraitAllowed` says, takes only an entry on display.

## `LCorpusRowShown`

True while the excerpt shows a chosen Example, the only case a missing row clears.

## `LCorpusExampleOpen`

The navigation's arrival on the corpus tab.
It shows an Example that comes from outside the list, keeping the scribe open when either editor was open.
When the query or the filter hides it, the list clears it at once.
Both are then dropped and the Example is shown again, so an arrival never lands on a blank excerpt.
A row that vanished stays cleared, and without a query or filter nothing is dropped.

## `CCorpusMentionFind`

The driver hands the whole shown text and the clicked UTF-16 unit in it.
It asks the leave question first, since the click may open an Entry in another panel.
The anthology then finds the word, opens what opens at once and reports a failure.
The answer is what the menu offers under the found word, null when the user stays.

## `LCorpusStoredShow`

Shows the stored Example outside the scribe after the desk stores it.
The save button, a tab leave and the window's close finish through it, and a row click does not.

## `LCorpusQuotationCreate`

Opens a fresh entry in the editor, already citing the chosen Example when there is one.
The engine starts and cites it in one call, so the first paint shows the citation.
The fresh open clears the quotation panel, which closes the editor before the blank opens.
It is the diptych's create seam, so `CDiptychEntryCreate` reaches it while a row is chosen.

## `CCorpusRowsRead`

A failed read is reported by the anthology and answers no rows, and then nothing is cleared.
When the excerpt shows a chosen Example the rows no longer list, both lists clear before the rows return.
The clear announces fresh rows, which carry the same Examples, so a driver's nested refill is harmless.

## `CCorpusPortraitPrint`

Prints the entry on display, else the chosen Example on the excerpt, else nothing.
The entry prints with the label, and the Example list with the Example realm's legend.
A decline of the printer question prints nothing.

## `LCorpusVistaRestore`

Starts the `corpus` vista over Examples by text and the `quotation` vista over entries by headword.
The anthology takes the first, and the quotation list takes both, since its rows follow the chosen Example.
The entry editor takes the quotation vista, so an entry it stores lands in that list.
Each list then attaches its observers on its fresh vista.
The anthology hands a workspace notice to `LCorpusWorkspaceResonate`.
The quotation list refreshes the example rows on an entry notice and hands its chosen entry to `LDiptychChildResonate`.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.
It paints nothing, since the drivers paint the first rows once their flags are loaded.

## `LCorpusWorkspaceResonate`

Answers the workspace notice by clearing both lists, which cancels the transcript desk.
It then raises `CCorpusWorkspaceChanged`.

## `CQuotation`

The corpus panel's quotation list, holding the entries that quote the chosen Example.
The example vista is its roll, and its own vista sits in the panel's aperture.
The corpus builds it and restores its vistas, so no driver holds a port or a vista.
Its panel runs under the `List.LoadFailed` key with no delete scope.
The empty list reads `Example.Vacant` when nothing quotes the Example, and `Example.Unmatched` under a search.
The change seam is the entry editor's desk, and the finish seam is the corpus session's.
An entry notice goes to the panel first, which may adopt a freshly stored Entry.
The example rows follow, since the store may quote an Example.
The chosen entry's own notice redraws or drops it.
The rows are every entry while no Example is chosen.
The engine matches the query and drops the hidden languages.
The quotation side owns export, since only its shown entry exports.
It exports the entry on display and does nothing otherwise.
`CQuotationShown` gates the export.
Print is called by no driver, since `CCorpusPortraitPrint` chooses which side prints.

## `CTranscript`

The corpus transcript's held Example and the gates that edit its Mentions.
It is split from `CCorpus`, since none of its gates asks the leave question.
The corpus owns the desk and the anthology, and this class only reads and edits through them.
Every gate past the open addresses the Example's single text.
It goes through the example quill or the draft site.
It names no card or sentence.
It reports failures through the envoy, never through a seam a driver hands up.
Every constructor argument is required, so a missing one fails at once.
`CCorpus` builds it right after the anthology and attaches the desk's draft notice once at build.

## `CTranscriptDraftChanged`

Raised with the held Example after an edit to its draft, through the marshal the corpus attached.
`CCorpusTranscriptChanged` fills the transcript anew, while this one only refreshes what the edit changed.
A notice with no Example to show raises nothing, so the driver never redraws from nothing.
The anthology's `LAnthologyTranscriptRead` marks whether the sentence field already shows its text.

## `CTranscriptMentionOffered`

The transcript's mention offer, ready to show.
The picker belongs to the corpus editor, so its Refine is wired to this event rather than handed a record.

## `LTranscriptRead`

Reads the Example the desk holds, which persists the tenure first and so can throw.
A throw reports `Example.HoldFailed` through the envoy and reads as null.
The Example carries the anthology panel's tally.
Its callers are the transcript's own draft notice and the corpus's session-held handler.

## `CTranscriptMentionOpen`

Opens the mention picker for the selected word as the user gave it.
It reads the offer through `CMention.LMentionProspectRead`, and a failed read shows `Mention.FindFailed`.

## `CTranscriptMentionAdd`

An Entry picked for the selection, with the box's raw text and selection.
The span rule and the request are ShellEngine's.
The engine has no new Entry path for a Mention.
So a null id shows `Refusal.TargetMissing` and links nothing.

## `CTranscriptSilenceSet`

The silence command marks the selection as standing for nothing.
It has its own gate, so a fresh pick's null id is never read as silence.
Conduct maps the silence to the engine's Entry 0 here, so the driver never sends it.

## `CTranscriptSenseSet`

A Meaning picked in the sense menu, with the field's raw text and selection at the pick.
The Mention under the selection is found and narrowed below Conduct, and a selection in no Mention changes nothing.
The unlink from the field works the same way, and a chip's unlink takes the Mention id.

## `CTranscriptMentionCheck`

Whether a Mention lies under the field's selection, so the unlink command may run.
`CTranscriptSenseCheck` asks the same for a Mention linked to an Entry.
Both answer from the raw selection and never persist.

## `CTranscriptSenseRead`

The sense menu, ready to show, for the Entry the selection's Mention links to.
It answers null when no linked Mention lies there, so no menu opens.
A failed read shows `Mention.FindFailed`, through the shared read in `CMention`.
`CTranscriptMentionRead` answers the chip line the same way, with no chips on failure.

## Stamp

Commit: `8c403d1df768`
- `CAnthology.cs` `466c68e5ea67ce97`
- `CCorpus.cs` `28fd61a422f56c0c`
- `CQuotation.cs` `42aa5e1b796518f8`
- `CTranscript.cs` `ec7c7f5897c3022f`
