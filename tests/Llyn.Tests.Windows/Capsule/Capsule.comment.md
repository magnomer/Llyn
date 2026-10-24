# Capsule

Tests for Deportment's Capsule file, run under a real workspace folder with no window.

## `TCapsule`

A saved state reads back whole, and no pending file is left beside it.
A save over a read-only file throws, keeps the old state and leaves no pending file.
No file, junk text or a JSON array reads as the default state, so the window always opens.
A null or blank root reads the default state and saves nothing, since it names no file.

## Stamp

Commit: `5813c19fd19c`
- `TCapsule.cs` `47f4eab2d35c43ef`
