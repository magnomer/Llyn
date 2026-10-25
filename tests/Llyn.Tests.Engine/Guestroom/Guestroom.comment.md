# Guestroom

These tests check which start the launcher's choice runs.
Each start returns its own code, so the code tells which one ran.

## `TGuestroom`

A path ending in `.com` runs the console start, in any letter case.
A path ending in `.exe` runs the window start.
A missing path runs the window start, so the GUI stays the fallback.
A marked start runs the console start whatever the path, `.exe` or missing included.

## Stamp

Commit: `8c403d1df768`
- `TGuestroom.cs` `b5582f401c69aeb8`
