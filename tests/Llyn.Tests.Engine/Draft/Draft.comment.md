# Draft

Tests of held drafts: the unsaved work kept outside the database, its edits, its recovery and its commit.
They stand apart because they pin what must never be lost or stored twice across a crash.
The court beneath the drafts folder holds links to tentative targets and is exercised through these calls.

## `TClaim.cs`

Covers the claim files, the record on disk of which running program holds which tentative draft.
An in-memory set answers for one copy of the program only, and two copies may run on one workspace.
Where the holder must be gone, the tests spawn a real child process.
A claim is only worth anything if the operating system agrees the holder is alive.
A claim naming a live process with its true start time holds the draft and is never swept.
A claim naming an exited process is swept with its file, which is what a forced shutdown leaves.
A claim naming a live process with another start time is stale too.
The operating system hands a freed id to the next program.
The id alone would let it hold work hostage.
A draft nobody claims reports nothing held.
A draft another running copy claims is not offered as a leftover, and becomes one once that copy is gone.
Committing or cancelling a draft removes its claim, since nothing asks about a draft that is gone.

## `TClaimClerk.cs`

Covers the claim clerk over a rig of fakes, posing as one process and then another over the same folder.
A started draft is held, is this clerk's to take away, and reads back.
A clerk posing as another process sees the first claim as foreign and holds nothing itself.
Finishing a draft empties the held set, the file and the claim.
Sweeping a stale draft cancels it with its claim, and the live draft beside it is kept.
Cancelling the only draft linking a tentative target takes the row and the target with it.
Cancelling one of two drafts linking a target drops that draft's row and leaves the target held.

## `TDraftArchive.cs`

Covers the drafts folder, which keeps unsaved work outside the database, and the court register beneath it.
The court records links pointing at records that are still tentative.
Tests read the files back through the archive alone.
A saved draft comes back field for field, nested lists and state values included.
A shallow round trip would pass while losing what the user typed.
The file carries the stored fields and none of the derived ones, which the serializer option decides.
A Situation draft keeps its pictures and clips, ids and spans included, so a crash loses no picture.
The listing returns every draft written, since recovery is useless if it misses any.
A file holding invalid JSON is skipped instead of throwing.
A crash mid-write is when a truncated file appears, which is when the listing matters most.
Settling a target drops every waiting link and its file, and returns the links with their owners.
The engine needs the owners to rewrite the drafts holding those links.
Settling leaves links to other targets, because the court is shared by every pending link.
Settling the same target twice returns nothing the second time.
A saved link keeps its headword and language, which are shown before the target is real.

## `TDraftClerk.cs`

Covers the draft clerk on its own, over a rig of fakes and no engine.
The engine tests drive the same code through the facade and remain the regression net.
A headword request rewrites the headword, and the draft handed in is untouched.
A tag added by text lands first in the card's list under a minted negative id.
A tag's rewritten text is held trimmed.
Text still being typed with no comma completes nothing and is kept as typed.
A typed list hands each completed part trimmed, skips blanks, and keeps a refused part before the rest.
A settled list completes its last part too, and a blank settled list hands nothing.
A second transcription row under a scheme the draft already carries is refused, and the first stands.
The gloss-empty check answers true only for an Example holding no Gloss.
A request kind no switch knows throws an argument error and not a refusal.

## `TDraftExample.cs`

Covers the sentence side of the drafts folder, the held work the corpus panel pushes into.
A sentence draft carries an Example where an entry draft carries entry content.
A sentence typed and then lost with its launch is still on disk, and recovery offers it back.
The panel used to hold the sentence in its controls alone.
A committed sentence leaves no file and nothing to recover.
The sweep collects a draft matching the Example it names and keeps one naming no stored Example.
A kill just after a commit leaves such a file.
Only the sweep can tell it from work the user would lose.
An Example's language is optional, since a sentence may be written in two languages.
So a sentence committed without one is stored without one.

## `TDraftLeftover.cs`

Covers what a launch finds in the drafts folder that no open window holds.
A leftover is offered back, swept when the entry already has it, or set aside when it cannot be read.
An engine never offers back the drafts it started itself.
A fresh engine over the same folder offers every one left changed, which is the shape of a crash.
An untouched blank draft is not counted, because a blank form costs nothing to lose.
A draft matching the entry it names is swept, and is not reported, because the file is gone.
Only the sweep collects it, since a kill just after a commit leaves it.
The typed draft beside it survives and is still offered back.
A blank draft naming no entry is swept with its claim.
Every panel starts one at launch, so a kill leaves many such files, and recovery never offers them.
A draft naming no entry but holding text survives.
Starting a workspace sweeps blank drafts first, and the state it answers equals a fresh read after the sweep.
The shell reads that state at launch, so it must not predate the sweep.
A claim file whose draft is gone is deleted, since nothing asks about a draft that is gone.
A draft file stamped with another version is not read.
It is set aside under `drafts/broken`, with its claim and the links naming it dropped.
An old field rename would otherwise read back naming no entry and commit as a duplicate.
The draft owning the swept link is of the current shape and stays.
A file that does not read as a draft is moved aside byte for byte and never deleted.
It is the user's work in some shape, and the next build may read it again.
The listing does not look under `broken`, so it is not offered as a leftover.
A half-written `.json.tmp` older than an hour is deleted from the drafts folder and the court.
A newer one stays, since a save in another copy of the program may be in flight.

## `TDraftReference.cs`

Covers the source side of the drafts folder, the held work the imprint panel pushes into.
A source draft carries a Reference where a situation draft carries a Situation.
A source typed and then lost with its launch is still on disk, and recovery offers it back.
A committed source leaves no file, since keeping it would offer saved work back at every launch.
The sweep collects a draft matching the Reference it names and keeps one naming no stored Reference.

## `TDraftSituation.cs`

Covers the situation side of the drafts folder, the held work the repertoire panel pushes into.
A context typed and then lost with its launch is still on disk, and recovery offers it back.
A committed context leaves no file and nothing to recover.
The sweep collects a draft matching the Situation it names and keeps one naming no stored Situation.
A picture and a clip added to a held Situation count as a change.
The commit stores both, and the next start reads them back.
The reopened draft is unchanged, so the stored lists and the held lists agree.

## `TIdentity.cs`

Covers the identity rule for drafts.
Every item id is positive or negative, and only the engine mints.
Two new sentences with one wording are two items.
Each has its own negative id and its own row on commit.
Matching by text would have collapsed them.
An item picked from the database keeps its positive id, and the commit reuses that row and mints nothing.
Every negative id the saved draft held is a key of the outcome map.
Every value is a stored row.
A draft naming a stored row that was since deleted is refused rather than rebound.
The old path made a fresh row from the chip's text, and the card silently pointed somewhere new.
The refusal names the link, writes nothing, and the draft file stays for the user to mend.
A wording typed into a chip is looked up by the engine before it is added.
A Situation or Register held under that wording is picked by id, with case and edge spaces folded.
So the form and any other client get one row per wording, and the shelf grows no twins.
A wording nothing matches is minted as new, and a later draft typing it finds that row.
The drafts folder refuses to write a draft whose item still carries zero.
So writing the file directly cannot bypass the rule.

## `TSpeechClerk.cs`

Covers the part of speech rules with no engine.
Typed text is appended trimmed and once, and blank text adds nothing.
A declared name carries its catalog value, and a refused one is kept trimmed with no value.
A blank or held name declares nothing.
An erased chip drops that part only.
A draft that agrees keeps the pending text, and one changed elsewhere is cleaned and clears it.
Only a typed text that appends a part is pending, and the chips leave the pending last part out.
The catalog's parts are found by the trimmed text without case and marked when held.
A blank text offers every part closed, and an empty catalog or a text matching none has its own verdict.

## Inline notes

### TClaim.cs: TClaimDraftCreate

An untouched draft is not a leftover whatever claims it.
So the content has to differ from the blank draft it started as.

### TClaim.cs: TClaimProcessStart

The child stays alive until killed and redirects its input, so it waits rather than reading the runner's console.

### TClaim.cs: TClaimProcessStop

It kills the child and waits for the operating system to release its id.
The start time read before the kill gives a process that certainly existed and now does not.

### TIdentity.cs: TIdentityContentCreate

The card carries a negative id from the test counter, not one the engine minted.

## Stamp

Commit: `c2f335c331fe`
- `TClaim.cs` `3717bbbfffe16b67`
- `TClaimClerk.cs` `79fcef1e200d2842`
- `TDraftArchive.cs` `adeba8a2994531ab`
- `TDraftClerk.cs` `280dc232c5a117a6`
- `TDraftExample.cs` `3f1d3998d7a49532`
- `TDraftLeftover.cs` `9e4e68a23d143a0c`
- `TDraftReference.cs` `32f442cd70178839`
- `TDraftSituation.cs` `25860dbb392ccf0b`
- `TIdentity.cs` `91d8f92b072ca101`
- `TSpeechClerk.cs` `d4b2191233d1245f`
