# Row

Row clerks apply requests to the repeated rows of an entry draft or a card draft.
Each clerk owns one kind of row and answers null for any request it does not own.
The draft clerk then tries the next row clerk.
A row is added blank or with what the form already has, and filled afterwards.
So a blank row is held and named, and the commit leaves it out.
Every list edit refuses a row id the draft does not hold.

## `LDraftClerkEtymology`

Applies the etymology requests to the entry draft being edited.
It keeps both etymology shapes on the draft and picks neither, which the entry clerk does when saving.
It holds the entry store to check that a link or a span names a real Entry.
The identity mints the id of every span the clerk puts down.

### `LEtymologyTextApply`

Sets the prose and carries the spans across the edit.
`LDraftClerkMention` does the carrying, so etymology prose and sentence text shift alike.

### `LEtymologyMentionApply`

Puts one span over the range given, or clears the range when no Entry is named.
Every span the range touches is dropped first, so no two spans overlap.
A range outside the text is refused, as is a span naming an Entry the store does not hold.

### `LEtymonAdd`

Inserts a direct link at the position given, clamped into the list.
A link the draft already holds is refused.

### `LEtymonMove`

Moves one direct link to another place in the list.
A link the draft does not hold is refused.
`LEtymonRemove` drops the link to the Entry named and leaves the rest in order.

### `LEtymologyValidate`

Refuses an id that is not a positive id of an Entry the store holds.

### `LEtymologyLengthRead`

The length of the prose counts code points, which is what a span offset counts.

## `LDraftClerkGloss`

The Gloss requests, applied to the sentence a card row holds or to the corpus panel's own Example.
Every handler edits the Gloss list of one Example draft and hands the rest of the draft back untouched.
The only service a Gloss edit needs is the issuer that names a new Gloss.

### `LGlossApply`

Routes every Gloss request to its handler, and answers null for any other request.
The clerk then hands that request on to the situation fields.
The private overload routes one list change to the Example the request names.
Card 0 and sentence 0 name the draft's own Example, held by the corpus panel.
Its stored rows are read as drafts and written back resolved.
Any other pair names a card row, whose Example draft takes the changed list directly.

### `LGlossAdd`

Adds an empty Gloss in the requested language at the requested position, under a freshly minted negative id.

### `LGlossEmptyCheck`

Whether the draft holds that sentence's Example with no Gloss on it.
A draft without that Example answers false, so nothing is seeded into it.
The card and sentence are zero for the held Example itself.

### `LGlossChange`

The private overload is the one place a Gloss row is found by id and rewritten.
It refuses when no row carries the id.
The text overload resolves the value from what was written.
`LGlossRemove` refuses when no row carries the id.

### `LGlossRead`

The drafts resolved to Glosses, ids kept.
The engine's commit reads a row's Glosses the same way, so it calls here.

## `LDraftClerkMedia`

The image and video rows of a card or a Situation.
The list edits are written once over a bare list, so a card and a Situation share every rule.
A form adds a row with whatever location it has, often none.

### `LSituationDispatch`

Routes a media request to the image list or the video list of the Situation the draft holds.
Answers null for a draft without a Situation or for any other request.
It runs first, so a media request on a Situation draft never reaches a card.
The card id such a request carries is ignored there, because a Situation draft holds no card.

### `LImageApply`

The list overload answers the image list after one request, or null when the request is not an image one.
An addition is a new row with the location sent and a minted id, at the place asked for.
A pick copies the stored image under its own id, unless the list already holds it.
A location change on a row the list does not hold is refused as an item.
The entry overload applies the same edit to the card the request names.
A location change there reaches every card holding the picture instead.
It answers null for any other request, so the clerk hands it on to the video rows.

### `LImageChange`

Relocates the image in every card holding it, and refuses when none does.
`LImageRead` refuses when no stored image carries the id.

### `LImageEndRead`

The place after the last picture row, on the held Situation or on the card named.
A card the draft lacks answers zero, and the addition then refuses it.

### `LImageCardFind`

The card holding the picture row is found among the Meanings, their children and then the Collocations.
A child card counts, so a picture on a child can be removed like any other.
A Situation draft answers card zero, since its rows hang on the draft itself.
A row no card holds answers null.
`LVideoCardFind` finds a video's card the same way.

### `LVideoApply`

The list overload mirrors the image list, with no span on an addition.
The location and the span change through the same routine with a different lambda.
The entry overload applies the edit to the card named.
A location or span change there reaches every card holding the video.
It answers null for any other request, so the clerk refuses it as a kind it does not apply.

### `LVideoChange`

Applies one change to the video in every card holding it, and refuses when none does.
`LVideoEndRead` reads the place after the last video row as the picture end is read.

### `LVideoSpanRead`

The moments a video row plays from and until, read from its span text by the video's own rule.

## `LDraftClerkMention`

The Mentions of the Example a draft holds, on a card row or on the corpus panel.
Every handler goes through one locate routine, so the two panels differ only in where the Example lives.
The text handlers of both panels call the shift routine here, so a Mention follows the word it marks.

### `LMentionApply`

Routes every Mention request to its handler, and answers null for any other request.
The clerk then hands that request on to the Glosses.
The private overload locates the Example on the card row named.
When card and sentence are both 0, it locates the Example on the draft panel.
The panel holds an `LExample`, so its list is converted to drafts for the change and back afterwards.
A row quoting no Example has no words to link and is refused.

### `LMentionAdd`

Links one span to an Entry, replacing every Mention the span overlaps.
A span of no length or one past the end of the text is refused.
So is an Entry no row carries, or a Meaning of another Entry.
The new Mention gets a minted id and the list is sorted by offset.

### `LMentionRemove`

Drops the Mention named, and refuses when the Example holds none by that id.

### `LMentionChange`

Narrows the Mention named to a Meaning of its own Entry, or clears the choice with 0.
A Mention standing for nothing is refused, because there is no Entry to narrow.

### `LMentionValidate`

Refuses an Entry no row carries and a Meaning that does not belong to that Entry.
An Entry of 0 stands for nothing and may carry no Meaning.
A minted negative id is refused too, because a Mention points only at stored rows.

### `LMentionUpdate`

The text overloads replace the text of a row's Example, or of the corpus panel's.
They shift its Mentions to follow.
The list overload recomputes every span after the text changed from `before` to `after`.
The common prefix and the common suffix of the two texts are found in code points.
A Mention entirely inside the prefix keeps its offset.
One entirely inside the suffix shifts by the length delta.
One touching the edited middle is dropped, because the word it marked is no longer what it was.
This is right for every single-keystroke edit.
A paste over the whole sentence drops everything, which is what the user expects.
The etymology prose uses it too, so both texts carry their spans across an edit alike.

### `LMentionRead`

Converts between stored Mentions and drafts with ids kept, negative for rows not yet written.
The engine's commit resolves a row's Mentions the same way, so it calls here.
`LMentionRuneRead` splits text into code points, so offsets count what the store counts.

## `LDraftClerkReading`

The pronunciation and transcription rows of a draft.
Each is an ordered list the draft holds by id.
It holds the pack cache a respelling is derived through.

### `LReadingApply`

Routes every reading request to its handler, and answers null for any other request.
The clerk then hands that request on to the reflex rows.
`LRequestIpa` and `LRequestAudio` edit the primary row alone, for a form that shows one reading.
`LRequestRespelling` and `LRequestPronunciationRespelling` write the respelling alone and derive nothing.

### `LAudioClear`

Empties the audio and source of every pronunciation row whose recording is not the stored row's.
A recording is audio of one word in one language.
The engine calls this after a headword or language change, with the stored entry or nothing.
No stored entry drops every recording, which is what a new language deserves.
A stored entry keeps its own recordings and drops only the ones fetched this draft.
So a typo corrected in a stored headword keeps the entry's audio.

### `LAudioMatch`

Whether the row's recording may stay.
A row without one has nothing to lose.
No stored entry means nothing may stay.
A row whose id names a stored row keeps the recording that row already carries.
Any other recording was fetched this draft and is fresh.

### `LPronunciationPrimaryChange`

Applies a change to the first pronunciation row, minting that row when the list is empty.
The reading and the recording are two requests, and either may arrive first.
The minted row is seeded, because the primary field stands on the form before anything was typed.
So it counts as a pronunciation only once it carries something.

### `LPronunciationAdd`

A new pronunciation row with the reading sent and a minted id, at the place asked for.
Its respelling is derived from that reading at once.
An addition past the primary onto an empty list seeds a blank primary first.
The primary is always row zero, so the shell never has to ask.

### `LPronunciationPositionRead`

The place a new pronunciation takes when added after the row given.
Id zero names the primary, so the new row follows it.
A row the list no longer holds also answers the place after the primary.

### `LTranscriptionAdd`

A new transcription row for the scheme sent and a minted id, at the place asked for.
A scheme the draft already carries is refused before the row is made.
`LSchemeChange` renames the scheme of one row and refuses a name another row carries.

### `LSchemeValidate`

The one-scheme rule at request time.
The form learns of a doubled scheme as it is typed rather than at commit.
A blank scheme passes, because a row still being named collides with nothing.

### `LSchemeTakenCheck`

Whether a row other than the own id already holds the scheme, the one owner of the one-scheme rule.
The refusal above asks it, and the editor's scheme dropdown asks it to grey out a held scheme.

### `LTranscriptionSheetRead`

The editor's transcription block, built from the pack's schemes and the draft's rows.
It lists the `other` rows, which lie outside the glyph scheme.
Each other row carries every scheme, marked by the one-scheme rule for that row.
The block shows while the pack declares any scheme.

### `LSchemeFreeRead`

The first scheme no row holds, or null when every scheme is held.
It asks the one-scheme rule, so a new row never takes a scheme the addition would refuse.

### `LTranscriptionPositionRead`

A new row goes after the row given.
For id zero or a row gone, it goes at the end.

## `LDraftClerkReflex`

The reflex rows of a draft, an ordered list the draft holds by id.
A row is added with its language and kind and filled afterwards.
It holds the pack cache a row is respelled and cut through.

### `LReflexApply`

Routes every reflex request to its handler, and answers null for any other request.
The clerk then hands that request on to the etymology rows.
A language or text request respells the row under its own language and recuts its anatomy under the entry's language.
A respelling request keeps the respelling typed and recuts the anatomy.
A romanization or note request replaces that one text of the named row.
A meaning request also marks the row as user-owned so the meaning survives a rebuild.
No request edits the region, which the fetch and the markup import supply.
An anchor request ties or unties the named row and one fanqie row.
The row's other anchors stand as they were.

### `LReflexFind`

The list's own find matches the id, so an id of zero is refused.

### `LReflexDefaultRead`

The language and kind a row added from another row starts with.
It copies the pressed row's, and a zero or gone id starts blank.

### `LReflexPositionRead`

A row added from another row goes right below it.
A zero or gone id places it last.

## `LDraftClerkSentence`

The sentence rows of a card.
A row is born blank and named at once, so the form can address it before anything is typed.
The Example inside it is named only when it has something to say.

### `LSentenceApply`

Routes every sentence row request to its handler, and answers null for any other request.
An Example or Source request naming card or sentence zero is resolved by the clerk before it reaches here.
The clerk then hands any other request on to the situation chips.

### `LSentenceAdd`

A new row with a minted id, no Example and an empty frame, at the place asked for.
`LSentenceRemove` and `LSentenceMove` refuse when the card holds no row with the id.

### `LSentenceSelect`

Puts the stored Example named under the row, in place of whatever the row quoted.
The row's copy carries the stored id, so commit updates that row rather than making one.
The copy carries the stored Mentions under their positive ids, so a word already linked stays linked.
An id naming no stored Example is refused.

### `LExampleChange`

Applies one change to the Example a row quotes.
A row quoting nothing is offered a blank Example, and the change decides whether it becomes one.
A blank Example the change left blank is dropped again, so an empty text on an empty row is nothing.
One the change wrote into is named here, which is the moment a sentence begins to exist.

### `LSentenceChange`

Applies one change to the row named, and refuses when the card does not hold it.
The Gloss and Mention classes reach a row's Example through here.

## Stamp

Commit: `c2f335c331fe`
- `LDraftClerkEtymology.cs` `367ab59e9a8c31f1`
- `LDraftClerkGloss.cs` `92c491993bdf9de1`
- `LDraftClerkMedia.cs` `9ecb2b5075228d05`
- `LDraftClerkMention.cs` `40b3a0ded1f2c470`
- `LDraftClerkReading.cs` `5ebb0dcc2c810c99`
- `LDraftClerkReflex.cs` `6f8f9116225f8d3a`
- `LDraftClerkSentence.cs` `0e5e7ed4ec851b28`
