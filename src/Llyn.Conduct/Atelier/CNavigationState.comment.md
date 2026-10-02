# CNavigationState.cs
Hash: `1c11754f58b51511`

## `public sealed record CNavigationState(`

What the window paints after a navigation change.
Every change carries the whole state, so the window keeps no copy of its own.

**Parameters**

- `CNavigationStateTab`: the tab that now stands open, or null when the change leaves the open tab alone.
  A startup with no stored tab and a voyage record leave it null.
- `CNavigationStateHidden`: the tabs the session does not offer, as the last startup found them.
- `CNavigationStateVoyage`: whether the voyage can step back or forward.
