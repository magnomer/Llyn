# PSCustomsItem.cs

## `internal sealed class PSCustomsItem : INotifyPropertyChanged`

One row of the customs window: an entry from the file and how it is to enter.
It keeps the entry's position in the file, since the intake is answered by position.
The candidates are the stored entries sharing its headword and language, looked up once when the row is made.
A row with exactly one candidate starts as Merge into it, and every other row starts as New.

## `public string PSCustomsItemNumber`

The row's place in the file, counted from one as a reader counts.

## `public string PSCustomsItemHeadword`

The headword as shown, which twin numbering may have extended.
It announces only a real change, so numbering an unshared name stays silent.

## `public LMarkupMode PSCustomsItemMode`

The way the entry enters, bound to the mode dropdown.
Changing it also announces whether a target is wanted, since that follows from the mode.

## `public long PSCustomsItemTarget`

The stored entry the row joins, zero while none is chosen.
It is kept across mode changes, so switching to New and back does not lose the pick.

## `public bool PSCustomsItemTargeted`

Whether the target dropdown is live: true for Merge and Replace, false for New.

## `public string PSCustomsItemLoss`

What a Replace would drop, blank for any other mode.
The window computes it, since counting needs the engine.

## `public bool PSCustomsItemReady`

Whether the row can be accepted: New always, the other modes once a target is chosen.
