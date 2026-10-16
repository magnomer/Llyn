# Guestroom

These tests check which start the launcher's choice runs.
Each start returns its own code, so the code tells which one ran.

## `TGuestroom`

A path ending in `.com` runs the console start, in any letter case.
A path ending in `.exe` runs the window start.
A missing path runs the window start, so the GUI stays the fallback.

## Stamp

Commit: `ed7d18d90760`
- `TGuestroom.cs` `c7365b97aa3bfc87`
