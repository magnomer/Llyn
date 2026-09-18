# LEngineDraftSituation.cs

## `public sealed partial class LEngine`

The held-draft calls that belong to a Situation rather than to an entry or a sentence.
A Situation is independent data owned by nothing, so a situation draft names a Situation and carries no entry.
It sits apart from `LEngineDraftHold.cs`, which owns what every kind of draft shares.
Only starting, saving and committing differ by kind, and only those live here.
The shared claim, sweep, recovery and discard read `LDraftSituation` and branch where the kinds part.
This follows `LEngineDraftExample.cs` call for call, so a third kind cannot invent a third protocol.

## `internal LDraft LEngineSituationStart(string origin, long? situationId)`

Mints a draft id, writes the first file, and returns the held Situation.
With no Situation the content is blank under an id minted for it, which is a context being written first.
With a Situation the content is that Situation read back, which is an edit.
The Situation id is given here rather than at the store.
A recovered draft names the same context it always did.
A Situation that is gone is refused before a file is written, so no draft can point at nothing.
A claim naming this process is written beside the draft, as every other kind of draft does.

## `internal LSituation LEngineSituationCommit(long id)`

Turns a held Situation into a stored one and returns it.
A draft naming no Situation is a create, one naming a Situation is a rewrite.
A Situation id naming a record since deleted is a create as well, because there is nothing left to rewrite.
The database write runs first and whole, so a refusal from it leaves the file exactly as it was.
The write and the revision recording it share one session, so the history never misses a stored context.
The draft file is rewritten with the stored id and the stored content the moment that write returns.
A kill between the two writes would otherwise leave the Situation stored and the file still nameless.
Recommitting such a file would store the context twice, and the sweep would never collect it.
The held file is deleted last, so a failure anywhere above leaves the work recoverable.
Nothing settles a court here, because a Situation links to no tentative record.

## `private bool LEngineSituationCheck(LDraft draft, LSituation held)`

Whether a held Situation differs from the one it was started from.
A draft naming no Situation is measured against an empty one.
A draft whose Situation has since gone is measured against that same empty one.
This is the domain rule the repertoire panel used to keep in its own comparison.

## `private LSituation LEngineSituationNormalize(LSituation content)`

The same Situation with an id minted when it carries none.
Its image and video rows are named the same way, and a row with no location is dropped.
The Situation that came in is returned itself when it was already named and its rows were too.
That sameness is the answer the form reads to know nothing was corrected.
A Situation written by an older launch is what arrives here unnamed.

## `private static LSituation LEngineSituationBlank`

What a draft naming no Situation is measured against.
A context that was never opened on a stored one started from nothing.

## `private static bool LEngineSituationMatch(LSituation one, LSituation other)`

Field by field, whether two Situations say the same thing.
Identity is left out, because a held Situation is named before the record it becomes exists.
The title, the description and the kind each count, so any of them typed is an edit.
The image and video lists count as well, so an added picture is an edit.
A blank row is not, because the commit drops it.
