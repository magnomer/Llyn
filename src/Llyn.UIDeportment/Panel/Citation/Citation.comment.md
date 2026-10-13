# Citation

The drivers of the Corpus panel's quotation column and its edit area's citation, gloss and mention parts.
It stands apart from Corpus because these are the sub-drivers `QCorpus` builds and introduces.
Each sub-driver subscribes what it paints itself, so the owner only builds and introduces it.

## `QCitationItem`

One Source offered in a row's Source list, with the id the row cites and the `Author (Year)` line.
The list is the workspace's whole shelf of Sources.
So the same row object is offered to every Example and Situation row on the form.

### `QCitationItemCreate`

The line is the byline the catalog row derived, which the record answers rather than this row.
So the shelf, the example line and the citation dropdown never write one Source two ways.

## `QDrawer`

The citation drawer under the transcript's citation field.
It holds the offered rows apart from the driver.
The row the arrow keys stand on is the list's own selection, and the pick reads it.
When the popup closes by any route, the selection clears and the offered rows empty.

### `QDrawerShow`

The drawer shuts when the engine's verdict says it stays shut.
It opens under the field's frame and is at least as wide as it.
No row is chosen on opening, so Enter commits the typed line until an arrow is pressed.

### `QDrawerMove`

The lantern gate answers the row one down or up, which wraps at either end.
An empty list gets no row from the gate, so nothing moves.
It answers whether a row was chosen, so the caller handles the key only when the selection moved.
The gate reads the list's selection and row count, and the drawer shows its answer.

### `QDrawerPlace`

The list hangs under the field's frame, and flips above it when the window ends first.
The shade is pulled back so the frame edges line up with the field.

### `QDrawerFrameFind`

The frame is the one the field's template draws.
So the drawer lines up with it rather than with the bare text.

## `QDrawerItem`

One offered Source in the drawer, copied from the offer row the engine split.
It holds text only, so the drawer's rows are fed no Conduct shape.
The lead is the text before the match.
It is the whole text when the typed word is not found in it.
The mark is the matched part, drawn in weight.

### `QDrawerItemApply`

The press is removed first, so a refill never doubles it.

## `QOccurrenceItem`

One Entry as a row of the repertoire panel's entry list.
It mirrors the taxonomy panel's row, because every middle column answers the same shape of question.
That question is which Entries reference the Situation chosen in the catalog beside them.
An Entry referencing from several of its cards is still one row, because the row stands for the Entry.
The headword and the shown name are separate, so two Entries sharing a headword can be numbered apart.
The chosen mark is the one thing that changes after the row is built.
It comes as a constructor parameter, so the mark has one kind of writer.

### `QOccurrenceItemEpithet`

The epithet is the reading the language pack names, empty when the setting is off or the entry keeps none.

### `QOccurrenceItemMatch`

The chosen mark is left aside.
`QSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

### `QOccurrenceItemSync`

The chosen mark of the fresh row is copied onto the held row that `QSplice` kept.

## `QQuotation`

The middle column of the Corpus panel, with the dredge field over it.
It lists the entries quoting the chosen Example, or every Entry while none is chosen.
A chosen row swaps the Example reading for the entry display in place, without leaving the tab.
The owner `QCorpus` only builds and introduces it.

### `QQuotationIntroduce`

It takes the atelier only to hear the workspace opening, which gives the list its first paint.

### `QQuotationVistaRefine`

A row keeps the flag it was built with, so the flags are loaded before the rows are read.
The catalog's restore paints the catalog, so each list reads only its own rows.
Its one request is `CQuotationRowsLoad`, which runs the flag fill and then answers the rows it paints.

### `QQuotationRefine`

The engine matches the dredge text and drops the hidden languages, so the driver decides nothing about what matches.
A failed read is reported by the list, which answers no rows.
An empty result is shown rather than hidden.
Its wording key is the aperture's `CApertureKey`, chosen by whether the list holds a query.
The paint of ready rows asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

### `QQuotationObserve`

A clicked row asks `CDiptychChildSelect` to show the Entry without leaving the tab.
It shows in the display or the editor, whichever the mode holds.

### `QQuotationApply`

The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is removed first, so a refill never doubles it.

## `QQuotationItem`

One Entry as a row of the corpus panel's entry list.
It mirrors `QOccurrenceItem` and the taxonomy row, asking which Entries quote the chosen Example.
Its epithet, match and sync follow the same rules as there.

## `QTranscriptCitation`

The citation field of the Corpus edit area, written the way the card row writes its own.
The typed Source is answered by the drawer below.
A pick, or an Enter on a chosen row, cites it.
An Enter with no chosen row hands the typed line to one gate.
That gate may mint a Source titled with it.
The key handlers run in subscription order, one per role, as the card row's handlers do.

### `_qDrawer`

It is built over the drawer's parts here, with the press on a row answered by this driver.

### `_qTranscriptCitation`

It is the citation line the field last showed, as the Example carried it ready.
It is paint memory for that one control, never a decision about data.
Escape, leaving the field and a pick or commit redraw the field from it.
A bulletin repaints the field only when the carried line differs from it, so typing survives unrelated edits.

### `QTranscriptCitationClose`

The drawer is shut so it never outlives the window.

### `QCitationRefine`

The text handler is unsubscribed while writing, because a fill is not a keystroke and must not open the drawer.
The line arrives ready on the Example, so the field looks nothing up.

### `QCitationTextObserve`

Which rows, how many, how each is split and whether the drawer opens are the engine's to say.
A failing search answers an empty offer, so the drawer shuts rather than showing an error on every keystroke.

### `QCitationKeyRefine`

Down and Up always ask the drawer to step, and the key is handled only when a row was chosen.
Escape shuts the drawer and is handled only while the drawer is shown.

### `QCitationPickObserve`

An Enter on the chosen row cites that Source by id through one gate.
The list holds no row while the drawer is shut, so a shut drawer leaves Enter to the commit.

### `QCitationCommitObserve`

An Enter the pick left unhandled hands the typed line to the typed-citation gate.
An empty line drops the citation but leaves the Source standing.

### `QCitationEscapeRefine`

An Escape the drawer left unhandled drops the typed line for the cited line.

### `QCitationLeaveRefine`

Leaving the field drops an uncommitted line, since a typed line never entered is not a citation.

### `QCitationPressObserve`

It answers the pointer going down, since the popup takes the window's activation when pressed.
A pick of the citation already held raises no bulletin, so the field is redrawn here too.

## `QTranscriptGloss`

The Gloss rows of the Corpus edit area, while `QExcerpt` lists the read ones.
Every edit goes through the anthology's gloss gates, the card sentence's gates over the Example itself.

### `QTranscriptGlossIntroduce`

The language list is the speaker menu `QTranscriptSpeaker` fills, which each edit row lists.
It binds the row list and attaches its own fill, since the template carries no bindings.

### `QTranscriptGlossShow`

The rows whose ids survive are kept.

### `QGlossSpeakerObserve`

The language goes to `CAnthologyLanguageSet`, which sends it at once.
The row raises only a pick, since its own fill sets the list with its handlers off.

### `QGlossTextObserve`

The text goes to `CAnthologyGlossSet`, which defers it keyed by the row.

### `QGlossAddObserve`

The place is the row whose plus was pressed, or the last place from the seed line.

### `QTranscriptGlossApply`

The row's text handler is off while filling, so a fill never reads as typing.
The plus and minus get their icons here, which their styles leave bare.

### `QTranscriptSeedShow`

The seed line shows only while the list is empty, so a first row can be added from somewhere.

### `QTranscriptSeedObserve`

Focusing the seed field asks `CAnthologyGlossPrepare` for a first row.
Only a made row hands on to the focus Refine.

## `QTranscriptMention`

The corpus scribe's side of the linking gesture, the same asks the card row makes.
Every gate call is the transcript's own, through `_cTranscript`, so the engine lands it on the draft's own Example.
No call asks the leave question, which is why the gates live on `CTranscript` and not `CCorpus`.
The chip line under the transcript shows what the transcript reads from that draft.

### `QTranscriptMention`

It adds the five Mention command bindings to the transcript, since that field is the corpus's, not the editor's.

### `_qTranscriptMentionMenu`

It is the menu driver alone, so this driver never holds the window.

### `QTranscriptMentionIntroduce`

It takes the atelier rather than the window, since the span check needs only its Mention gate.
The transcript's mention offer is wired to the picker's Refine, so no record is handed across.

### `QTranscriptLinkObserve`

The picker belongs to an editor, so the corpus's own editor shows it.
The command runs only when the selection spans anything, so no empty look happens here.

### `QTranscriptPickObserve`

The box's text and selection are handed raw, and the span is read below Conduct.
The command's parameter is the raw nullable id, and the gate decides what null means.

### `QTranscriptMeaningRefine`

It subscribes the `QMentionAsk` the menu answers, so only its own pick comes back.
A selection inside no linked Mention reads nothing, so the menu stays shut.

### `QTranscriptSenseObserve`

No other area's pick reaches it, so it compares no anchor.
The Mention under the selection at the pick is the one narrowed.

### `QTranscriptSilenceObserve`

The gate is its own, so no magic id is sent.

### `QTranscriptUnlinkObserve`

The gate that drops the chip's Mention and the gate that drops the Mention under the selection are exclusive.
The choice is by whether the command carries a chip.

### `QTranscriptMentionRefine`

A draft that holds no Mention shows an empty line.
The notice's Example is not read, since the transcript answers the draft's Mentions itself.

## Stamp

Commit: `c2f335c331fe`
- `QCitationItem.cs` `b0bb21eefb49ff43`
- `QDrawer.cs` `5bbc059f4b2fd2de`
- `QDrawerItem.cs` `3faf6e5db0d974be`
- `QOccurrenceItem.cs` `08fe00f01d95dc44`
- `QQuotation.cs` `19aa8cfe966fbd31`
- `QQuotationItem.cs` `90d64fd4af9f9ef3`
- `QTranscriptCitation.cs` `17199ca8207bd35c`
- `QTranscriptGloss.cs` `721f6bb7610c05b6`
- `QTranscriptMention.cs` `6930521213e35ccd`
