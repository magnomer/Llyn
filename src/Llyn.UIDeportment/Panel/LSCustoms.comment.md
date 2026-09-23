# LSCustoms.cs

## `public static class LSCustoms`

The customs window's deportment: what a Replace row would drop from the stored entry.

## `public static string LSCustomsLossResolve(LWindow? window, LMarkupMode mode, long target, string format)`

The meanings and collocations a Replace would drop, counted from the stored entry.
The format is the window's localized text, filled with both counts.
Blank for any other mode, and blank when the target cannot be read.

## `public static int LSCustomsCardScan(IReadOnlyList<LCardDraft> cards)`

Counts the cards and every card nested under them.
Nested meanings are counted too, since every one of them goes.

## `public static LMarkupIntake LSCustomsIntakeCreate(int index, IReadOnlyList<LEntry> candidates)`

A row with exactly one candidate starts as Merge into it, and every other row starts as New.

## `public static LMarkupIntake LSCustomsIntakeCreate(int index, LMarkupMode mode, long target)`

The intake a row sends when the window is accepted.
A New row sends no target, though the row keeps its pick across mode changes.
So the engine never sees a stale target it would have to know to ignore.

## `public static bool LSCustomsTargetCheck(LMarkupMode mode)`

Whether a mode wants a target: true for Merge and Replace, false for New.

## `public static bool LSCustomsReadyCheck(LMarkupMode mode, long target)`

Whether a row can be accepted: New always, the other modes once a target is chosen.
