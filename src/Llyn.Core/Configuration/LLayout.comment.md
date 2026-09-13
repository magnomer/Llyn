# LLayout.cs

## `public sealed record LLayout(`

The widths of one tab's side-by-side panels, carried between two runs of the program.
With them ride the ordering the tab lists by and the languages it hides.
Both are view state of the same tab.
It is part of the settings file, so it belongs to the workspace like the window geometry.
Only the fixed columns are stored, because the last column takes whatever room the window leaves.
A width left empty means the tab keeps the width it was designed with.
An ordering or filter left empty means the tab opens as it was designed to.
None of it is lexical data, so it lives in the settings file rather than the workspace database.

**Parameters**

- `LLayoutTab` — The lowercase name of the tab the widths belong to, for example `"library"`.
- `LLayoutLeft` — The width of the leftmost panel, or nothing before it was ever dragged.
- `LLayoutMiddle` — The width of the middle panel where a tab has three, or nothing otherwise.
- `LLayoutOrder` — The ordering the tab's catalog lists by, or nothing before one was chosen.
- `LLayoutFilter` — The languages the tab hides from its entries, or nothing before any was hidden.
