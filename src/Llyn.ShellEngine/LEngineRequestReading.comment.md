# LEngineRequestReading.cs

## `public sealed partial class LEngine`

The pronunciation and transcription rows of a draft.
Each is an ordered list the draft holds by id, edited one request at a time like the lists inside a card.
A row is added blank or with what the form already has, and filled afterwards.
So a blank row is held and named, and the commit leaves it out.

## `private LEntryDraft LEngineReadingApply(LEntryDraft content, LRequest request)`

Routes every reading request to its handler, and hands any other request on to the card lists.
`LRequestIpa` and `LRequestAudio` edit the primary row alone, for a form that shows one reading.

## `private LEntryDraft LEnginePrimaryChange(`

Applies a change to the first pronunciation row, minting that row when the list is empty.
The reading and the recording are two requests, and either may arrive first.

## `private LEntryDraft LEnginePronunciationAdd(LEntryDraft content, LRequestPronunciationAddition request)`

A new pronunciation row with the reading sent and a minted id, at the place asked for.

## `private static LEntryDraft LEnginePronunciationChange(`

Changes the pronunciation row named, and refuses when the draft holds none by that id.

## `private LEntryDraft LEngineTranscriptionAdd(LEntryDraft content, LRequestTranscriptionAddition request)`

A new transcription row for the scheme sent and a minted id, at the place asked for.
A scheme the draft already carries is refused before the row is made.

## `private static LEntryDraft LEngineSchemeChange(LEntryDraft content, LRequestTranscriptionScheme request)`

Renames the scheme of one transcription row, refusing a name another row already carries.

## `private static void LEngineSchemeValidate(IReadOnlyList<LTranscriptionDraft> drafts, string scheme, long ownId)`

The one-scheme rule at request time, so the form learns of a doubled scheme as it is typed rather than at commit.
A blank scheme passes, because a row still being named collides with nothing.

## `private static LEntryDraft LEngineTranscriptionChange(`

Changes the transcription row named, and refuses when the draft holds none by that id.
