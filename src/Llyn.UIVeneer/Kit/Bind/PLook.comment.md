# PLook.cs

## `internal static class PLook`

Maps a bare fact to the framework value a control property takes.
A veneer member may not branch, so the one `?:` each mapping needs lives here, once.
It owns no state and reads no logic value, only the bool or number it is handed.

## `internal static Visibility PLookVisibleRead(bool shown)`

Visible when shown, else collapsed so the control takes no room.

## `internal static bool? PLookCheckedRead(bool chosen)`

The three-state value a toggle's `IsChecked` takes, never indeterminate.

## `internal static PLookChoice PLookFirstRead<PLookChoice>(bool first, PLookChoice chosen, PLookChoice other)`

One of two values by a verdict, so a tab with two edit areas picks the active one without branching.

## `internal static bool PLookCheckedRead(bool? shown)`

A toggle's three-state check read as a plain yes or no, so a handler passes it down without comparing.
