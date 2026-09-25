# LCourtVault.cs

## `public interface LCourtVault`

The persistence port for the court, the tentative links a draft holds to records that are not yet real.
A translation may name an entry the user has not written yet, and that link cannot be a database row.
A link waits here until the record it points at becomes real or is dropped.
`LCourtArchive` in Infrastructure is its adapter over the workspace draft folder.

## `void LCourtSave(LCourt link);`

Stores `link` under its id, replacing whatever was there.

## `IReadOnlyList<LCourt> LCourtScan();`

Every readable link the court holds.
An unreadable one is skipped rather than thrown, so one bad file never hides the rest.

## `void LCourtDelete(long id);`

Removes the link with `id`, and stays quiet when it is already gone.

## `IReadOnlyList<LCourt> LCourtSettle(long draftId);`

Settles every link pointing at `draftId`, whether that draft became a real entry or was abandoned.
The links leave the court and are returned, each naming the draft that owns it.
Rewriting that draft is left to the caller, because the engine owns the draft.

## `void LCourtSweep();`

Drops the half-written and unreadable leftovers of the court that no read would ever pick up.
A link another copy of the program is still writing is left alone.
