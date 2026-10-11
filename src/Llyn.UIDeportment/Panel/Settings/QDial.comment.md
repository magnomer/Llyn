# QDial.cs
Hash: `7d06c7dba2fb4048`

## `internal sealed class QDial`

The settings panel's card dial, which shows the page of one group and hides the others.
It owns the table that pairs each group's name with the page the markup draws for it.
Marking the chosen row in the catalog stays with `QSettings`, which calls the dial.

## `internal QDial(FrameworkElement settings)`

Takes the settings panel's surface, where every page is pulled by its contract ID.

## `private StackPanel QDialWorkspace`

Each page of the markup is pulled by its contract ID, which keeps the markup's names.

## `internal void QDialRefine(string child)`

Shows the page of the group named `child` and collapses every other page.
A name the table lacks collapses every page.

## `private (string QDialChild, StackPanel QDialPage)[] QDialTableRead()`

Pairs each group's name with the page the markup draws for it, in the catalog's order.
The Sync group draws the page named `PDialOutpost`, since `Sync` is a verb and names no page.
