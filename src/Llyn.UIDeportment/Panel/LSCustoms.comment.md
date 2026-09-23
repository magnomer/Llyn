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
