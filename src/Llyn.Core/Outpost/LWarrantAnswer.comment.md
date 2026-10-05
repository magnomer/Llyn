# LWarrantAnswer.cs
Hash: `62ad2b48ebd61fe0`

## `public sealed record LWarrantAnswer(LWarrantState LWarrantAnswerState, string? LWarrantAnswerToken);`

Joplin's reply to one poll of a pending token request.

**Parameters**

- `LWarrantAnswerState` — Whether the user has yet to decide, accepted or rejected the request.
- `LWarrantAnswerToken` — The granted token, present only when the request was accepted.
