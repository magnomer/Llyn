# QUnitItem.cs
Hash: `47f0e19bc6379784`

## `internal sealed class QUnitItem`

Driver item for one row of the `PUnitDropdown` menu.
It carries the key Conduct's row names the unit by.
That key both draws the row and goes back to the gate.
It also carries whether the draft holds the unit, which draws a check.

The item is immutable, because the menu is rebuilt rather than edited.
