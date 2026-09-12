# LRequestReference.cs

The requests of the sources panel, one per stated field of the source a draft holds, and one for the whole body.
They name only the draft, because a draft holds one source.
The credited authors have their own family beside this one.

## `public sealed record LRequestReferenceTitle(long LRequestDraftId, LStateValue LRequestValue)`

Replaces the title.

## `public sealed record LRequestReferenceYear(long LRequestDraftId, LStateValue LRequestValue)`

Replaces the year.

## `public sealed record LRequestReferenceKind(long LRequestDraftId, LReferenceKind LRequestKind)`

Replaces the kind.
The kind travels as its own enum rather than as a state value.
So nothing is parsed on the way in.

## `public sealed record LRequestReferenceNote(long LRequestDraftId, LStateValue LRequestValue)`

Replaces the note.

## `public sealed record LRequestReferenceUrl(long LRequestDraftId, LStateValue LRequestValue)`

Replaces the url.

## `public sealed record LRequestReferenceBody(long LRequestDraftId, LReference LRequestReference)`

Carries every stated field of the source at once, as the form currently shows them.
The engine takes the title, year, kind, note, url and author state from it and keeps the held id.
A form sends this instead of comparing its controls against the draft field by field.
So the decision of what changed lives in the engine, and an unchanged body saves and announces nothing.
