# Card

Card holds the gates of the entry editor's cards, which hold meanings and collocations.
It is split by role, so each class keeps one kind of field or row.
The gates hold the interaction only, and the draft rules live below in the Application clerks.
`CFolio` holds the maps from engine drafts to the shapes every driver shows.
Every gate that edits the draft does nothing while the desk fills it, so an echoed field writes nothing back.

## `CCard`

The gates of what a card's fields ask while the user types.
That covers translation, situation, source, register and tag fields.
It holds only the desk and the ports it was given, and the editor owns one.
A failed translation shows through the envoy with the ready notice the settings port reads.
The source search shows `Input.FindFailed` once until the user acts, through the atelier's repaint memory, and offers nothing.

### Translation

`CCardTranslationAdd`, `CCardTranslationResolve` and `CCardTranslationInsert` are internal.
Drivers reach them through `CCaret`, which supplies the position from the translation trail.
So no driver holds or sends a place in the translation row.
`CCardTranslationRemove` stays public, and `CCaret` also hands it to the translation trail.

`CCardTranslationAdd` hears every change of the translation field.
The quill links each completed word that resolves to one entry and answers what the field keeps.
The same answer carries the dropdown, so the field paints both at once.
The dropdown leaves out the edited Entry and lists the fresh Entry languages.
A failed resolve shows `Input.TranslationFailed`, keeps the text as typed and offers nothing.

`CCardTranslationResolve` is the gate for enter or for focus leaving the field.
Enter answers the dropdown, with one whole-headword match already chosen, so one keystroke confirms it.
Leaving links one whole-headword match without asking.
That choice between showing and linking is the interaction this gate holds.
A failed search or link shows `Input.TranslationFailed` and leaves the word in the field.

`CCardTranslationInsert` is the one gate for any row picked from the dropdown.
A stored Entry id links that Entry to the card at once.
A null id is a fresh row, a new Entry in one language.
The court starts first from the fresh word and language, then its new Entry is linked.
The driver never sends the engine's id 0 for a new Entry.

`CCardTranslationRemove` erases a link, and the quill drops the court behind a tentative link in the same call.
Both a failed drop and a failed insert show `Input.TranslationFailed`.

### Mention and citation maps

`CCardMentionRead` opens the mention picker with a word, searched in the draft's own language.
`LCardProspectRead` is a plain map with no rule, reused by `CMention`.
`LCardProfferRead` maps the engine's citation offer, and `CAnthology` reuses it, so one offer has one map.

`CCardReferenceFind` answers the citation dropdown for typed text and writes nothing.
The trim, the byline check, the split and the cap are the reference clerk's.
`CCardReferenceRead` lists every Source in author order.
A failed read shows the load failure and answers no Sources.

`LCardRegisterRead` and `LCardTagRead` are the one owner of those maps, read by the tenor and taxonomy rows.
The register name goes through `CFolio.CFolioStateRead`.

### Tags, situations and registers

`CCardTagRemove`, `CCardSituationRemove` and `CCardRegisterRemove` drop one chip and hold no rule of their own.
The typed gates of those rows live in `CCaret`, which places each chip at the caret.

### Citation

`CCardCitationSet` points the sentence's citation at the Reference the engine resolves the typed title to.
One chip quill call resolves and cites, since the order of those steps is a data rule.
A failure shows the create failure and leaves the citation as it was.

## `CCaret`

The caret of the chip rows of a card, kept in Conduct so no view holds it.
It owns one `CCaretTrail` each for situation, tag, register and translation, and `CCaretRead` answers all four.
The editor builds it over its desk and its `CCard`.
The add, settle and insert gates read the insert position from the row's trail, so the view sends none.
A settle differs from an add only in handing the text as settled.
With no chip quill the gates answer the text and an empty, closed dropdown.
The situation, tag and register gates are shaped alike and hold no rule of their own.
They answer the kept text and the dropdown of stored rows that text matches.
The field hands every change unsettled, and Enter or leaving hands the text settled.
A failed add shows `Input.UpdateFailed` every time and answers the typed text with an empty, closed dropdown.
A failed search after the add shows `Input.FindFailed` once until the user acts.
It answers the kept text with an empty, closed dropdown.
`CCaret` takes the atelier's repaint memory at build and reads through the desk's `LDeskRepaintRead`.
The held draft's language seeds the register search, so the driver passes none.
The translation gates forward to `CCard` with the trail's position, and keep that gate's failure notices.

## `CCaretTrail`

The caret inside one row of a card's chips, as the ids that lie at and after it.
The row is read from the held draft every time, so the trail never keeps a stale row.
A new tenure clears every kept trail.
The caret sits after the last chip until the user moves it, and a trail never leaves the row.
A trail holds ids, not an index.
So a chip added or removed elsewhere leaves the caret before the same chip.
When every kept id has gone, the caret falls to the row's end.
`CCaretTrailMove` answers false when the step would leave the row, and keeps the caret.
Its anchor is the chip right of the caret, or null at the end.
`CCaretTrailDelete` removes the chip left of the caret for a negative step and right of it for a positive one.
It sends nothing outside the row.

## `CCardField`

The fields of the held draft's cards: title, expression, definition and meaning links.
It keeps no draft state, only its desk, two ports and the envoy.
The ports and envoy serve only the sense list read.
`CCardSenseAttach` and `CCardSenseDetach` edit the links of a collocation card and take no edit on an empty or filling desk.
`CCardSenseRead` lists the held entry's meanings at any depth for the link picker.
An unnamed meaning is worded `Display.Unknown`.
A failed scan shows `Input.LoadFailed` and answers an empty list, so the picker still opens.

## `CCardList`

The two card lists share these gates for adding, removing, moving and folding whole cards.
Every draft rule sits in `LDraftClerkCard`, reached through one `LQuillCard` call per gate.
The fold gate goes to the card port instead, since a fold is view state and no edit.
The new card's place is the clerk's default, not a count the view hands in.
A card alone in its list is kept by the clerk's rule, so the view sends every press.

`CCardFoldToggle` names the held entry by its stored id, and a draft never stored sends nothing.
It never dirties the draft, makes a revision or enters undo.
The port raises the fold bulletin, and `CEntryDraftChanged` then hands every card its new fold.
True means the port returned without throwing, not that a stored row changed.
A draft never stored and a throwing port call both answer false, and the caller puts its chevron back.
The verdict comes from the one call alone, with no read-back.
A failed write shows `Fold.SaveFailed` every time, since the user acted.

The two `CCardMove` overloads are one action heard in two raw forms, so the view parses nothing.
A drag hands a place, the index in the list `CEntryDraftChanged` last handed out.
The gate maps it through `CFolio.CFolioPlaceRead`, and a place outside the list sends nothing.
The quill asks the card clerk to judge the engine index, so a stale card or unchanged place sends nothing.
The badge hands raw typed text, which `CFolio.CFolioOrdinalRead` clamps to the list's ends.
The engine's own ordinal rule counts its raw list, which may not be in position order.
Text that is no number sends nothing.

`CCardPositionOpen` marks a card as being moved and answers true only when its list holds more than one card.
A card alone in its list, or a desk with no held draft, answers false and marks nothing.
`CCardPositionClose` drops the mark and is safe for a card never marked.
`CCardPositionSettle` takes the typed ordinal only for a marked card, and removes the mark in the same call.
An unmarked card answers false and sends nothing, so a badge cannot settle twice.
A marked card moves through the ordinal `CCardMove`, so the clamp and the number check stay in one place.
A null ordinal throws.

## `CCardSpeech`

The draft's parts of speech while the user builds the list.
The chips live in the draft, below Conduct, and the speech quill reads them for every edit.
It holds only the raw text being typed, which the read overlays on the draft's chips.
`CCardSpeechSet` keeps the text, defers it as a pending part, and answers the category menu.
With no tenure held it answers an empty, closed menu.
A failed lookup shows `Input.FindFailed` once until the user acts, and answers an empty, closed menu.
`CCardSpeechAdd` clears the typed text, while `CCardSpeechRemove` leaves it pending.
A failed add shows `Input.SpeechFailed` every time, and the typed text stays cleared.
`CCardSpeechRead` shows the chips and typed text as the quill settles them.
A failed read shows `Input.FindFailed` once and answers no chips with the typed text kept.
A draft changed from elsewhere shows its own chips and clears the typed text.
The unit and freedom rows are worded through static port members, so no port is held.
`LUnitRowParse` and `LFreedomRowParse` throw on a key the menu never offered, a programming error.

## `CFolio`

The one map from an engine entry draft to the shape every driver shows.
It is split from `CCard` since the gates and the maps change for different reasons.
`CFolioTargetRead`, `CFolioGlossRead`, `CFolioImageRead` and `CFolioVideoRead` are shared with the reading view, repertoire and mention map.
`CFolioEntryRead` receives link targets keyed by card and folded card ids from one store read.
It also receives picture and video addresses through `media`, and the meaning scan.
So each card carries its links, fold and media ready.

`CFolioStateRead` carries the engine's text and verdicts, so no driver judges a state.
The empty text for a value with nothing legible is the engine's own, so the map keeps no fallback.

`CFolioSheetRead` shapes the meaning or collocation cards.
The tenure answers an entry of targets for every card, so a missing one is a fault and throws.
Only the collocation sheet passes `expressed` as true, so a meaning card shows its expression as unspecified.
A card's situation link carries no media, so its picture and video lists stay empty.
Links join the linked sense ids to `senses`, so a link to a vanished meaning is dropped.
A linked meaning carries depth zero, since the chip shows only its name and path.
Fold state comes from `folds`, while fold eligibility comes from `LCardDraftStored`, true for a stored card.

`CFolioOrderRead` is the one order, by the engine's position with ties kept in the engine's order.
The sheet map, the place map, the reading view and the contents share it, so those paths agree.
`CFolioPlaceRead` turns a handed place into the engine index for the same card, and answers null outside the list.
`CFolioOrdinalRead` counts from one and pulls a number past either end to that end.
It answers a place, not an engine index, so the badge number and the shown order agree.
Text that is not a whole number answers null.
`CFolioListRead` is the list holding `cardId`, the collocations when no meaning holds it.
`CFolioCountRead` is the length of that list, so the position gate asks the same list the move uses.

A sentence row with no example yet words the empty text, so its field shows the example hint.
Images and videos carry the engine's empty verdict, so a reading card folds a picture nobody located.
The address or screen comes from `media`, and videos carry the moments the engine reads from the span.
The engine settles what may be reached, and the map only hands it the plain text.

## `CSentence`

The sentence gates of the editor, one per user action on a card's sentence rows.
Each gate holds the interaction only and calls one ShellEngine member on the held draft.
The rules about the rows live in the Application clerk, so no gate trims or clamps.
The add gate alone checks its row index, since Conduct defines what that index means.

`CSentenceReferenceChanged` fires on the driver's thread through the editor's marshal.
`LSentenceObserverAttach` hears the reference subject on every tenure the desk starts.
`CSentenceAdd` takes `below`, the index of the pressed row in the card's sentences.
`CFolio` maps sentences in the engine's order, so that index is the engine's.
A stale or foreign index sends nothing, and the new row takes the next place.
`CSentenceCitationSet` carries a stored Source's id, so nothing is resolved from its byline.
A typed title is `CCard.CCardCitationSet`'s action instead.
Gloss text is deferred, a gloss language pick is sent at once, and the engine picks a new gloss's language.

`CSentenceMentionAdd` takes the raw pick, and a null id shows `Refusal.TargetMissing` and links nothing.
Null never means silence, since a fresh pick and a silence are different user actions.
The span rule and request are ShellEngine's, shared with the corpus transcript's gate.
`CSentenceSilenceSet` has its own gate.
So Conduct maps the silence to the engine's Entry 0, and the driver never sends it.
`CSentenceSenseSet` lets ShellEngine persist pending typing and narrow the Mention under the selection.
The mention and sense checks read the held draft even while the desk fills.
`CSentenceSenseRead` answers null when no linked Mention lies under the selection, so no menu opens.
The shared read in `CMention` chooses the fallback `Display.Unknown`.
A failed read shows `Mention.FindFailed` and answers null.

`CSentenceMentionRead` answers one entry for every sentence of the held cards, and empty means no chips.
A failed read shows `Mention.FindFailed` once and answers an empty list for every sentence.
With no draft held it answers no entries.
Each unlinked chip carries its own key, so the driver passes no silent word.
`CSentenceFrameRead` answers the word order, particles and dependences in the draft's language.
A failed particle or dependence read leaves that list empty, so the editor still opens.
It shows `Sentence.ParticleFailed` or `Sentence.DependenceFailed`.

## `CSentenceFrame`

The frame of the sentence rows in one language.
Its lists are empty when none are known.

## Stamp

Commit: `2f9a06b8eb95`
- `CCard.cs` `73aa7858dc403d5e`
- `CCardField.cs` `1c45a2b59a846aaf`
- `CCardList.cs` `676fda163fb37d36`
- `CCardSpeech.cs` `c33bd983a66825f2`
- `CCaret.cs` `cf2769876b6c7b6e`
- `CCaretTrail.cs` `0ac8e0459942d0a7`
- `CFolio.cs` `9146a2e633f8019f`
- `CSentence.cs` `ea7e08ddebc389f9`
- `CSentenceFrame.cs` `dcb9e108a9b63eb9`
