# LRequestReference.cs
Hash: `c3e01736e791b25c`

The requests of the sources panel.
There is one per stated field of the source a draft holds.
They name only the draft, because a draft holds one source.
The credited authors have their own family beside this one.

## `public sealed record LRequestReferenceTitle(long LRequestDraftId, LStateWritten LRequestValue)`

Replaces the title.

## `public sealed record LRequestReferenceYear(long LRequestDraftId, LStateWritten LRequestValue)`

Replaces the year.

## `public sealed record LRequestReferenceKind(long LRequestDraftId, LReferenceKind LRequestKind)`

Replaces the kind.
The kind travels as its own enum rather than as a state value.
So nothing is parsed on the way in.

## `public sealed record LRequestReferenceNote(long LRequestDraftId, LStateWritten LRequestValue)`

Replaces the note.

## `public sealed record LRequestReferenceUrl(long LRequestDraftId, LStateWritten LRequestValue)`

Replaces the url.
