# Paradigm

This folder holds the rule book's plan of how the inflection box lays out views, rows and cells.
Headers and labels are localization keys the pack names, carried opaque and never translated here.

## `LInflectionLayout`

How the forms of one part of speech are laid out in the inflection box.
Both views target the same part, but each sheet may select different cells.

## `LInflectionLine`

One row of a view, with the cells it shows in order.
The group label sits on the first line of its group only, so later lines carry an empty group.
Loader-built cells contain distinct ascending codes, so they match slot codes by exact sequence.

## `LInflectionSheet`

The plan of one view of the inflection box.
The UI language picks the header text, so the engine never names a language.

## Stamp

Commit: `c2f335c331fe`
- `LInflectionLayout.cs` `01e6bf69064874cf`
- `LInflectionLine.cs` `e5849ac49a233a0f`
- `LInflectionSheet.cs` `d4db41b1b6784598`
