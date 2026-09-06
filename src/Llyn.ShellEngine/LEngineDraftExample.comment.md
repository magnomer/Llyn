# LEngineDraftExample.cs

## `public sealed partial class LEngine`

The held-draft calls that belong to a sentence rather than to an entry.
An Example is independent data owned by nothing, so a sentence draft names an Example and carries no entry.
It sits apart from `LEngineDraftHold.cs`, which owns what every kind of draft shares.
Only starting, saving and committing differ by kind, and only those live here.
The shared claim, sweep, recovery and discard read `LDraftExample` and branch where the kinds part.

## `public LDraft LEngineExampleStart(string origin, string? exampleId)`

Mints a draft id, writes the first file, and returns the held sentence.
With no Example the content is blank under an id minted for it, which is a sentence being written for the first time.
With an Example the content is that Example read back, which is an edit.
The sentence id is given here rather than at the store, so a recovered draft names the same sentence it always did.
An Example that is gone is refused before a file is written, so no draft can point at nothing.
A claim naming this process is written beside the draft, as every other kind of draft does.

## `public LExample LEngineExampleSave(LDraft draft)`

Overwrites that one file with the sentence given, and hands back the sentence it stored.
The content that came in is handed straight back when nothing needed naming.
So the caller can tell a settled write from a corrected one without comparing field by field.
A draft carrying no sentence is refused, because this call is the sentence side of the folder.

## `public LExample LEngineExampleCommit(string id)`

Turns a held sentence into a stored Example and returns it.
A draft naming no Example is a create, one naming an Example is a rewrite.
An Example id naming a record since deleted is a create as well, because there is nothing left to rewrite.
The database write runs first and whole, so a refusal from it leaves the file exactly as it was.
The draft file is rewritten with the stored id and the stored sentence the moment that write returns.
A kill between the two writes would otherwise leave the Example stored and the file still nameless.
Recommitting such a file would store the sentence twice, and the sweep would never collect it.
The held file is deleted last, so a failure anywhere above leaves the work recoverable.
Nothing settles a court here, because a sentence links to no tentative record.

## `private bool LEngineExampleCheck(LDraft draft, LExample held)`

Whether a held sentence differs from the Example it was started from.
A draft naming no Example is measured against an empty sentence in the language the draft already carries.
The language a new sentence opens in was chosen for it rather than typed, so it is not by itself an edit.
A draft whose Example has since gone is measured against that same empty sentence.

## `private static LExample LEngineExampleNormalize(LExample content)`

The same sentence with an id minted when it carries none.
The sentence that came in is returned itself when it was already named.
That sameness is the answer the form reads to know nothing was corrected.
A sentence written by an older launch is what arrives here unnamed.

## `private static LExample LEngineExampleBlank`

What a draft naming no Example is measured against.
A sentence that was never opened on an Example started from nothing.

## `private static bool LEngineExampleMatch(LExample one, LExample other)`

Field by field, whether two sentences say the same thing.
Identity is left out, because a held sentence is named before the Example it becomes exists.
The language counts, because the tongue a sentence is written in is part of the sentence.
The cited Source counts too, so recitation is a change like any other.
