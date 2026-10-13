# Capsule

Tests for Deportment's Capsule file, run under a real workspace folder with no window.

## `TCapsule`

A saved state reads back whole, and no pending file is left beside it.
No file, junk text or a JSON array reads as the default state, so the window always opens.
A null or blank root reads the default state and saves nothing, since it names no file.

## Stamp

Commit: `c2f335c331fe`
- `TCapsule.cs` `a1e4da75536d795b`
