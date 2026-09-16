# LRequestReflex.cs

The reflex requests, one per change to the entry's ordered reflex list.
Each names the row by its id, so a change lands on that row wherever the list has moved it.

## `public sealed record LRequestReflexAddition(`

Adds a new reflex row at `LRequestPosition` for `LRequestLanguage` and `LRequestKind`, its text still to be typed.
The form copies the language and kind of the row the plus was pressed on.
So a sibling reading needs no retyping.

## `public sealed record LRequestReflexRemoval(long LRequestDraftId, long LRequestReflexId)`

Removes one reflex row.

## `public sealed record LRequestReflexLanguage(long LRequestDraftId, long LRequestReflexId, string LRequestText)`

Replaces the borrowing language of one row.

## `public sealed record LRequestReflexKind(long LRequestDraftId, long LRequestReflexId, string LRequestText)`

Replaces the kind of one row.

## `public sealed record LRequestReflexText(long LRequestDraftId, long LRequestReflexId, string LRequestText)`

Replaces the reading of one row.
The engine derives the row's respelling from the new reading again, so a hand-written respelling is replaced.

## `public sealed record LRequestReflexRespelling(long LRequestDraftId, long LRequestReflexId, string LRequestText)`

Replaces the respelling of one row alone, leaving the reading as it stands.
The form sends it in place of `LRequestReflexText` while the respelling switch shows that row's language respelled.

## `public sealed record LRequestReflexNote(long LRequestDraftId, long LRequestReflexId, string LRequestText)`

Replaces the note of one row.

## `public sealed record LRequestReflexRemark(long LRequestDraftId, long LRequestReflexId, string LRequestText)`

Replaces the remark of one row.

## `public sealed record LRequestReflexMain(long LRequestDraftId, long LRequestReflexId, bool LRequestMain)`

Marks or unmarks one row as the reading in common use.

## `public sealed record LRequestReflexAnchor(`

Ties one row to the fanqie row `LRequestFanqieId` names when `LRequestAnchored`, and unties it otherwise.
One request per tick, so the other anchors of the row and the other rows stand untouched.

## `public sealed record LRequestReflexAnchor(`

Ties one row to the fanqie row `LRequestFanqieId` names when `LRequestAnchored`, and unties it otherwise.
One request per tick, so the other anchors of the row and the other rows stand untouched.
