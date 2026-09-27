# CLayout.cs

## `public sealed record CLayout(string CLayoutTab, double? CLayoutLeft, double? CLayoutMiddle);`

The column widths one panel's layout keeps.
The engine's layout also holds the order and filter, which the vista reads itself.

**Parameters**

- `CLayoutTab`: the panel the widths belong to.
- `CLayoutLeft`: the left column's width, null when none is stored.
- `CLayoutMiddle`: the middle column's width, null when none is stored.
