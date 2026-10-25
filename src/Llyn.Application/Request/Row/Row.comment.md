# Row

The edit requests for the ordered lists and sub-records of a held draft.
They cover the etymology map boxes, pronunciations, reflexes, registers and transcriptions.
The example, sentence, gloss, mention and translation requests live in the sibling `Example` folder.
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
Addition, pick, removal and shift name the card by kind and id.
The engine mints an added register's id, and commit creates the row in the entry's language.
Pick copies the stored row under its positive id.

## `LRequestTranscription.cs`

These requests change the entry's ordered transcription list.

### `LRequestTranscriptionAddition`

The engine refuses a scheme the entry already carries, because one scheme spells the reading one way.
The `LRequestSeeded` flag marks the row the form offers on its own when the entry has none.
A seeded row is no change and is not stored until it is filled.
A row the user added is kept even empty.

### `LRequestTranscriptionScheme`

The engine refuses a name another row of the entry already carries.

## Stamp

Commit: `8c403d1df768`
- `LRequestAncestor.cs` `e0928f51c6dbf32c`
- `LRequestPronunciation.cs` `0db0bd99c329efb9`
- `LRequestReflex.cs` `422cdc5cc80320ba`
- `LRequestRegister.cs` `22d3fca0ab388745`
- `LRequestTranscription.cs` `9238d000b827e9b5`
