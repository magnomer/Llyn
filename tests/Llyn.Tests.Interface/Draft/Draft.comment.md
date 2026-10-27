# Draft

This folder holds the archive relays for drafts, sample drafts and the in-memory dossier.

## TInterfaceCourt

The relays for the draft-folder archives.
They hand the workspace folder its draft, court and claim files and read them back.
Draft and court relays go through the dossier port, as production does.
The claim relays read and write the claim files as raw JSON.
So a test can plant a claim of any process.
`TCourtCreate` builds one court row, for a fake port to answer.
`TCourtArchiveRead` alone picks its row from a scan.
Every other relay passes one call through.

## TInterfaceDraft

Sample drafts the draft tests build from, shared so every file reads the same shape.
The card builder carries nothing but a title, for order that is read by title alone.
The nested draft has enough nesting to prove the whole shape survives the file.
The plain draft has one meaning and nothing nested, a light base for tests that write draft content.
The link builder makes a court link with a fresh id, for tests that seed the register directly.
The match relay is the draft equality the save and the dirty check share.
The reflex draft builder makes an English entry with one plain meaning and the given reflex rows.
Both reflex test classes build their entries through it, so the rows are the only variable.

## `TVaultFakeDraft`

An in-memory dossier keyed by draft id.
A clerk test can hold, claim, link and finish drafts on it without a workspace.
It carries the claims and the court the real dossier keeps, since one port covers all three.
The constructor seats the process of the test's first rig, and a new claim names that process.
`TClaimProcessSet` changes it, for a second rig posing as another copy of the program.
Every claim is live, since no process is checked.
Settling rewrites the owner drafts and answers them, and removes the rows pointing at the target, as the dossier does.
The sweep drops the ids a test marked stale, cancels each, and drops the claims left without a draft.
`TDraftStaleSet` marks one draft as what the next sweep drops.
`TDraftDoomedSet` names a draft that the next delete finishes first, and then throws, as a cascade failing midway would.

## Stamp

Commit: `1f3467bbe5c6`
- `TInterfaceCourt.cs` `8f3034314b82e5d4`
- `TInterfaceDraft.cs` `eb4c15bc2bf700ab`
- `TVaultFakeDraft.cs` `417f2add702df5ec`
