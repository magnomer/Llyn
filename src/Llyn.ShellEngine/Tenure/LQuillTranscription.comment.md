# LQuillTranscription.cs
Hash: `a15074db3fbc86b6`

## `public sealed class LQuillTranscription`

The edits of the held entry's transcription rows, each building exactly one request.
It also reads the sheet those rows paint from, since the add takes its free scheme.

## `private readonly LTenure _lQuillTranscriptionTenure;`

The tenure every request is built for and handed to.

## `public LQuillTranscription(LTenure tenure)`

Builds the edits over one tenure, which they never swap.

## `public LTranscriptionSheet? LQuillTranscriptionRead()`

The held draft's transcription rows outside the glyph scheme, each with the schemes its pack declares.
The engine answers the pack's schemes and the glyph split, and the clerk builds the sheet from them.
The clerk marks each scheme taken per row and names the first scheme still free.
An ended tenure holds no draft, so it answers nothing.

## `public void LQuillTranscriptionSet(long transcription, string text)`

Defers the text typed into the transcription row `transcription`, the glyph row among them.
Every typing driver of a transcription text writes through it, so typing has one builder.

## `public void LTranscriptionSchemeSet(long transcription, string scheme)`

Switches the transcription row `transcription` to the scheme the user picked.
A pick is one deliberate act, so it applies at once.
The clerk refuses a scheme another row holds.

## `public void LQuillTranscriptionAdd(long transcription)`

Adds a blank row after the row `transcription`, or at the end for id zero or a row gone.
The new row takes the first scheme no row holds, as `LQuillTranscriptionRead` answers it.
Nothing is added while every scheme is held.

## `public void LQuillTranscriptionRemove(long transcription)`

Removes the transcription row `transcription` at once.
