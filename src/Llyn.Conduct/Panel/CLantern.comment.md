# CLantern.cs

## `public static class CLantern`

The lit row of an offer dropdown, shared by every medium that steps one with the arrow keys.
It keeps no state, since each medium holds its own lit row and offered count.

## `public static int? CLanternMove(int chosen, int count, int step)`

Down or Up lights the next or former row, wrapping at either end.
Down from no lit row lights the first row, and Up from no lit row lights the last.
An empty offer lights nothing and answers null, so the key stays unhandled.
