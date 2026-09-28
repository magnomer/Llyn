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

## `public void EstablishmentAmount_AnySize_CountsMegabytesFromOneAndKilobytesRoundedUp(`

The size counts in megabytes once it reaches one, and in kilobytes rounded up below that.
A file of one byte therefore counts as one kilobyte, never as zero.

## `public void DraftCancel_TypedDraft_RaisesDraftBulletinWithZeroId()`

Cancelling raises one draft bulletin carrying id zero, and the count read after it is zero.
The bar listens for that bulletin, so a discard without it would leave the bar saying unsaved.
