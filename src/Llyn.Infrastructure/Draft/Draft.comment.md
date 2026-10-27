# Draft

This folder keeps tentative records as plain files under the workspace drafts folder.
A record the user has not committed must never appear in the database, so nothing here reaches SQLite.
Files are the cheapest store that survives a forced shutdown, and a folder listing is all recovery needs.
Three archives share one discipline.
Two copies of the program may run against one workspace.
So every operation touches a single named file and holds nothing open.
The folders are never locked, and a file this process did not write is never removed.

Every save writes a .json.tmp file first and then moves it over the target.
A move is atomic, so a reader never sees a half-written file.
A crash mid-write leaves the previous file intact.
Every scan skips a file that fails to parse or cannot be opened rather than throwing.
Recovery lists leftovers after a crash, which is exactly when a truncated file is likely.
One bad file must not hide the rest.
Every delete treats a file already gone, or held open by the other copy, as no error.
A save killed between write and move leaves a pending file that listings ignore.
Nothing else collects it, so each sweep does.
A pending file younger than an hour may belong to a save in flight, so the sweep leaves it.

## `LDossierEntrance`

The one entrance of the drafts in flight, answering Core's `LDraftVault` for one workspace.
It keeps the drafts, the claims and the court consistent.
A claim or a link without its draft is invalid.
Its members are explicit implementations of the port, so its public surface is the constructor alone.
The three archives behind it are internal and carry no Core interface.
The process a claim names is this process, as `LClaimCreate` takes it.
The callers already hold the engine gate, so the entrance adds no lock.

`LDraftDelete` and `LDraftCancel` run `LCourtRemove`, and cancelling then settles the rows pointing at the draft to nothing.
Both end with the claim and the file, and answer an `LDraftRelease`.
The release lists every draft gone, the draft itself last, and every owner draft rewritten.
The caller drops those from its held set and its chronicle.

`LCourtRemove` drops every court row one draft owns, and the tentative target each row named when nothing else wants it.
A target another draft still links to stays, because that draft's chip would otherwise point at nothing.
A target goes only while this process claims it and the caller's held set holds it.
So another window's editor is never emptied.
A target whose claim is gone was left by an earlier launch, and its own recovery will offer it back.
A row whose owner draft is gone is dropped as well, wherever the sweep meets one.

`LDraftSweep` sweeps the draft and court files and cancels each draft of another version.
It then drops each claim whose draft file is gone.

`LDraftCourtSettle` settles the rows pointing at a draft and rewrites each owner through Core's `LEntryDraft.LEntryDraftSettle`.
It answers the owners it rewrote.
An owner whose file is gone is skipped.

## `LClaim`

The claim record, internal to this folder, since no caller outside the entrance reads a claim.

## `LDraftArchive`

The draft file store behind `LDossierEntrance`, keeping one file per draft named after its id.

## `LDraftArchiveIndent`

The one option set both directions share, indented on write.
Its resolver drops every property without a setter.
So a derived value such as the draft's IPA never lands in the file.
The records in Core carry no serializer attribute for that, since the serializer is this adapter's alone.
`LDraftArchiveNormalize` trims the contract by hand because the serializer's read-only switch keeps get-only collections.

## `LDraftSave`

Writes the draft to drafts/`<LDraftId>`.json, replacing whatever was there.
The file is stamped with the archive's version before it is written.
The draft handed in is not changed, so a caller keeps working with the value it holds.

The save refuses a draft in which any item still carries id zero.
Positive means a stored row and negative means an id the engine minted, so zero is never a saved state.
Only the engine mints, and this guard means no caller can slip past it.
A credited author is an item too, and so are the image and video rows of a held Situation.
The ancestor boxes of an entry draft are items too, so a box of id zero refuses the save.
The Meaning ids a collocation card links to are checked the same way.
The entry id of the draft itself may be zero, because that names a new record rather than an item.

## `LDraftArchiveVersion`

The shape of the draft file the current build writes.
It moves whenever a draft-shaped record changes, so an older file is skipped rather than misread.
Version two added the credited authors a source draft carries.
Version three made the pronunciation a list and added the transcriptions beside it.
Version five added the etymology an entry draft carries, with its spans and its links.
Version six follows the migration that gave Meanings and Collocations one id space.
An older draft may name a Collocation by the number it had before, so it is set aside.
Version seven replaced the entry draft's etymology with its list of ancestor boxes.
An older draft is set aside rather than loaded without them.

## `LDraftRead`

Returns the draft stored under the id, or null when no readable file holds it.
A missing file, an unknown one and a file of another version are the same answer.
A draft the current build did not write is not a draft.

## `LDraftScan`

Returns every draft the folder holds, skipping files that fail to parse or open.

## `LDraftDelete`

Removes the file for an id, which is how a draft ends once its record is saved or abandoned.

## `LDraftSweep`

Deletes every half-written .json.tmp in the drafts folder that is older than an hour.
It also sets aside every draft file of another version and returns the ids they carried.
The entrance cancels each returned id, which drops its claim and its court rows.
A file that does not read as a draft at all is of no version.
It is set aside but not returned.
Nothing else can name such a file.
No draft is deleted.
A draft written by another build is still the user's work, and an upgrade must not wipe it.
A set-aside file moves under drafts/broken, where the listing never looks.
A file that cannot be read this round is left where it is.
A passing lock is not a broken draft.
A missing folder is not an error, and neither is a file that vanishes mid-sweep.

## `LDraftBrokenSave`

Moves one unreadable or foreign-version file under drafts/broken and reports whether it moved.
A name already taken there gets the moment appended, so an earlier copy is never overwritten.
A file that cannot be moved stays and is reported as kept, so its id is not dropped.

## `LDraftArchiveParse`

Reads one draft out of its JSON text, or nothing when the text is not a draft.
The checking flag refuses a draft of another version, as every reader but the sweep does.

## `LClaimArchive`

Keeps the claims running programs hold on tentative records, one file per draft in drafts/claim.
A claim on disk lets a second launch tell live work from what a crash left behind.
An in-memory set answers that only for the copy of the program holding it.
The folder sits under drafts because a claim means nothing once the draft it names is gone.
Files are named after draft ids, so a listing of the drafts folder never picks them up.

## `LClaimCreate`

Makes a claim on a draft naming the process running this code and the instant it started.
The start time travels with the id because the operating system hands a freed id to the next program.
It is stored as the universal instant, so a workspace read across a daylight-saving change still recognises its own claims.
Nothing is written here, so a caller can compose a claim without touching the workspace.

## `LClaimRead`

Returns the claim held on a draft, or null when no readable file holds one.
No claim and an unknown claim are the same answer, because nobody can be shown to hold the draft.
`LClaimArchiveLoad` reads one file back, or null when it is missing, truncated or unknown.

## `LClaimDelete`

Drops the claim on a draft, which is how a hold ends once the draft is committed, cancelled or deleted.

## `LClaimCheck`

Says whether a running program still holds the draft.
True demands all three.
A claim exists, its process is running, and that process started when the claim says.
A claim whose process is gone is what a crash leaves behind.
So is one whose id now belongs to a later program.
Such a claim is deleted here rather than re-examined at every launch.
So the answer doubles as the sweep, and a stale hold never outlives the first question asked about it.

## `LClaimArchiveMatch`

Says whether the process a claim names is the one that wrote it.
The start times are compared with a second of slack.
The operating system reports them at coarser resolution than they are stored.
Both sides are taken as universal instants, so the comparison does not move when the shown clock does.
A process that cannot be found, has exited, or refuses to say when it started answers no.
A claim nothing can confirm must not hold work back forever.

## `LCourtArchive`

Keeps the court as plain files in drafts/court, one file per tentative link named after its id.
A translation may name an entry the user has not written yet, and that link cannot be a database row.
Writing a real row and deleting it on discard leaks rows whenever the program dies first.
A link waits here instead until the record it points at becomes real or is dropped.
A link to an entry that already exists is never written here, because an ordinary id needs no register.
Nothing here edits a draft file.

## `LCourtArchiveVersion`

Stamped on every saved link.
A link of any other version reads as nothing, and the sweep deletes it.
So raising it abandons every tentative link a workspace holds.

## `LCourtSettle`

Settles every link pointing at a draft, whether that draft became a real entry or was abandoned.
The link files disappear and the links themselves are returned.
Each returned link names the draft that owns it, so the entrance can rewrite that draft's stored content.
Rewriting is not done here, because the draft archive owns the draft file.
Resolution and cancellation were two names for this one deletion.
The real id one of them demanded was never stored anywhere.
The caller already knows what to put in place of the tentative id.

## `LCourtSweep`

Deletes every half-written .json.tmp in the court older than an hour.
It also deletes every link file of another version, or of no readable shape, which a read already skips.
A missing folder is not an error, and neither is a file that vanishes mid-sweep.

## Inline notes

### LDraftArchive.cs: the extension check inside the listing

A Windows search pattern can match a file whose extension merely starts with the one asked for.
The half-written .json.tmp file must never be read as a draft.
The claim and court listings carry the same check for their own files.

### LDraftArchive.cs: the age check inside the sweep

A pending file younger than the hour may belong to a save still in flight.
Taking it would break a write the other copy of the program is making.

## Stamp

Commit: `1f3467bbe5c6`
- `LClaim.cs` `bdeb202449d99380`
- `LClaimArchive.cs` `b40097f5fc67e90a`
- `LCourtArchive.cs` `ae05d267e5d64588`
- `LDossierEntrance.cs` `348ed36de8fcdf39`
- `LDraftArchive.cs` `48759eb8160760dd`
