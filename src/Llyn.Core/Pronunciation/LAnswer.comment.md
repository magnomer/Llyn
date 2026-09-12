# LAnswer.cs

## `public sealed record LAnswer(IReadOnlyList<LReading> LAnswerReadings, bool LAnswerReached)`

What one source gave back for a word.
It separates a source that answered with nothing from one that never answered at all.
A menu can then say "No entry" where it means that, and "Failed to retrieve" where a host was unreachable.
Returning a bare list could not tell the two apart.
A dead source read as a word no dictionary carries.
The same record serves the reader below it, where the value is a page body rather than a transcription.

**Parameters**

- `LAnswerReadings` — The extracted readings, each tagged with its variety, and empty when there are none.
  A pronunciation source may return several, one per variety the page lists.
  An audio source or a page reader returns one untagged reading holding an address or a body.
- `LAnswerReached` — Whether the source answered at all.
  False means every URL failed: a timeout, a refused connection, or a 5xx that outlived its retries.
  True with no readings means the source answered and simply had nothing for this word.

## `public static LAnswer LAnswerLost { get; }`

The source was never reached.

## `public static LAnswer LAnswerBlank { get; }`

The source answered and had nothing.

## `public static LAnswer LAnswerCreate(IReadOnlyList<LReading> readings)`

The source answered with `readings`.

## `public static LAnswer LAnswerCreate(string value)`

The source answered with one untagged reading holding `value`.
Audio harvest and the page reader take this form, since neither carries a variety.

## `public string? LAnswerValue`

The first reading's form, or null when there is none.
It serves the callers that need a single string, such as audio harvest.

## `public bool LAnswerEmpty`

Whether there is a reading to take, whatever the source's fate was.
