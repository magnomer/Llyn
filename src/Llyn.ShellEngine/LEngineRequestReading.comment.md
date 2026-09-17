# LEngineRequestReading.cs

## `public sealed partial class LEngine`

The pronunciation and transcription rows of a draft.
Each is an ordered list the draft holds by id.
It is edited one request at a time like the lists inside a card.
A row is added blank or with what the form already has, and filled afterwards.
So a blank row is held and named, and the commit leaves it out.

## `private LEntryDraft LEngineReadingApply(LEntryDraft content, LRequest request)`

Routes every reading request to its handler, and hands any other request on to the reflex rows.
`LRequestIpa` and `LRequestAudio` edit the primary row alone, for a form that shows one reading.
`LRequestRespelling` and `LRequestPronunciationRespelling` write the respelling alone and derive nothing.

## `private LDraft LEngineAudioClear(LDraft draft, LRequest request)`

Applies a headword or language request, then drops the recordings that no longer fit the draft.
A recording is audio of one word in one language.
A new language makes every recording on the entry audio in the wrong language, so all of them go.
A new spelling drops only a recording fetched this draft, since a stored one is the entry's own.
So a typo corrected in a stored headword keeps the entry's audio.
A request that leaves the field as it was drops nothing, so a form may send it on every pause.
The stored entry is read through the draft's entry id, and a draft started on nothing has none.

## `private static LEntryDraft LEngineAudioClear(LEntryDraft content, LEntryDraft? stored)`

Empties the audio and source of every pronunciation row whose recording is not the stored row's.

## `private static bool LEngineAudioMatch(LPronunciationDraft row, LEntryDraft? stored)`

Whether the row's recording may stay.
A row without one has nothing to lose.
No stored entry means nothing may stay.
A row whose id names a stored row keeps the recording that row already carries.
Any other recording was fetched this draft and is fresh.

## `private LEntryDraft LEnginePrimaryChange(`

Applies a change to the first pronunciation row, minting that row when the list is empty.
The reading and the recording are two requests, and either may arrive first.
The minted row is seeded, because the primary field stands on the form before anything was typed.
So it counts as a pronunciation only once it carries something.

## `private LEntryDraft LEnginePronunciationAdd(LEntryDraft content, LRequestPronunciationAddition request)`

A new pronunciation row with the reading sent and a minted id, at the place asked for.
Its respelling is derived from that reading at once.

## `private LPronunciationDraft LEngineRespellingResolve(string language, LPronunciationDraft spoken)`

Fills the row's respelling from its reading through the pack's respelling groups, scoped by its variety.
It runs whether or not the respelling switch is on, so both forms are always stored.
The switch then only picks which form is shown.
A blank reading, a blank language or a pack without groups leaves the respelling blank.
The form then shows the reading in its place.
Whatever the user wrote into the respelling by hand is replaced, because a stale respelling would silently mislead.

## `private LEntryDraft LEngineRespellingRebuild(LEntryDraft content)`

Derives every pronunciation row's respelling again, for a draft whose language changed.
The language request runs `LEngineAnatomyRebuild` over its result, so the reflex rows are recut as well.

## `private static LEntryDraft LEnginePronunciationChange(`

Changes the pronunciation row named, and refuses when the draft holds none by that id.

## `private LEntryDraft LEngineTranscriptionAdd(LEntryDraft content, LRequestTranscriptionAddition request)`

A new transcription row for the scheme sent and a minted id, at the place asked for.
A scheme the draft already carries is refused before the row is made.

## `private static LEntryDraft LEngineSchemeChange(LEntryDraft content, LRequestTranscriptionScheme request)`

Renames the scheme of one transcription row, refusing a name another row already carries.

## `private static void LEngineSchemeValidate(IReadOnlyList<LTranscriptionDraft> drafts, string scheme, long ownId)`

The one-scheme rule at request time.
The form learns of a doubled scheme as it is typed rather than at commit.
A blank scheme passes, because a row still being named collides with nothing.

## `private static LEntryDraft LEngineTranscriptionChange(`

Changes the transcription row named, and refuses when the draft holds none by that id.
