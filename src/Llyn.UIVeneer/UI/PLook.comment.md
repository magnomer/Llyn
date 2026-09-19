# PLook.cs

## `internal static class PLook`

Maps a bare fact to the framework value a control property takes.
A veneer member may not branch, so the one `?:` each mapping needs lives here, once.
It owns no state and reads no logic value, only the bool or number it is handed.

## `internal static Visibility PLookVisibleRead(bool shown)`

Visible when shown, else collapsed so the control takes no room.

## `internal static Visibility PLookHiddenRead(bool shown)`

Visible when shown, else hidden so the control keeps its room.

## `internal static bool? PLookCheckedRead(bool chosen)`

The three-state value a toggle's `IsChecked` takes, never indeterminate.

## `internal static double PLookOpacityRead(bool active, double full, double faded)`

The full opacity when active, else the faded one the caller names.

## `internal static PLookChoice PLookFirstRead<PLookChoice>(bool first, PLookChoice chosen, PLookChoice other)`

One of two values by a verdict, so a tab with two edit areas picks the active one without branching.

## `internal static bool PLookCheckedRead(bool? shown)`

A toggle's three-state check read as a plain yes or no, so a handler passes it down without comparing.

## `internal static Thickness PLookThicknessRead(double left)`

A margin that indents from the left alone.
