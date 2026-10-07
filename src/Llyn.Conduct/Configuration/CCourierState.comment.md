# CCourierState.cs
Hash: `5f87ef7985ad4552`

## `public sealed record CCourierState(`

What the status bar shows of the courier, raised whole so the bar never asks piece by piece.

**Parameters**

- `CCourierStateBusy`: whether a push or connect still runs, so a gate refuses a second one.
- `CCourierStateAllowed`: whether both actions are enabled, true exactly when nothing runs.
- `CCourierStateAttached`: whether the workspace holds a Joplin token.
  The view then words itself as connected and drops its connect action.
- `CCourierStateLine`: the worded status line, empty when there is nothing to report.
