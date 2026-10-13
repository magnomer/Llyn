# Draft

This folder holds the archive relays for drafts, sample drafts and the in-memory claim, court and draft stores.

## TInterfaceCourt

The relays for the draft-folder archives.
They hand the workspace folder its draft, court and claim files and read them back.
`TCourtCreate` builds one court row, for a fake port to answer.
`TClaimCreate` builds one claim.
`TCourtArchiveRead` alone picks its row from a scan.
Every other relay passes one archive call through.

## TInterfaceDraft

Sample drafts the draft tests build from, shared so every file reads the same shape.
The card builder carries nothing but a title, for order that is read by title alone.
The nested draft has enough nesting to prove the whole shape survives the file.
The plain draft has one meaning and nothing nested, a light base for tests that write draft content.
The link builder makes a court link with a fresh id, for tests that seed the register directly.
The match relay is the draft equality the save and the dirty check share.
The reflex draft builder makes an English entry with one plain meaning and the given reflex rows.
Both reflex test classes build their entries through it, so the rows are the only variable.

## `TVaultFakeClaim`

An in-memory claim folder keyed by draft id, so a clerk test can pose as one process and then another.
Every claim is live, since no process is checked, and a new claim names the process the fake was told.
The constructor seats the process of the test's first rig.
`TClaimProcessSet` changes it, for a second rig posing as another copy of the program.

## `TVaultFakeCourt`

An in-memory court keyed by link id.
A clerk test can link a draft to a tentative target on it and cancel the draft.
Settling answers and removes the rows pointing at the target, as the archive does.
The sweep drops nothing, since the fake holds no files of another version.

## `TVaultFakeDraft`

An in-memory drafts folder keyed by draft id.
A clerk test can hold and finish a draft on it without a workspace.
The sweep answers the ids a test marked stale, as the archive answers the files of another version.
`TDraftStaleSet` marks one draft as what the next sweep drops.

## Stamp

Commit: `c2f335c331fe`
- `TInterfaceCourt.cs` `8e00b31340846d57`
- `TInterfaceDraft.cs` `eb4c15bc2bf700ab`
- `TVaultFakeClaim.cs` `df75b32eafaf645b`
- `TVaultFakeCourt.cs` `fd8a0700210e2d85`
- `TVaultFakeDraft.cs` `d96b3d7a5a5022e9`
