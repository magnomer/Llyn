# TVolumeCatalog.cs
Hash: `7d8119e688ca8282`

## `public sealed class TVolumeCatalog`

Covers the shared volume level that the window's one volume owner paints on every tray.
The level is a plain value, so the tests need no WPF thread.

## `public void VolumeCatalogLevel_RepeatedLevel_RaisesOneNoticePerChange()`

A set to the level already held raises no notice.
This silence keeps the trays and the display from notifying each other in a loop.
