# LEngineUpdate.cs

## `public sealed partial class LEngine`

The two write paths that make an entry out of a draft.
The save creates and the update changes.
Both are facades over `LEntryClerk`, which holds the rules and the vaults.
What stays here is what only the engine holds.
That is the gate, the draft normalization, the draft match, the pending fetch and the frequency fill.
The transcriptions and the reflexes are still reconciled here, since their sync reads the source factory.
Plan 16 gives them a clerk, and the facade calls into that clerk instead.
Until then the facade holds the outer session across the clerk and its own two syncs.
A nested session on the same thread joins the outer one, so a commit is still whole or nothing.

## `internal LEntry LEngineEntrySave(LEntryDraft draft)`

Saves the whole input form as one new entry and returns it with its assigned id and timestamps.
A blank headword is refused before any connection opens, so a save that cannot be made costs nothing.
The draft is normalized before anything is written, so an item still carrying id zero is named here.
Only the engine mints, and normalizing here means no caller can slip a zero past it.
The respellings are derived through the language cache on the way in.
The clerk writes the entry, and the transcriptions and reflexes follow in the same session.
A background frequency fill starts once the commit is through, which `LEngineFrequency.cs` owns.

## `internal LEntry LEngineEntryUpdate(long id, LEntryDraft draft)`

Applies `draft` to the entry identified by `id` and returns the stored entry as it now stands.
An id no entry carries is refused before anything is written.
The form may have been opened on an entry that has since been deleted.
Falling back to creating a copy is exactly the defect this seam exists to remove.
The revision is recorded once every part has added its changes, so the history says what an edit did.
Saving the same draft again records no revision, so the history holds only rounds that changed something.
A changed headword or language starts a fresh frequency fill after the commit.

## `private LEntry LEngineEntryUpdate(long id, LEntryDraft draft, Dictionary<long, long> identity, List<LRevisionChange> changes)`

The same reconciliation with the revision left to the caller.
Every change is appended to `changes` and no revision is recorded.
The markup import fills many entries under one revision through this seam.
The draft is matched against the entry as loaded, and the clerk moves the stamp only when they differ.
The pending inflection fetch is cancelled before the clerk rewrites the forms.

## `private void LEngineRevisionRecord(long target, string subject, string kind, string? summary)`

Records one revision holding a single change through the clerk.
The sentence, situation and source commits each record their round through this.

## `private void LEngineUpdatedSet(long entryId)`

Moves the entry's `updated_utc` to now, through the clerk.
Every seam that changes one part of an entry outside the draft path calls this.

## `private void LEngineUpdatedSet(long ownerId, bool collocation)`

The same, reached from a card rather than the entry, through the card clerk.

## `private static void LEngineFieldSync<LEngineRow, LEngineWritten>(IEnumerable<LEngineWritten> written, IReadOnlyList<LEngineRow> attached, Func<LEngineRow, long> identify, Func<LEngineWritten, long> resolve, Action<long> detach, Action<long, int> attach)`

The shared field reconciliation, forwarded for the situation part that still runs it here.
