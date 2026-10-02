# CAuthor.cs
Hash: `5f9d35007ec20787`

## `public sealed record CAuthor(long CAuthorId, string CAuthorLead, string CAuthorMark, string CAuthorTail);`

One author the byline dropdown offers for a typed credit, its name already split around the typed word.

**Parameters**

- `CAuthorId`: the stored author.
- `CAuthorLead`: the name before the match, or the whole name when the word is not found in it.
- `CAuthorMark`: the matched part of the name, empty when the word is not found in it.
- `CAuthorTail`: the name after the match.
