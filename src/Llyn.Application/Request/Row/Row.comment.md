# Row

The edit requests for the ordered lists and sub-records of a held draft.
They cover the etymology map boxes, the example, glosses, mentions, pronunciations, reflexes, registers, sentences, transcriptions and translations.
They stand apart from the entry requests, which edit the entry's own fields and cards.
The field requests key `LRequestKey` by type and row id, so a deferred edit replaces only its own row.
Each list request names its row by id.
So a change lands on that row wherever the list has moved it.

## `LRequestAncestor.cs`

These requests are one per gesture of the etymology map editor.
Each names the draft and the box by id, real or minted.
Text, language, gloss and relation key by box, so a deferred edit replaces only its own box's field.
Target and the structural requests keep the type key, since the tenure applies them at once.
Insertion and addition put an empty box above or beside the one named.
Id zero in an insertion names the headword itself.
Removal drops the box and its sources move down to its child.
Target links the box to an entry, and Restoration undoes the link while its set-aside sources are held.
Language names a tongue by id, or by text when the tongue id is zero.
Relation id zero clears the relation.

## `LRequestExample.cs`

These are the requests of the example panel, one per field of the sentence a draft holds on its own.
They name only the draft, because a draft holds one such sentence.
A sentence inside a card is edited through the sentence requests instead.
The reference request names the cited source, or zero to cite nothing.

## `LRequestGloss.cs`

These requests are one per gesture a shown sentence offers on its renderings.
Every one names the card and the sentence by id, real or minted.
Card id 0 and sentence id 0 together name the draft's own Example, the one the corpus panel holds.
So one set of four records serves the card editor and the corpus panel alike.

### `LRequestGlossAddition`

The position is clamped into the list.
The engine mints the negative id, so the form never carries one of its own.

### `LRequestGlossRemoval`

A Gloss no row carries is refused.

## `LRequestMention.cs`

These requests are one per gesture a shown sentence offers on one of its words.
They name the card and the sentence as the gloss requests do.
They use the same zero convention for the corpus panel.
So one set of three records serves both editors.

### `LRequestMentionAddition`

The word is given by offset and length in code points.
Entry id 0 marks the word as standing for nothing, and sense id 0 chooses no Meaning.
A span overlapping an existing Mention replaces it.
So the form has one gesture and the engine decides whether it is new or a change.
A span past the end of the text is refused.
So is an Entry no row carries, or a Meaning of another Entry.

### `LRequestMentionSense`

Sense id 0 clears the choice.
A Mention standing for nothing has no Entry to narrow, so the request is refused on it.

## `LRequestPronunciation.cs`

These requests change the entry's ordered pronunciation list.
`LRequestIpa` and `LRequestAudio` remain for a form that edits the primary row alone.

### `LRequestPronunciationRemoval`

The recording goes with the row.
Id zero names the primary row, and nothing happens when there is none.

### `LRequestPronunciationShift`

The row moved to the front becomes the primary one.

### `LRequestPronunciationIpa`

It leaves the row's recording and stored detail alone.
The engine derives the row's respelling from the new reading again, so a hand-written respelling is replaced.

### `LRequestPronunciationRespelling`

It leaves the original reading as it stands.
The form sends it in place of `LRequestPronunciationIpa` while the respelling switch is on.
The value stands until the reading or the variety changes and the engine derives the respelling again.

### `LRequestPronunciationVariety`

The label tells one row from the others, such as a region.
The respelling is derived again, because the pack's respelling groups are scoped by variety.

### `LRequestPronunciationAudio`

An empty file clears the recording.

## `LRequestReflex.cs`

These requests change the entry's ordered reflex list.

### `LRequestReflexAddition`

The form copies the language and kind of the row the plus was pressed on.
So a sibling reading needs no retyping.

### `LRequestReflexText`

The engine derives the row's respelling from the new reading again, so a hand-written respelling is replaced.

### `LRequestReflexRespelling`

It leaves the reading as it stands.
The form sends it in place of `LRequestReflexText` while the respelling switch shows that row's language respelled.

### `LRequestReflexMeaning`

It also marks the meaning as entered by the user.

### `LRequestReflexMain`

It marks or unmarks the row as the reading in common use.

### `LRequestReflexAnchor`

It ties the row to the named fanqie row, or unties it.
One request per tick, so the other anchors of the row and the other rows stand untouched.

## `LRequestRegister.cs`

The register requests are shaped like the situation requests.
A register is a shared row, so its name is edited by row id and not by card.
The engine mints an added register's id, and commit creates the row in the entry's language.
Pick copies the stored row under its positive id.

## `LRequestSentence.cs`

These requests are one per structural change and one per field of a sentence row.
Every one names the card and the sentence by id, real or minted.
The addition alone names no sentence, because it has none yet.
The example a sentence shows is edited through the sentence request.
The form never sees an example apart from its row.

### `LRequestSentenceAddition`

The position is clamped to the list.
The engine mints the row's id and gives it no example.
The example appears with the first text that arrives for the row.

### `LRequestSentenceExample`

It picks an existing example for the sentence.
The engine copies the row's content under its positive id, replacing whatever example the sentence had.

### `LRequestSentenceText`

On a sentence with no example the first non-empty text mints one.
On an example with a positive id it edits the draft copy, and commit updates the row.

### `LRequestSentenceReference`

A zero source cites nothing.
On a sentence with no example a non-zero source mints one.

## `LRequestTranscription.cs`

These requests change the entry's ordered transcription list.

### `LRequestTranscriptionAddition`

The engine refuses a scheme the entry already carries, because one scheme spells the reading one way.
The `LRequestSeeded` flag marks the row the form offers on its own when the entry has none.
A seeded row is no change and is not stored until it is filled.
A row the user added is kept even empty.

### `LRequestTranscriptionScheme`

The engine refuses a name another row of the entry already carries.

## `LRequestTranslation.cs`

A translation links only to an existing entry or to a draft the court opened, so there is no addition.
The id is the entry's, or the target draft's until that draft commits and the court settles it.
Pick leaves an entry the card already links as it is.
Removal leaves a court link behind it for the form to close.

## Stamp

Commit: `c6e60141a172`
- `LRequestAncestor.cs` `e0928f51c6dbf32c`
- `LRequestExample.cs` `e9de61e4d141ceb2`
- `LRequestGloss.cs` `5ff67c89dcfaff57`
- `LRequestMention.cs` `c201672700398580`
- `LRequestPronunciation.cs` `0db0bd99c329efb9`
- `LRequestReflex.cs` `422cdc5cc80320ba`
- `LRequestRegister.cs` `89dc8d8bedcdd41f`
- `LRequestSentence.cs` `4b45d9994fc5773f`
- `LRequestTranscription.cs` `9238d000b827e9b5`
- `LRequestTranslation.cs` `48562c54fd98ef49`
