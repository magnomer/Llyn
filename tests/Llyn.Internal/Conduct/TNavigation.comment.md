# TNavigation.cs

## `public sealed class TNavigation`

Covers Conduct's navigation over a real posture on the fake rig, with no window.
With nothing stored, the first tab reads as open and startup opens nothing.
A stored tab opens again at startup, and one no longer allowed falls back to the first tab.
A tab click asks only the open tab to be left, and a jump asks its target first.
Any refusal keeps the open tab and saves nothing.

## `private static readonly string[] TNavigationTabs`

Three tab keys in navigation order, the first standing in for the input tab.
