# QFreedomItem.cs
Hash: `27469f2726afbac9`

## `internal sealed class QFreedomItem`

Driver item for one row of the `PFreedomDropdown` menu.
It carries the key Conduct's row names the freedom by.
That key both draws the row and goes back to the gate.
It also carries whether the draft holds the freedom, which draws a check.

The item is immutable, because the menu is rebuilt rather than edited.
