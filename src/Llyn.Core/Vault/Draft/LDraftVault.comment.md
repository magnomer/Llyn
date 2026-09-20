# LDraftVault.cs

## `public interface LDraftVault`

The persistence port for drafts, the editable copies the engine holds between load and save.
`LDraftArchive` in Infrastructure is its adapter over the workspace draft folder.
The engine asks by draft id and never learns where or how a draft is kept.

## `void LDraftSave(LDraft draft);`

Stores `draft` under its id, replacing any earlier copy.

## `LDraft? LDraftRead(long id);`

Reads the draft with `id`, or `null` when none is stored or the stored one is unreadable.

## `IReadOnlyList<LDraft> LDraftScan();`

Reads every readable draft in the workspace.

## `void LDraftDelete(long id);`

Removes the draft with `id`, and stays quiet when it is already gone.

## `IReadOnlyList<long> LDraftSweep();`

Drops stale half-written drafts and moves unreadable ones aside.
Returns the ids of the drafts moved aside.
