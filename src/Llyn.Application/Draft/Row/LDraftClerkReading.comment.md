# LDraftClerkReading.cs

## `public sealed class LDraftClerkReading`

The pronunciation and transcription rows of a draft.
Each is an ordered list the draft holds by id.
It is edited one request at a time like the lists inside a card.
A row is added blank or with what the form already has, and filled afterwards.
So a blank row is held and named, and the commit leaves it out.

## `public LDraftClerkReading(LLanguageCache languages, LIdentity identity)`

Holds the pack cache a respelling is derived through and the issuer that names a new row.

## `public LEntryDraft? LReadingApply(LEntryDraft content, LRequest request)`

Routes every reading request to its handler, and answers null for any other request.
The clerk then hands that request on to the reflex rows.
`LRequestIpa` and `LRequestAudio` edit the primary row alone, for a form that shows one reading.
`LRequestRespelling` and `LRequestPronunciationRespelling` write the respelling alone and derive nothing.

## `public static LEntryDraft LAudioClear(LEntryDraft content, LEntryDraft? stored)`

Empties the audio and source of every pronunciation row whose recording is not the stored row's.
A recording is audio of one word in one language.
The engine calls this after a headword or language change, with the stored entry or nothing.
No stored entry drops every recording, which is what a new language deserves.
A stored entry keeps its own recordings and drops only the ones fetched this draft.
So a typo corrected in a stored headword keeps the entry's audio.

## `private static bool LAudioMatch(LPronunciationDraft row, LEntryDraft? stored)`

Whether the row's recording may stay.
A row without one has nothing to lose.
No stored entry means nothing may stay.
A row whose id names a stored row keeps the recording that row already carries.
Any other recording was fetched this draft and is fresh.

## `private LEntryDraft LPronunciationPrimaryChange(`

Applies a change to the first pronunciation row, minting that row when the list is empty.
The reading and the recording are two requests, and either may arrive first.
The minted row is seeded, because the primary field stands on the form before anything was typed.
So it counts as a pronunciation only once it carries something.

## `private LEntryDraft LPronunciationAdd(LEntryDraft content, LRequestPronunciationAddition request)`

A new pronunciation row with the reading sent and a minted id, at the place asked for.
Its respelling is derived from that reading at once.

## `private static LEntryDraft LPronunciationChange(`

Changes the pronunciation row named, and refuses when the draft holds none by that id.

## `private LEntryDraft LTranscriptionAdd(LEntryDraft content, LRequestTranscriptionAddition request)`

A new transcription row for the scheme sent and a minted id, at the place asked for.
A scheme the draft already carries is refused before the row is made.

## `private static LEntryDraft LSchemeChange(LEntryDraft content, LRequestTranscriptionScheme request)`

Renames the scheme of one transcription row, refusing a name another row already carries.

## `private static void LSchemeValidate(IReadOnlyList<LTranscriptionDraft> drafts, string scheme, long ownId)`

The one-scheme rule at request time.
The form learns of a doubled scheme as it is typed rather than at commit.
A blank scheme passes, because a row still being named collides with nothing.

## `private static LEntryDraft LTranscriptionChange(`

Changes the transcription row named, and refuses when the draft holds none by that id.
