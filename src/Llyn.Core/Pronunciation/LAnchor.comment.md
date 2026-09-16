# LAnchor.cs

## `public static class LAnchor`

The anchors of a reflex row: the ids of the fanqie rows the user tied that reading to by hand.
Nothing derives an anchor, so a reading counts on a Diwei page only through the placements the user chose.
A row without anchors counts nowhere.
The helpers keep the list sorted and free of repeats, so two lists compare by position.

## `public static IReadOnlyList<long> LAnchorNormalize(IReadOnlyList<long>? anchors)`

The anchors sorted ascending, each once, ids below one dropped, an empty list for `null`.

## `public static IReadOnlyList<long> LAnchorToggle(IReadOnlyList<long> anchors, long fanqieId, bool anchored)`

The list with `fanqieId` added when `anchored` and removed otherwise, normalized.

## `public static bool LAnchorMatch(IReadOnlyList<long> one, IReadOnlyList<long> other)`

Whether two normalized lists hold the same ids in the same order.
The records carrying anchors compare by reference on the list, so the match is done here.
