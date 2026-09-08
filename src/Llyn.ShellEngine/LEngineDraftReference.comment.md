# LEngineDraftReference.cs

## `public sealed partial class LEngine`

The held-draft calls that belong to a Reference rather than to an entry, a sentence or a situation.
A Reference is independent data owned by nothing, so a source draft names a Reference and carries no entry.
It sits apart from `LEngineDraftHold.cs`, which owns what every kind of draft shares.
Only starting, saving and committing differ by kind, and only those live here.
The shared claim, sweep, recovery and discard read `LDraftReference` and branch where the kinds part.
This follows `LEngineDraftSituation.cs` call for call, so the fourth kind cannot invent a fourth protocol.
The Authors a Reference credits are attached to the stored record and never held here.
A source nothing has stored yet credits nobody.
The panel offers that control only on a stored one.

## `public LDraft LEngineReferenceStart(string origin, string? referenceId)`

Mints a draft id, writes the first file, and returns the held Reference.
With no Reference the content is blank under an id minted for it, which is a source being written first.
With a Reference the content is that Reference read back, which is an edit.
The Reference id is given here rather than at the store.
A recovered draft names the same source it always did.
A Reference that is gone is refused before a file is written, so no draft can point at nothing.
A claim naming this process is written beside the draft, as every other kind of draft does.

## `public LReference LEngineReferenceSave(LDraft draft)`

Overwrites that one file with the Reference given, and hands back the Reference it stored.
The content that came in is handed straight back when nothing needed naming.
So the caller can tell a settled write from a corrected one without comparing field by field.
A draft carrying no Reference is refused, because this call is the source side of the folder.

## `public LReference LEngineReferenceCommit(string id)`

Turns a held Reference into a stored one and returns it.
A draft naming no Reference is a create, one naming a Reference is a rewrite.
A Reference id naming a record since deleted is a create as well, because there is nothing left to rewrite.
The database write runs first and whole, so a refusal from it leaves the file exactly as it was.
The draft file is rewritten with the stored id and the stored content the moment that write returns.
A kill between the two writes would otherwise leave the Reference stored and the file still nameless.
Recommitting such a file would store the source twice, and the sweep would never collect it.
The held file is deleted last, so a failure anywhere above leaves the work recoverable.
Nothing settles a court here, because a Reference links to no tentative record.

## `private bool LEngineReferenceCheck(LDraft draft, LReference held)`

Whether a held Reference differs from the one it was started from.
A draft naming no Reference is measured against an empty one.
A draft whose Reference has since gone is measured against that same empty one.
This is the domain rule the imprint panel used to keep in its own comparison.

## `private static LReference LEngineReferenceNormalize(LReference content)`

The same Reference with an id minted when it carries none.
The Reference that came in is returned itself when it was already named.
That sameness is the answer the form reads to know nothing was corrected.
A Reference written by an older launch is what arrives here unnamed.

## `private static LReference LEngineReferenceBlank`

What a draft naming no Reference is measured against.
A source that was never opened on a stored one started from nothing.
Its author state is unspecified, because nobody has been credited or ruled unreadable yet.

## `private static bool LEngineReferenceMatch(LReference one, LReference other)`

Field by field, whether two References say the same thing.
Identity is left out, because a held Reference is named before the record it becomes exists.
The title, the programme, the channel, the year and the address each count.
The author state counts too, because ruling the writers unreadable is an edit nothing else records.
