# PSCustomsItem.cs

## `internal sealed class PSCustomsItem : INotifyPropertyChanged`

One row of the customs window: an entry from the file and how it is to enter.
It keeps the entry's position in the file, since the intake is answered by position.
The candidates are the stored entries sharing its headword and language, looked up once before the row is made.
Its starting mode and target come from `LSCustoms.LSCustomsIntakeCreate`.

## `public string PSCustomsItemNumber`

The row's place in the file, counted from one as a reader counts.

## `public string PSCustomsItemHeadword`

The headword as shown, fixed when the row is made.

## `public LMarkupMode PSCustomsItemMode`

The way the entry enters, as the mode dropdown shows it.
Changing it also announces whether a target is wanted, since that follows from the mode.

## `public long PSCustomsItemTarget`

The stored entry the row joins, zero while none is chosen.
It is kept across mode changes, so switching to New and back does not lose the pick.

## `public bool PSCustomsItemTargeted`

Whether the target dropdown is live, true for Merge and Replace and false for New.

## `public string PSCustomsItemLoss`

What a Replace would drop, blank for any other mode.
The window computes it, since counting reads the stored entry through the window deportment.

## `public bool PSCustomsItemReady`

Whether the row can be accepted: New always, the other modes once a target is chosen.
