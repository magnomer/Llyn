# LAnswer.cs

## `public sealed record LAnswer(string? LAnswerValue, bool LAnswerReached)`

What one source gave back for a word.
It separates a source that answered with nothing from one that never answered at all.
A menu can then say "No entry" where it means that, and "Failed to retrieve" where a host was unreachable.
Returning a bare string could not tell the two apart.
A dead source read as a word no dictionary carries.
The same record serves the reader below it, where the value is a page body rather than a transcription.

**Parameters**

- `LAnswerValue` — The extracted value, or null when there is none.
  It is a phonetic form for a pronunciation source and an audio address for an audio source.
- `LAnswerReached` — Whether the source answered at all.
  False means every URL failed: a timeout, a refused connection, or a 5xx that outlived its retries.
  True with no value means the source answered and simply had nothing for this word.

## `public static LAnswer LAnswerLost { get; }`

The source was never reached.

## `public static LAnswer LAnswerBlank { get; }`

The source answered and had nothing.

## `public static LAnswer LAnswerCreate(string value)`

The source answered with `value`.

## `public bool LAnswerEmpty`

Whether there is a value to take, whatever the source's fate was.
