# LLayout.cs

## `public sealed record LLayout(`

The widths of one tab's side-by-side panels, carried between two runs of the program.
It is part of the settings file, so it belongs to the workspace like the window geometry.
Only the fixed columns are stored, because the last column takes whatever room the window leaves.
A width left empty means the tab keeps the width it was designed with.

**Parameters**

- `LLayoutTab` — The lowercase name of the tab the widths belong to, for example `"library"`.
- `LLayoutLeft` — The width of the leftmost panel, or nothing before it was ever dragged.
- `LLayoutMiddle` — The width of the middle panel where a tab has three, or nothing otherwise.
