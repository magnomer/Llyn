# TEstablishment.cs

## `public sealed class TEstablishment`

Covers the one read the status bar makes and the bulletin that keeps it current.

## `public void EstablishmentRead_FreshWorkspace_CountsNothingUnsaved()`

A workspace nothing has touched reports no unsaved draft, no entry and a database file of some size.
The size is above zero because the schema alone occupies pages.

## `public void EstablishmentRead_TypedDraft_CountsOneUnsaved()`

A draft with a headword typed into it counts, and a draft left blank does not.
The blank one is still held, so being held is not what counts.

## `public void EstablishmentRead_CommittedDraft_CountsEntryNotUnsaved()`

Committing moves the draft out of the unsaved count and into the entry count.

## `public void DraftCancel_TypedDraft_RaisesDraftBulletinWithZeroId()`

Cancelling raises one draft bulletin carrying id zero, and the count read after it is zero.
The bar listens for that bulletin, so a discard without it would leave the bar saying unsaved.

## `private sealed class TEstablishmentObserver : LObserver`

Collects every bulletin the engine raises, in order.
