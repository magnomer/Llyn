# Descent

This folder holds the tone correspondence of a borrowing language against the rime-book tone classes.
It stands apart from `Hypothesis`, which declares the classes, and from `Anatomy`, which cuts the contours.

## `LDescent`

The tone correspondence of one borrowing language.
It names which rime-book tone classes a contour of that language descends from.
Mandarin `55` descends from class 1 and, for a checked syllable, class 7.
Cantonese `13` descends from 4S or 4, since both fell together there.
The Classical Chinese pack declares one row per reflex language in `anatomy_tone.json`, beside `anatomy.json`.
The classes named are the class keys of the pack's own `LHypothesisTone` rows.
The engine hands the rows to a view, and the view marks the placements whose class the contour allows.
Each contour is keyed as the anatomy cuts it, such as `214`.

## `LDescentJoiner`

The joiner the anatomy keeps between a citation tone and its sandhi form, as `214-21`.

## `LDescentMatch`

Whether the row serves the language, compared without case.

## `LDescentResolve`

The classes the contour may descend from, or none for a blank or unlisted contour.
Only the citation tone before the joiner is looked up, since that is the tone the character carries alone.

## `LDescentScan`

The classes under the first row that serves the language, or none when no row does.

## Stamp

Commit: `c2f335c331fe`
- `LDescent.cs` `e79f6eb5541e2a1b`
