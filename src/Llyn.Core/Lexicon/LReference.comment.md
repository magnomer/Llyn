# LReference.cs

## `public sealed record LReference(`

One bibliographic Reference — the material an Entry or an Example cites.
A Reference is independent: no Entry, Example, or Author owns it, and it owns none of them.
Entries reach it through ordered reference rows and an Example through its single source column.
Deleting a citing row never touches the Reference.
Deleting the Reference never touches its citers.

There is no source-type discriminator.
One shape describes an article, a television episode, a video, or a picture alike.
The fields that mean something for the material at hand are specified.
The rest stay `LState.LStateUnspecified`.
Each field is an `LReferenceValue` so "never entered", "recorded as unknown", and "this value" stay distinct.
`LReferenceAuthorState` is that same distinction for the authorship as a whole.
The Authors themselves are separate rows attached in order.
So a specified authorship may name one Author, several, or the specified Author `Anonymous`.

**Parameters**

- `LReferenceId` — Opaque, program-generated stable id.
- `LReferenceTitle` — Title of the material, and for a television episode the episode name.
- `LReferenceProgram` — Program name, when the material belongs to one.
- `LReferenceChannel` — Channel name, when the material belongs to one.
- `LReferenceYear` — Publication or release year.
- `LReferenceUrl` — Address the material was found at.
- `LReferenceAuthorState` — Whether the authorship is unspecified, unknown, or specified by attached Authors.
