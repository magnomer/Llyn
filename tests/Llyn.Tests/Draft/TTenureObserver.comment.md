# TTenureObserver.cs

## `public sealed class TTenureObserver`

Tenure subscriptions filter bulletins by subject and the relevant identity.
Preparation suppresses notices during nested or failing changes.
Cancellation detaches subscriptions.

## `Prepare_RequestApplied_ReturnsNewestDraftAndDropsItsNotice()`

Applying during preparation returns the updated draft and suppresses its immediate notice.
After preparation, a matching bulletin is delivered.

## `Attach_SubjectAndDraftFilters_DeliverOnlyMatchingNotices()`

Subject subscriptions accept the configured global settings notice.
Draft subscriptions require the tenure's draft ID.
Unrelated subjects and IDs are ignored, and cancellation removes the observer.

## `EntryAttach_StoredEntryIdentity_IsDistinctFromDraftIdentity()`

An Entry subscription matches the stored Entry ID rather than the separate tenure/draft ID.

## `Prepare_NestedNotices_DropsEveryOneUntilOutermostScopeExits(bool fails)`

Nested preparation suppresses matching notices through the outermost scope, even when it throws.
Later notices still arrive.
