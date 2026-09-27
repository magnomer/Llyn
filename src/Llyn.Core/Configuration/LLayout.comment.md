# LLayout.cs

## `public sealed record LLayout(`

The ordering one tab lists by and the languages it hides, carried between two runs of the program.
Both are view state of the same tab, and a console lists by them as the GUI does.
It is part of the posture the shell keeps, so it belongs to the workspace.
An ordering or filter left empty means the tab opens as it was designed to.
None of it is lexical data, so it lives in the posture file rather than the workspace database.
Panel widths are GUI-only, so they live in the GUI driver's Capsule, not here.

**Parameters**

- `LLayoutTab` — The lowercase name of the tab the record belongs to, for example `"library"`.
- `LLayoutOrder` — The ordering the tab's catalog lists by, or nothing before one was chosen.
- `LLayoutFilter` — The languages the tab hides from its entries, or nothing before any was hidden.
