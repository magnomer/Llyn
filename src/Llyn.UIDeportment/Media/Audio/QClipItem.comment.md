# QClipItem.cs
Hash: `fcd2539c79608e01`

## `internal sealed class QClipItem`

One source row of the audio menu, painted from the errand's ready `CClipItem`.
It only turns the row's keys into text and its readings into previewable entries.
It raises no change, since the menu builds its rows afresh from each clip state.

## `public string QClipItemSource`

The source's name as the pack gives it.
It is a proper name, so unlike the notice it is shown without a catalog lookup.

## `public IReadOnlyList<QClipReading> QClipItemReading`

The row's recordings as entries, each label and flag looked up from the ready variety keys.
The label is the localized variety name, and a blank variety shows none.

## `public string QClipItemNotice`

The notice the row shows while it carries no recording, looked up from the errand's key.

## `public bool QClipItemReady`

Whether the row carries a recording the user can take now.
The fill switches the recordings and the notice on it.

## `internal static IReadOnlyList<QClipItem> QClipItemBuild(IReadOnlyList<CClipItem> rows)`

Builds the menu's rows from the clip state, in its order.
