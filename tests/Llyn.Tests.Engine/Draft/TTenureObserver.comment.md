# TTenureObserver.cs
Hash: `48226af69615022a`

## `public sealed class TTenureObserver`

Tenure subscriptions filter bulletins by subject and the relevant identity.
Preparation suppresses notices during nested or failing changes.
Cancellation detaches subscriptions.

## `public void Prepare_RequestApplied_ReturnsNewestDraftAndDropsItsNotice()`

Applying during preparation returns the updated draft and suppresses its immediate notice.
After preparation, a matching bulletin is delivered.

## `public void Attach_SubjectAndDraftFilters_DeliverOnlyMatchingNotices()`

Subject subscriptions accept the configured global settings notice.
Draft subscriptions require the tenure's draft ID.
Unrelated subjects and IDs are ignored, and cancellation removes the observer.

## `public void EntryAttach_StoredEntryIdentity_IsDistinctFromDraftIdentity()`

An Entry subscription matches the stored Entry ID rather than the separate tenure/draft ID.

## `public void StoredRead_StoredDraft_AnswersIdWithoutPersisting()`

A stored draft answers its Entry ID.
A deferred headword stays unapplied, so the read persisted nothing.

## `public void StoredRead_RefusedDraft_AnswersNull()`

A workspace switch leaves the held draft stale, so its read is refused.
The stored-id read answers null instead of throwing.

## `public void Cancel_DiscardFails_EndsTheTenureRaisesItsStateAndThrows()`

A halted tenure whose discard fails, because a file stands where the court folder belongs, still raises its state.
The failure reaches the caller as exactly an `IOException` naming the court folder.
The tenure answers the ended state, no longer halted.
The court folder is restored right after the cancel, even when the assertion fails.
So the engine and workspace dispose over a sound tree.

## `public void Prepare_NestedNotices_DropsEveryOneUntilOutermostScopeExits(bool fails)`

Nested preparation suppresses matching notices through the outermost scope, even when it throws.
Later notices still arrive.
