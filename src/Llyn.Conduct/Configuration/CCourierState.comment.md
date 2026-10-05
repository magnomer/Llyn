# CCourierState.cs
Hash: `6f9aa4129bc6db9f`

## `public sealed record CCourierState(bool CCourierStateBusy, bool CCourierStateAllowed, string CCourierStateLine);`

What a Joplin view shows of the courier, raised whole so the view never asks piece by piece.

**Parameters**

- `CCourierStateBusy`: whether a push or connect still runs, so a gate refuses a second one.
- `CCourierStateAllowed`: whether both actions are enabled, true exactly when nothing runs.
- `CCourierStateLine`: the worded status line, empty when there is nothing to report.
