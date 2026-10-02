# CNavigationPanel.cs
Hash: `269271317524a145`

## `internal sealed record CNavigationPanel(`

The hooks one panel area registers with the navigation for its tab.
Every hook is a member of the area itself, so no hook comes up from a window.

**Parameters**

- `CNavigationPanelLeave`: the area's own leave question, asked through its envoy.
- `CNavigationPanelStation`: the record the area stands on, zero for none.
- `CNavigationPanelScribe`: the area's editor restore at startup.
- `CNavigationPanelArrival`: the area's open of a record a jump lands on.
- `CNavigationPanelAllowed`: whether the tab is offered, or null when it always is.
