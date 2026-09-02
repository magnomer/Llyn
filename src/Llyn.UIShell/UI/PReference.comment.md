# PReference.cs

## `internal sealed class PReference`

One Source offered in a row's Source list: the id the row cites when this one is picked, and the name shown for it. The list is the workspace's whole shelf of Sources, so the same row object is offered to every Example and Situation row on the form.

## `internal static PReference PReferenceCreate(LReference reference)`

Reads a stored Source into the row that offers it. A Source is shown by the first field it actually states — title, then program, then channel, then url — because every field may stand unspecified or unknown; a Source that names itself nowhere is offered under its id rather than as a blank row the user could not tell from the next one.
